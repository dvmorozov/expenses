using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SocialApps.Models;
using System.Web;
using System.IO;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        private void FillExpenseLinks(string returnToMethod,
            //  https://www.evernote.com/shard/s132/nl/14501366/eb75b683-fead-4822-9d38-17e50ab7de2f
            object returnToParams = null)
        {
            var sessionLinks = (List<int>)(Session["SessionLinks"]);

            if (sessionLinks != null)
            {
                var userId = GetUserId();
                var links =
                    (from link in _db.Links
                        where sessionLinks.Contains(link.ID) && (link.DataOwner == userId)
                        select new LinkModel { Id = link.ID, URL = link.URL, Name = link.Name }).ToList();

                ViewBag.Links = links;
            }

            Session["InitialPageWithLinks"] = returnToMethod;
            Session["InitialPageWithLinksData"] = returnToParams;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        private void DropSessionLinks()
        {
            Session["SessionLinks"] = null;
            Session["InitialPageWithLinks"] = null;
        }

        [HttpPost]
        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        public ActionResult UploadDoc(IEnumerable<HttpPostedFileBase> file)
        {
            try
            {
                foreach (HttpPostedFileBase f in file)
                {
                    if (f == null) continue;

                    //  https://action.mindjet.com/task/14509395
                    int linkId;
                    _repository.PutDocument(GetUserId(), out linkId, f.InputStream, f.FileName);

                    var sessionLinks = (List<int>)Session["SessionLinks"] ?? new List<int>();
                    sessionLinks.Add(linkId);
                    Session["SessionLinks"] = sessionLinks;
                }

                var initialPageWithLinks = (string)Session["InitialPageWithLinks"];
                //  https://www.evernote.com/shard/s132/nl/14501366/eb75b683-fead-4822-9d38-17e50ab7de2f
                return RedirectToAction(initialPageWithLinks ?? "NewExpense", Session["InitialPageWithLinksData"]);
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "UploadDoc"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        public ActionResult UnlinkDoc(int linkId)
        {
            try
            {
                if (Session["ExpenseId"] == null)
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                var expenseId = (int)Session["ExpenseId"];

                var userId = GetUserId();
                var links =
                    (from link in _db.ExpensesLinks
                     where (link.LinkID == linkId) && (link.ExpenseID == expenseId)
                     select link);

                foreach(var link in links)
                    _db.ExpensesLinks.Remove(link);

                _db.SaveChanges();

                return RedirectToAction("DocListByExpense", new { expenseId = expenseId });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "UnlinkDoc"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        private List<LinkModel> GetLinkedDocs(int expenseId)
        {
            var userId = GetUserId();

            return
                (from link in _db.Links
                 join linkExp in _db.ExpensesLinks on link.ID equals linkExp.LinkID
                 where (linkExp.ExpenseID == expenseId) && (link.DataOwner == userId)
                 select new LinkModel { Id = link.ID, URL = link.URL, Name = link.Name }).ToList();
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        private bool HasLinkedDocs(int expenseId)
        {
            return GetLinkedDocs(expenseId).Count() != 0;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        public ActionResult DocListByExpense(int expenseId)
        {
            try
            {
                var links = GetLinkedDocs(expenseId);
                ViewBag.Links = links;

                var expense = _db.Expenses.Single(t => t.ID == expenseId);
                Session["ExpenseId"] = expenseId;

                return View("DocListByExpense",
                    new NewExpense
                    {
                        ExpenseId = expenseId,
                        Name = expense.Name.Trim(),
                        Cost = (expense.Cost != null ? ((double)expense.Cost).ToString() : string.Empty),
                        Currency = expense.Currency,
                        Day = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Day : -1),
                        EncryptedName = expense.EncryptedName,
                        EndMonth = (expense.LastMonth != null ? ((DateTime)expense.LastMonth).Month : -1),
                        EndYear = (expense.LastMonth != null ? ((DateTime)expense.LastMonth).Year : -1),
                        Forever = expense.LastMonth == null,
                        Hour = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Hour : -1),
                        Min = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Minute : -1),
                        Month = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Month : -1),
                        Monthly = expense.Monthly != null ? (bool)expense.Monthly : false,
                        Sec = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Second : -1),
                        StartMonth = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Month : -1),
                        StartYear = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Year : -1),
                        Year = (expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Year : -1)
                    });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "DocListByExpense"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        public FileContentResult Document(int linkId)
        {
            try
            {
                var userId = GetUserId();

                //  https://action.mindjet.com/task/14509395
                string fileName;
                MemoryStream stream;
                _repository.GetDocument(GetUserId(), linkId, out stream, out fileName);

                var contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                switch (Path.GetExtension(fileName).ToLower())
                {
                    case(".jpg"):
                    case(".jpeg"):
                        contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                        break;

                    case(".pdf"):
                        contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                        break;

                    case (".rtf"):
                        contentType = System.Net.Mime.MediaTypeNames.Application.Rtf;
                        break;

                    case (".zip"):
                        contentType = System.Net.Mime.MediaTypeNames.Application.Zip;
                        break;

                    case (".gif"):
                        contentType = System.Net.Mime.MediaTypeNames.Image.Gif;
                        break;

                    case (".tiff"):
                        contentType = System.Net.Mime.MediaTypeNames.Image.Tiff;
                        break;

                    case (".html"):
                        contentType = System.Net.Mime.MediaTypeNames.Text.Html;
                        break;

                    case (".txt"):
                        contentType = System.Net.Mime.MediaTypeNames.Text.Plain;
                        break;

                    case (".xml"):
                        contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                        break;
                }

                return File(stream.ToArray(), contentType, fileName);
            }
            catch (Exception e)
            {
                Application_Error(e);
                return File(System.Text.Encoding.UTF8.GetBytes(e.Message), System.Net.Mime.MediaTypeNames.Application.Octet, "Exception.txt");
            }
        }
    }
}
