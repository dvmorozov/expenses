using Newtonsoft.Json;
using SocialApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialApps.Repositories
{
    public partial class MobileRepository
    {
        //  Cached expense data.
        private class SelectedExpenses
        {
            public DateTime Date;
            public bool ShortList;
            public int CatId;
            public ExpenseNameWithCategory[] List = null;
        }

        private const string SelectExpensesPrefix = "SelectExpenses";

        //  https://action.mindjet.com/task/14509395
        private void DeleteCachedExpenses(Guid userId, int catId)
        {
            try
            {
                DeleteBlobs(userId, SelectExpensesPrefix + "_" + catId);
                _session[SelectExpensesPrefix] = null;
            }
            catch
            {
            }
        }

        //  https://action.mindjet.com/task/14509395
        public ExpenseNameWithCategory[] SelectExpenses(int catId, Guid userId, DateTime date, bool? shortList)
        {
            bool _shortList = (shortList != null && (bool)shortList);

            //  Check if data are already in session.
            if (_session[SelectExpensesPrefix] != null &&
                ((SelectedExpenses)_session[SelectExpensesPrefix]).ShortList == _shortList &&
                ((SelectedExpenses)_session[SelectExpensesPrefix]).CatId == catId)
                return ((SelectedExpenses)_session[SelectExpensesPrefix]).List;

            var fileName = SelectExpensesPrefix + "_" + catId + "_" + date.Year + "_" + date.Month + "_" + date.Day + "_" + _shortList;

            string json;
            if (!DownloadText(userId, out json, fileName))
            {
                //  Removes all possibly existing old files.
                DeleteCachedExpenses(userId, catId);

                //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
                var expensesList =
                (_shortList ?
                (
                    from exp in
                        //  https://action.mindjet.com/task/14479694
                        _db.GetExpenseNamesWithCategoryByUser5(catId, userId, date.Year, date.Month, date.Day, shortList)
                    select new ExpenseNameWithCategory
                    {
                        Count = 1, //exp.Count,
                        EncryptedName = exp.EncryptedName,
                        Name = exp.Name,
                        Id = exp.Id
                    }
                )
                :
                (
                from exp in
                    //  https://action.mindjet.com/task/14479694
                    _db.GetExpenseNamesWithCategoryByUser5(catId, userId, date.Year, date.Month, date.Day, shortList)
                select new ExpenseNameWithCategory
                {
                    Count = 1, //exp.Count,
                    EncryptedName = exp.EncryptedName,
                    Name = exp.Name,
                    Id = exp.Id
                }
                )).ToArray();

                //  https://action.mindjet.com/task/14509395
                //  Converting to JSON.
                json = JsonConvert.SerializeObject(new SelectedExpenses { Date = date, ShortList = _shortList, List = expensesList, CatId = catId });
                UploadText(userId, json, fileName);
            }

            //  Cache data in session.
            _session[SelectExpensesPrefix] = JsonConvert.DeserializeObject<SelectedExpenses>(json);
            return ((SelectedExpenses)_session[SelectExpensesPrefix]).List;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        private void SaveExpenseLinks(int expenseId)
        {
            if (_session["SessionLinks"] != null)
            {
                using (var tran = _db.Database.BeginTransaction())
                {
                    var sessionLinks = (List<int>)_session["SessionLinks"];
                    foreach (var linkId in sessionLinks)
                    {
                        _db.ExpensesLinks.Add(new ExpensesLinks { ExpenseID = expenseId, LinkID = linkId });
                        _db.SaveChanges();
                    }

                    tran.Commit();
                }
            }
        }

        //  Null will cause an exception in adding.
        //  https://www.evernote.com/shard/s132/nl/14501366/517ecc17-f01d-4856-b729-2330c22e4a1e
        private string CheckName(string name, string encryptedName)
        {
            return (name == null || name.Trim() == string.Empty) ?
                   (!string.IsNullOrEmpty(encryptedName) ? "password required" : null) : name;
        }

        //  All empty text fields must be set to NULL.
        //  https://action.mindjet.com/task/14962990
        private string CheckString(string p)
        {
            return (p == null || p.Trim() == string.Empty) ? null : p;
        }

        public void AddExpense(DateTime date, string name, double amount, string note, bool? monthly, DateTime? firstMonth, DateTime? lastMonth, string encryptedName, string currency, short? rating, int? categoryId, short? importance, string project, Guid userId, bool? income = null)
        {
            using (var tran = _db.Database.BeginTransaction())
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
                //  https://www.evernote.com/shard/s132/nl/14501366/49348fc0-3dc6-45cb-8425-6fe72042eac2
                //  https://www.evernote.com/shard/s132/nl/14501366/c816a9eb-d26d-4fbf-9bd4-ad7167f550d5
                var newExpense = _db.Operations.Add(
                    new Operations
                    {
                        Date = date,
                        //  Null will cause an exception in adding.
                        //  https://www.evernote.com/shard/s132/nl/14501366/517ecc17-f01d-4856-b729-2330c22e4a1e
                        Name = CheckName(name, encryptedName),
                        //  All empty text fields must be set to NULL.
                        //  https://action.mindjet.com/task/14962990
                        Cost = amount,
                        Note = CheckString(note),
                        DataOwner = userId,
                        Monthly = monthly,
                        FirstMonth = firstMonth,
                        LastMonth = lastMonth,
                        EncryptedName = CheckString(encryptedName),
                        Currency = CheckString(currency),
                        Rating = rating,
                        Importance = importance,
                        Project = CheckString(project),
                        Income = income
                    }
                );
                _db.SaveChanges();

                var expenseId = newExpense.ID;

                //  https://vision.mindjet.com/action/task/14485574
                if (categoryId != null)
                    _db.ExpensesCategories.Add(
                        new ExpensesCategories
                        {
                            ExpenseID = expenseId,
                            CategoryID = (int)categoryId
                        }
                    );

                _db.SaveChanges();

                tran.Commit();

                SaveExpenseLinks(expenseId);
            }
        }

        //  Adds new type of expenses.
        //  https://action.mindjet.com/task/14509395
        public void NewExpense(DateTime date, string name, double amount, string note, bool? monthly, DateTime? firstMonth, DateTime? lastMonth, string encryptedName, string currency, short? rating, int catId, short? importance, string project, Guid userId, bool? income = null)
        {
            //  Removes all possibly existing old files.
            DeleteCachedExpenses(userId, catId);

            AddExpense(date, name, amount, note, monthly, firstMonth, lastMonth, encryptedName, currency, rating, catId, importance, project, userId, income);
        }

        //  https://action.mindjet.com/task/14509395
        public void DeleteExpense(int expenseId, Guid userId)
        {
            //  This method is used also for deleting incomes.
            //  A category isn't associated with incomes.
            //  https://action.mindjet.com/task/14895523
            if (_db.ExpensesCategories.Where(t => t.ExpenseID == expenseId).Count() != 0)
                DeleteCachedExpenses(userId, _db.ExpensesCategories.First(t => t.ExpenseID == expenseId).CategoryID);

            _db.DeleteOperationByUser(expenseId, userId);
        }

        public Expenses GetExpense(Guid userId, int expenseId)
        {
            return _db.Expenses.Single(t => t.DataOwner == userId && t.ID == expenseId);
        }

        public bool? IsIncome(int expenseId)
        {
            return _db.Operations.First(t => t.ID == expenseId).Income;
        }

        public Operations GetIncome(int incomeId)
        {
            return _db.Operations.First(t => t.ID == incomeId);
        }

        public void EditExpense(int expenseId, Guid userId, DateTime clientExpenseDate, string expenseName, double amount, string note, bool? monthly,
            DateTime? firstMonth, DateTime? lastMonth, string encryptedName, string currency, short? rating, short? importance, string project)
        {
            //  Remove all possibly existing old files.
            //  Category may be absent (for example, for incoms).
            //  https://action.mindjet.com/task/14665340
            var categories = _db.ExpensesCategories.Where(t => t.ExpenseID == expenseId);
            if (categories.Count() != 0)
                DeleteCachedExpenses(userId, categories.First().CategoryID);

            var expense = _db.Operations.First(t => t.ID == expenseId && t.DataOwner == userId);
            //  All empty text fields must be set to NULL.
            //  https://action.mindjet.com/task/14962990
            expense.Date = clientExpenseDate;
            expense.Name = CheckName(expenseName, encryptedName);
            expense.Cost = amount;
            expense.Note = CheckString(note);
            expense.Monthly = monthly;
            expense.FirstMonth = firstMonth;
            expense.LastMonth = lastMonth;
            expense.EncryptedName = CheckString(encryptedName);
            expense.Currency = CheckString(currency);
            expense.Rating = rating;
            expense.Importance = importance;
            //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
            expense.Project = CheckString(project);
            _db.SaveChanges();

            //  https://www.evernote.com/shard/s132/nl/14501366/eb75b683-fead-4822-9d38-17e50ab7de2f
            SaveExpenseLinks(expenseId);
        }

        public List<TodayExpense> GetDayExpenses(Guid userId, DateTime date)
        {
            //  For this application it's more convenient to order the list in reverse cost order.
            //  https://www.evernote.com/shard/s132/nl/14501366/49348fc0-3dc6-45cb-8425-6fe72042eac2
            //  Separate variable is necessary to avoid the error "LINQ to Entities does not recognize 
            //  the method 'System.DateTime AddMonths(Int32)' method, and this method cannot be translated 
            //  into a store expression".
            var dateLast = date.AddMonths(-1);
            List<TodayExpense> expenses =
                (from exp in _db.Expenses
                 join expCat in _db.ExpensesCategories on exp.ID equals expCat.ExpenseID
                 join cat in _db.Categories on expCat.CategoryID equals cat.ID
                 where (exp.DataOwner == userId) &&
                 (
                     // Unrepeatable expense.
                     ((exp.Monthly == null || !(bool)exp.Monthly) &&
                     exp.Date.Day == date.Day && exp.Date.Month == date.Month && exp.Date.Year == date.Year) ||
                     // Repeatable expense.
                     ((exp.Monthly != null && (bool)exp.Monthly) &&
                        exp.Date.Day == date.Day &&
                        //  FirstMonth, LastMonth are optional.
                        //  FirstMonth, LastMonth are defined as date of the first day.
                        (exp.FirstMonth == null || date >= (DateTime)exp.FirstMonth) &&
                        //  Compare with the first day of next month.
                        //  https://action.mindjet.com/task/15101539
                        (exp.LastMonth == null || dateLast < (DateTime)exp.LastMonth))
                 )
                 orderby cat.ID, exp.Currency, exp.Cost descending
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
                     Importance = (ExpenseImportance?)exp.Importance,
                     //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
                     Project = exp.Project,
                     // https://action.mindjet.com/task/14893592
                     CategoryID = cat.ID
                 }).ToList();

            //  Must be outside LINQ expression.
            foreach (var e in expenses)
            {
                e.HasLinkedDocs = HasLinkedDocs(e.ID, userId);
                // All empty string are converted to NULLs.
                // https://action.mindjet.com/task/14962990
                e.CategoryEncryptedName = CheckString(e.CategoryEncryptedName);
                e.CategoryName = CheckString(e.CategoryName);
                e.Currency = CheckString(e.Currency);
                e.ExpenseEncryptedName = CheckString(e.ExpenseEncryptedName);
                e.Name = CheckString(e.Name);
                e.Note = CheckString(e.Note);
                e.Project = CheckString(e.Project);
            }

            return expenses;
        }

        public TodayExpenseSum[] GetDayExpenseTotals(Guid userId, DateTime date)
        {
            return GetDayExpenses(userId, date).GroupBy(t => new
            {
                t.CategoryID,
                t.CategoryName,
                t.CategoryEncryptedName,
                t.ExpenseEncryptedName,
                t.Name,
                t.Currency
            }).Select(s => new TodayExpenseSum
            {
                CategoryID = s.Key.CategoryID,
                Name = s.Key.Name,
                Cost = s.Sum(t => t.Cost),
                CategoryName = s.Key.CategoryName,
                ExpenseEncryptedName = s.Key.ExpenseEncryptedName,
                CategoryEncryptedName = s.Key.CategoryEncryptedName,
                Currency = s.Key.Currency
            })
            .OrderBy(t => t.CategoryID)
            .ThenBy(t => t.Currency)
            .ThenBy(t => t.Cost).ToArray();
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

        public TodayExpense[] GetExpensesByCategory(Guid userId, DateTime date, int categoryId, string currency)
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/efb1faa9-2d68-4d40-b7e3-3eb9a0b2c1fe
            //  https://www.evernote.com/shard/s132/nl/14501366/d2c71f03-0b70-441f-bd16-9587f82850ae
            //  https://www.evernote.com/shard/s132/nl/14501366/c03c9b9e-5375-4177-bac3-f7e9e50c3d12
            //  https://github.com/dvmorozov/expenses/issues/71
            var expenses =
               (from exp in _db.Expenses
                join expCat in _db.ExpensesCategories on exp.ID equals expCat.ExpenseID
                join cat in _db.Categories on expCat.CategoryID equals cat.ID
                where (exp.DataOwner == userId) && (cat.ID == categoryId) && (exp.Currency.Trim() == currency.Trim()) &&
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
                    Importance = (ExpenseImportance?)exp.Importance,
                    Rating = exp.Rating,
                    Currency = exp.Currency
                }).ToArray();

            return expenses;
        }

        //  Returns all expenses not included in the given array of categories.
        //  https://github.com/dvmorozov/expenses/issues/22
        public TodayExpense[] GetExpensesExceptCategory(Guid userId, DateTime date, int[] categoryIds, string currency)
        {
            var expenses =
               (from exp in _db.Expenses
                join expCat in _db.ExpensesCategories on exp.ID equals expCat.ExpenseID
                join cat in _db.Categories on expCat.CategoryID equals cat.ID
                where (exp.DataOwner == userId) && (!categoryIds.Contains(cat.ID)) && (exp.Currency.Trim() == currency.Trim()) &&
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
                    Importance = (ExpenseImportance?)exp.Importance,
                    Rating = exp.Rating,
                    Currency = exp.Currency
                }).ToArray();

            return expenses;
        }

        //  https://action.mindjet.com/task/14896530
        public TodayExpense[] GetExpensesByName(Guid userId, int expenseId)
        {
            var e = (from exp in _db.Expenses
                where (exp.DataOwner == userId) && (exp.ID == expenseId)
                select new TodayExpense
                {
                    Name = exp.Name,
                    ExpenseEncryptedName = exp.EncryptedName,
                }).First();

            var expenses =
               (from exp in _db.Expenses
                where (exp.DataOwner == userId) &&
                (
                    ((exp.Monthly == null || !(bool)exp.Monthly) &&
                    exp.Name == e.Name && exp.EncryptedName == e.ExpenseEncryptedName)
                )
                orderby exp.Date descending
                select new TodayExpense
                {
                    Name = exp.Name,
                    ExpenseEncryptedName = exp.EncryptedName,
                    ID = exp.ID,
                    Date = exp.Date,
                    Rating = exp.Rating,
                    Importance = (ExpenseImportance?)exp.Importance,
                    Currency = exp.Currency,
                    Cost = exp.Cost,
                    Note = exp.Note,
                    Project = exp.Project
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
                    Importance = (ExpenseImportance?)exp.Importance,
                    Rating = exp.Rating
                }).ToArray();

            return expenses;
        }

        public void UpdateExpenses(Guid userId, EncryptedList list)
        {
            foreach (var exp in list.List)
            {
                _db.UpdateExpenseByUser(exp.Id, exp.OpenText, exp.EncryptedText, userId);
            }
        }
    }
}
