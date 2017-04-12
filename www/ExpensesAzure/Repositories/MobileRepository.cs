using SocialApps.Models;
using System;
using System.Linq;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using System.Web;
using System.Collections.Generic;
using System.Globalization;
#if DEBUG
using System.Configuration;
#endif

namespace SocialApps.Repositories
{
    //  https://action.mindjet.com/task/14509395
    public partial class MobileRepository: IDisposable
    {
        private ExpensesEntities _db;
        private HttpSessionStateBase _session;

        public MobileRepository(HttpSessionStateBase session)
        {
            //  https://action.mindjet.com/task/14889062
            _db = new ExpensesEntities();
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

        public List<TodayExpense> GetDayExpenses(Guid userId, DateTime date)
        {
            //  For this application it's more convenient to order the list in reverse cost order.
            //  https://www.evernote.com/shard/s132/nl/14501366/49348fc0-3dc6-45cb-8425-6fe72042eac2
            List<TodayExpense> todayExpenses = (from exp in _db.Expenses
                                                join expCat in _db.ExpensesCategories on exp.ID equals expCat.ExpenseID
                                                join cat in _db.Categories on expCat.CategoryID equals cat.ID
                                                where (exp.DataOwner == userId) &&
                                                    (
                                                        ((exp.Monthly == null || !(bool)exp.Monthly) &&
                                                        exp.Date.Day == date.Day && exp.Date.Month == date.Month && exp.Date.Year == date.Year) ||
                                                        ((exp.Monthly != null && (bool)exp.Monthly) &&
                                                        exp.Date.Day == date.Day && date >= exp.FirstMonth && (exp.LastMonth == null || date <= exp.LastMonth))
                                                    )
                                                orderby exp.ID descending
                                                select new TodayExpense
                                                {
                                                    CategoryEncryptedName = cat.EncryptedName,
                                                    CategoryName = cat.Name,
                                                    Cost = exp.Cost,
                                                    Currency = exp.Currency,
                                                    Date = exp.Date,
                                                    ExpenseEncryptedName = exp.EncryptedName,
                                                    ID = exp.ID,
                                                    Name = exp.Name,
                                                    Note = exp.Note,
                                                    Rating = exp.Rating,
                                                    Importance = exp.Importance,
                                                    //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
                                                    Project = exp.Project
                                                }).ToList();

            //  Must be outside LINQ expression.
            foreach (var e in todayExpenses)
                e.HasLinkedDocs = HasLinkedDocs(e.ID, userId);

            return todayExpenses;
        }

        public TodayExpensesSumsByUser_Result[] GetDayExpenseTotals(Guid userId, DateTime date)
        {
            return _db.TodayExpensesSumsByUser(date, userId).
                OrderBy(t => t.CategoryEncryptedName).
                ThenBy(t => t.CategoryName).
                ThenBy(t => t.Currency).
                ThenBy(t => t.Cost).Reverse().ToArray();
        }

        public TodayAndMonthTotalByUser2_Result GetTodayAndMonthTotals(Guid userId, DateTime date)
        {
            return _db.TodayAndMonthTotalByUser2(date, userId).FirstOrDefault();
        }

        public List<EstimatedTop10CategoriesForMonthByUser2_Result> GetTop10Categories(Guid userId, DateTime now)
        {
            return _db.EstimatedTop10CategoriesForMonthByUser2(now.Year, now.Month, now.Day, userId).ToList();
        }

        public List<LastYearTotalExpensesByMonthByUser_Result> GetLastYearTotalExpensesByMonth(Guid userId, int lastMonthNumber)
        {
            return _db.LastYearTotalExpensesByMonthByUser(lastMonthNumber, userId).ToList();
        }

        public List<LastYearBalanceByMonthByUser_Result> GetLastYearBalanceByMonth(Guid userId, int lastMonthNumber)
        {
            return _db.LastYearBalanceByMonthByUser(lastMonthNumber, userId).ToList();
        }

        public List<LastYearCategoryExpensesByMonthByUser_Result> GetLastYearCategoryExpensesByMonth(Guid userId, int categoryId, int lmn)
        {
            return _db.LastYearCategoryExpensesByMonthByUser(categoryId, lmn, userId).ToList();
        }

        public void AddMonthBudget(int year, int month, decimal budget, Guid userId, string currency)
        {
            _db.AddMonthBudgetByUser2(year, month, budget, userId, currency);
        }

        public ExpenseNameWithCategory[] GetExpenseNamesWithCategory(Guid userId, DateTime date, int cat)
        {
            var expenses =
            (
                from exp in
                    //  https://action.mindjet.com/task/14479694
                    _db.GetExpenseNamesWithCategoryByUser5(cat, userId, date.Year, date.Month, date.Day, false)
                select new ExpenseNameWithCategory
                {
                    Count = 1, //exp.Count,
                    EncryptedName = exp.EncryptedName,
                    Name = exp.Name,
                    Id = exp.Id
                }
            ).ToArray();

            return expenses;
        }

        public bool? IsIncome(int expenseId)
        {
            return _db.Operations.First(t => t.ID == expenseId).Income;
        }

        public Operations GetIncome(int incomeId)
        {
            return _db.Operations.First(t => t.ID == incomeId);
        }

        public Expenses GetExpense(int expenseId)
        {
            return _db.Expenses.First(t => t.ID == expenseId);
        }

        public CategoryModel GetCategory(int categoryId)
        {
            var category = _db.Categories.Single(t => t.ID == categoryId);
            var model = new CategoryModel
            {
                Name = category.Name.Trim(),
                Limit = category.Limit != null ? ((float)category.Limit).ToString(CultureInfo.InvariantCulture) : "",
                Id = categoryId,
                EncryptedName = category.EncryptedName
            };
            return model;
        }

        public TodayExpense[] GetExpensesByCategory(Guid userId, DateTime date, int categoryId)
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/efb1faa9-2d68-4d40-b7e3-3eb9a0b2c1fe
            //  https://www.evernote.com/shard/s132/nl/14501366/d2c71f03-0b70-441f-bd16-9587f82850ae
            //  https://www.evernote.com/shard/s132/nl/14501366/c03c9b9e-5375-4177-bac3-f7e9e50c3d12
            var expenses =
               (from exp in _db.Expenses
                join expCat in _db.ExpensesCategories on exp.ID equals expCat.ExpenseID
                join cat in _db.Categories on expCat.CategoryID equals cat.ID
                where (exp.DataOwner == userId) && (cat.ID == categoryId) &&
                (
                    ((exp.Monthly == null || !(bool)exp.Monthly) &&
                    exp.Date.Month == date.Month && exp.Date.Year == date.Year) ||
                    ((exp.Monthly != null && (bool)exp.Monthly) &&
                    date >= exp.FirstMonth && (exp.LastMonth == null || date <= exp.LastMonth))
                )
                orderby exp.Date
                select new TodayExpense
                {
                    Name = exp.Name,
                    Cost = exp.Cost,
                    ExpenseEncryptedName = exp.EncryptedName,
                    Date = exp.Date,
                    Importance = exp.Importance,
                    Rating = exp.Rating
                }).ToArray();

            return expenses;
        }

