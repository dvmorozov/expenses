using System;
using SocialApps.Models;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
#if DEBUG
using System.Configuration;
#else
using Microsoft.Azure;
#endif

namespace SocialApps.Repositories
{
    public partial class MobileRepository
    {
        //  https://action.mindjet.com/task/14509395
        public void GetDocument(Guid userId, int linkId, out MemoryStream stream, out string fileName)
        {
            var linkData =
                (from link in _db.Links
                where (link.ID == linkId) && (link.DataOwner == userId)
                select new LinkModel { Id = link.ID, URL = link.URL, Name = link.Name }).First();

            DownloadBlob(userId, out stream, (new Uri(linkData.URL)).Segments.Last());
            fileName = linkData.Name;
        }

        //  https://action.mindjet.com/task/14509395
        public void PutDocument(Guid userId, out int linkId, Stream stream, string fileName)
        {
            //  File name for BLOB is replaced by GUID.
            var link = _db.Links.Add(new Links { URL = UploadBlob(userId, stream, Guid.NewGuid().ToString()), Name = fileName, DataOwner = userId });
            _db.SaveChanges();

            linkId = link.ID;
        }

        public List<LinkModel> GetLinks(Guid userId, List<int> sessionLinks)
        {
            var links =
                (from link in _db.Links
                where sessionLinks.Contains(link.ID) && (link.DataOwner == userId)
                select new LinkModel { Id = link.ID, URL = link.URL, Name = link.Name }).ToList();

            return links;
        }

        public void RemoveLinks(int linkId, int expenseId)
        {
            var links =
                (from link in _db.ExpensesLinks
                where (link.LinkID == linkId) && (link.ExpenseID == expenseId)
                select link);

            foreach (var link in links)
                _db.ExpensesLinks.Remove(link);

            _db.SaveChanges();
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

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        public List<LinkModel> GetLinkedDocs(int expenseId, Guid userId)
        {
            return
                (from link in _db.Links
                 join linkExp in _db.ExpensesLinks on link.ID equals linkExp.LinkID
                 where (linkExp.ExpenseID == expenseId) && (link.DataOwner == userId)
                 select new LinkModel { Id = link.ID, URL = link.URL, Name = link.Name }).ToList();
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        private bool HasLinkedDocs(int expenseId, Guid userId)
        {
            return GetLinkedDocs(expenseId, userId).Count() != 0;
        }
    }
}