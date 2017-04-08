using SocialApps.Models;
using System;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using System.Web;
using System.Configuration;

namespace SocialApps.Repositories
{
    //  https://action.mindjet.com/task/14509395
    public partial class MobileRepository
    {
        private ExpensesEntities _db;
        private HttpSessionStateBase _session;

        public MobileRepository(ExpensesEntities db, HttpSessionStateBase session)
        {
            _db = db;
            _session = session;
        }

        //  https://action.mindjet.com/task/14509395
        private CloudBlobContainer GetContainerReference(Guid userId)
        {
            //  https://action.mindjet.com/task/14886358
#if DEBUG
            var credentials = new StorageCredentials();
            var configuration = ConfigurationManager.AppSettings["StorageConnectionString"];
            var _storageAccount = CloudStorageAccount.Parse(configuration);
#else
            var account = CloudConfigurationManager.GetSetting("StorageAccount");
            var key = CloudConfigurationManager.GetSetting("StorageKey");
            var credentials = new StorageCredentials(account, key);
            var _storageAccount = new CloudStorageAccount(credentials, true);
#endif
            // Create the blob client.
            var blobClient = _storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(userId.ToString());
            if (!container.Exists())
                container.Create(BlobContainerPublicAccessType.Off);

            return container;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        //  https://action.mindjet.com/task/14509395
        private CloudBlockBlob GetBlob(string fileName, Guid userId)
        {
            var container = GetContainerReference(userId);
            return container.GetBlockBlobReference(fileName);
        }

        //  https://action.mindjet.com/task/14509395
        private string UploadBlob(Guid userId, Stream stream, string fileName)
        {
            var blockBlob = GetBlob(fileName, userId);
            blockBlob.UploadFromStream(stream);
            return blockBlob.Uri.ToString();
        }

        //  https://action.mindjet.com/task/14509395
        private string UploadText(Guid userId, string text, string fileName)
        {
            var blockBlob = GetBlob(fileName, userId);
            blockBlob.UploadText(text);
            return blockBlob.Uri.ToString();
        }

        //  https://action.mindjet.com/task/14509395
        private bool DownloadBlob(Guid userId, out MemoryStream stream, string fileName)
        {
            var blob = GetBlob(fileName, userId);
            stream = new MemoryStream();
            if (!blob.Exists()) return false;

            blob.DownloadToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return true;
        }

        //  https://action.mindjet.com/task/14509395
        private bool DownloadText(Guid userId, out string text, string fileName)
        {
            var blob = GetBlob(fileName, userId);
            text = "";
            if (!blob.Exists()) return false;

            text = blob.DownloadText();
            return true;
        }

        //  https://action.mindjet.com/task/14509395
        private void DeleteBlobs(Guid userId, string prefix)
        {
            var container = GetContainerReference(userId);

            foreach (var blob in container.ListBlobs(prefix))
            {
                container.GetBlockBlobReference(((CloudBlockBlob)blob).Name).DeleteIfExists();
            }
        }
    }
}
