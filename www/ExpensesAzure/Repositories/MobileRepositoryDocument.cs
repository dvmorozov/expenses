using System;
using SocialApps.Models;
using System.Linq;
using System.IO;

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
    }
}