        public TodayExpense[] GetExpensesByImportance(Guid userId, DateTime date, int importance)
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/efb1faa9-2d68-4d40-b7e3-3eb9a0b2c1fe
            //  https://www.evernote.com/shard/s132/nl/14501366/d2c71f03-0b70-441f-bd16-9587f82850ae
            //  https://www.evernote.com/shard/s132/nl/14501366/c03c9b9e-5375-4177-bac3-f7e9e50c3d12
            var expenses =
               (from exp in _db.Expenses
                where (exp.DataOwner == userId) && (exp.Importance == importance) &&
                (
                    ((exp.Monthly == null || !(bool)exp.Monthly) &&
                    exp.Date.Month == date.Month && exp.Date.Year == date.Year) ||
                    ((exp.Monthly != null && (bool)exp.Monthly) &&
                    date >= exp.FirstMonth && (exp.LastMonth == null || date <= exp.LastMonth))
                )
                orderby exp.Date
                select new TodayExpense
                {
                    Name = exp.Name,
                    Cost = exp.Cost,
                    ExpenseEncryptedName = exp.EncryptedName,
                    Date = exp.Date,
                    Importance = exp.Importance,
                    Rating = exp.Rating
                }).ToArray();

            return expenses;
        }

        public class EncryptedListItem
        {
            public int Id = -1;
            public string OpenText = null;
            public string EncryptedText = null;
        }

        public class EncryptedList
        {
            public EncryptedListItem[] List = null;
        }

