using Newtonsoft.Json;
using SocialApps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            DeleteBlobs(userId, SelectExpensesPrefix + "_" + catId);
            _session[SelectExpensesPrefix] = null;
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
                        Name = (name == null || name.Trim() == "") ?
                               (!string.IsNullOrEmpty(encryptedName) ? "password required" : null) : name,
                        Cost = amount,
                        Note = (note == null || note.Trim() == "") ? null : note,
                        DataOwner = userId,
                        Monthly = monthly,
                        FirstMonth = firstMonth,
                        LastMonth = lastMonth,
                        EncryptedName = encryptedName,
                        Currency = (currency == null || currency.Trim() == "") ? null : currency,
                        Rating = rating,
                        Importance = importance,
                        Project = (project == null || project.Trim() == "") ? null : project,
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
            DeleteCachedExpenses(userId, _db.ExpensesCategories.First(t => t.ExpenseID == expenseId).CategoryID);

            _db.DeleteOperationByUser(expenseId, userId);
        }

        public void EditExpense(int expenseId, Guid userId, DateTime clientExpenseDate, string expenseName, double amount, string note, bool? monthly, 
            DateTime? firstMonth, DateTime? lastMonth, string encryptedName, string currency, short? rating, short? importance, string project)
        {
            //  Removes all possibly existing old files.
            DeleteCachedExpenses(userId, _db.ExpensesCategories.First(t => t.ExpenseID == expenseId).CategoryID);

            var expense = _db.Operations.First(t => t.ID == expenseId && t.DataOwner == userId);
            expense.Date = clientExpenseDate;
            expense.Name = expenseName;
            expense.Cost = amount;
            expense.Note = note;
            expense.Monthly = monthly;
            expense.FirstMonth = firstMonth;
            expense.LastMonth = lastMonth;
            expense.EncryptedName = encryptedName == string.Empty ? null : encryptedName;
            expense.Currency = currency;
            expense.Rating = rating;
            expense.Importance = importance;
            //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
            expense.Project = project;
            _db.SaveChanges();

            //  https://www.evernote.com/shard/s132/nl/14501366/eb75b683-fead-4822-9d38-17e50ab7de2f
            SaveExpenseLinks(expenseId);
        }
    }
}
