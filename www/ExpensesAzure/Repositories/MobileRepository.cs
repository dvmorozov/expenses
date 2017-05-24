using SocialApps.Models;
using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;

namespace SocialApps.Repositories
{
    //  https://www.evernote.com/shard/s132/nl/14501366/a499d49f-68c6-4370-941d-f4beb5c87c74
    //  Is used in representations. Must be defined here.
    public enum ExpenseImportance
    {
        Liability = 5,
        Asset = 4,
        Necessary = 3,
        Pleasure = 2,
        Unnecessary = 1
    }

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

        public MonthBudgetByUser_Result GetMonthBudget(Guid userId, DateTime date)
        {
            return _db.MonthBudgetByUser(date, userId).FirstOrDefault();
        }

        public MonthBalance[] GetMonthBalances(Guid userId, DateTime date)
        {
            var incomes =
                (from exp in _db.Operations
                 where (exp.DataOwner == userId) && (exp.Income != null) && ((bool)exp.Income) &&
                       (exp.Date.Year == date.Year) && (exp.Date.Month == date.Month)
                 group exp by exp.Currency into g
                 select new MonthCurrency
                 {
                    Sum = g.Sum(t => t.Cost != null ? (double)t.Cost : 0.0),
                    Currency = g.FirstOrDefault().Currency
                 }).ToList();

            var expenses =
                (from exp in _db.Expenses
                 where (exp.DataOwner == userId) && (exp.Date.Year == date.Year) && (exp.Date.Month == date.Month)
                 group exp by exp.Currency into g
                 select new MonthCurrency
                 {
                     Sum = g.Sum(t => t.Cost != null ? (double)t.Cost : 0.0),
                     Currency = g.FirstOrDefault().Currency
                 }).ToList();

            var result = 
                (from income in incomes
                join expense in expenses on income.Currency equals expense.Currency
                select new MonthBalance
                {
                    Currency = income.Currency,
                    SumExpenses = expense.Sum,
                    SumIncomes = income.Sum
                }).ToArray();

            return result;
        }

        public decimal? GetMonthIncome(Guid userId, DateTime date)
        {
            return _db.MonthIncomeByUser(date, userId).FirstOrDefault();
        }

        public List<TodayExpense> GetIncomesForMonth(Guid userId, DateTime date)
        {
            //  https://vision.mindjet.com/action/task/14485587
            //  https://action.mindjet.com/task/14665340
            //  https://action.mindjet.com/task/14915101
            var expenses =
               (from exp in _db.Operations
                where (exp.DataOwner == userId) && (exp.Income != null) && ((bool)exp.Income) && 
                      (exp.Date.Year == date.Year) && (exp.Date.Month == date.Month)
                orderby exp.Currency
                select new TodayExpense
                {
                    Name = exp.Name,
                    Cost = exp.Cost,
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
