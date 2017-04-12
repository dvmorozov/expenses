using System;
using SocialApps.Models;
using System.Linq;
using System.IO;
using System.Collections.Generic;

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

        public Expenses GetExpense(Guid userId, int expenseId)
        {
            return _db.Expenses.Single(t => t.DataOwner == userId && t.ID == expenseId);
        }
    }
}