        public void UpdateExpenses(Guid userId, EncryptedList list)
        {
            foreach (var exp in list.List)
            {
                _db.UpdateExpenseByUser(exp.Id, exp.OpenText, exp.EncryptedText, userId);
            }
        }

        public void UpdateCategories(Guid userId, EncryptedList list)
        {
            foreach (var cat in list.List)
            {
                _db.UpdateCategoryByUser(cat.Id, cat.OpenText, cat.EncryptedText, userId);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/003838f7-d618-449e-836b-11b8a26669c2
        public void FillInitialCategories(Guid userId)
        {
            string[] catNames = {"Pets", "Food", "Fuel", "Home", "Car", "Loans", "Mobile", "Internet", "Clothing", "Medicine",
                                 "Gifts", "Amusements", "Taxes", "Interior", "Household", "Transport", "Education", "Sport"};

            foreach (var cat in catNames.OrderBy(t => t))
            {
                _db.Categories.Add(
                    new Categories
                    {
                        Name = cat,
                        DataOwner = userId
                    }
                );
                _db.SaveChanges();
            }
        }

        public MonthBudgetByUser_Result GetMonthBudget(Guid userId, DateTime date)
        {
            return _db.MonthBudgetByUser(date, userId).FirstOrDefault();
        }

        public decimal? GetMonthIncome(Guid userId, DateTime date)
        {
            return _db.MonthIncomeByUser(date, userId).FirstOrDefault();
        }

        public List<TodayExpense> GetIncomesForMonth(Guid userId, DateTime date)
        {
            //  https://action.mindjet.com/task/14834466
            //  https://vision.mindjet.com/action/task/14485587
            //  https://action.mindjet.com/task/14665340
            var expenses =
               (from exp in _db.IncomsForMonthByUser3(date.Year, date.Month, userId)
                select new TodayExpense
                {
                    Name = exp.Name,
                    Cost = exp.Amount,
                    ExpenseEncryptedName = exp.EncryptedName,
                    ID = exp.ID,
                    Date = exp.Date,
                    HasLinkedDocs = false,
                    Currency = exp.Currency,
                    Note = exp.Note
                }).ToList();

            return expenses;
        }

        public ExpenseNameWithCategory[] GetIncomeNames(Guid userId, DateTime date, bool? shortList)
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
            var expenses = (
                    from exp in
                        //  https://action.mindjet.com/task/14479694
                        _db.GetIncomeNamesByUser(userId, date.Year, date.Month, date.Day, shortList)
                    select new ExpenseNameWithCategory
                    {
                        Count = 1, //exp.Count,
                        EncryptedName = exp.EncryptedName,
                        Name = exp.Name,
                        Id = exp.Id
                    }
                ).ToArray();

            return expenses;
        }

        public void AddMonthIncome(Guid userId, int year, int month, decimal income, int? reset)
        {
            if (reset == null || reset == 0)
                _db.AddMonthIncomeByUser((int)year, (int)month, income, userId);
            else
                _db.ResetMonthIncomeByUser((int)year, (int)month, income, userId);
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/a499d49f-68c6-4370-941d-f4beb5c87c74
        public enum ExpenseImportance
        {
            Liability = 5,
            Asset = 4,
            Necessary = 3,
            Pleasure = 2,
            Unnecessary = 1
        }

        public List<MonthImportance> GetMonthImportances(Guid userId, DateTime now)
        {
            return (
            from groups in
                (
                    from exp in
                    (
                        from exp in _db.Expenses
                        where (exp.DataOwner == userId) &&
                            (
                                ((exp.Monthly == null || !(bool)exp.Monthly) &&
                                exp.Date.Month == now.Month && exp.Date.Year == now.Year) ||
                                ((exp.Monthly != null && (bool)exp.Monthly) &&
                                now >= exp.FirstMonth && (exp.LastMonth == null || now <= exp.LastMonth))
                            )
                        select new MonthImportance
                        {
                            Sum = exp.Cost != null ? (double)exp.Cost : 0.0,
                            Importance = exp.Importance != null ? (short)exp.Importance : (short)ExpenseImportance.Necessary
                        }
                    )
                    group exp by exp.Importance into g
                    select new MonthImportance
                    {
                        Sum = g.Sum(t => t.Sum),
                        Importance = g.FirstOrDefault().Importance
                    }
                )
            orderby groups.Importance descending
            select new MonthImportance
            {
                Sum = groups.Sum,
                Importance = groups.Importance
            }
            ).ToList();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
