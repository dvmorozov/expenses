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

        //  https://action.mindjet.com/task/14919145
        public MonthTotalByUser3_Result[] GetMonthTotalsWithCurrencies(Guid userId, int year, int month)
        {
            return _db.MonthTotalByUser3(new DateTime(year, month, 1), userId).ToArray();
        }

        public List<EstimatedTop10CategoriesForMonthByUser3_Result> GetTop10Categories(Guid userId, DateTime now)
        {
            return _db.EstimatedTop10CategoriesForMonthByUser3(now.Year, now.Month, now.Day, userId).ToList();
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
                       ((exp.Date.Year == date.Year && exp.Date.Month == date.Month) ||
                       ((exp.Monthly != null ? (bool)exp.Monthly : false) && date >= exp.FirstMonth && (exp.LastMonth != null ? date <= exp.LastMonth : true)))
                 group exp by exp.Currency into g
                 select new MonthCurrency
                 {
                    Sum = g.Sum(t => t.Cost != null ? (double)t.Cost : 0.0),
                    Currency = g.FirstOrDefault().Currency
                 }).ToList();

            var expenses =
                (from exp in _db.Expenses
                 where (exp.DataOwner == userId) &&
                       ((exp.Date.Year == date.Year && exp.Date.Month == date.Month) ||
                       ((exp.Monthly != null ? (bool)exp.Monthly : false) && date >= exp.FirstMonth && (exp.LastMonth != null ? date <= exp.LastMonth : true)))
                 group exp by exp.Currency into g
                 select new MonthCurrency
                 {
                     Sum = g.Sum(t => t.Cost != null ? (double)t.Cost : 0.0),
                     Currency = g.FirstOrDefault().Currency
                 }).ToList();

            //  Left outer join is implemented.
            //  https://github.com/dvmorozov/expenses/issues/5
            var result = 
                (from income in incomes
                join expense in expenses on income.Currency equals expense.Currency into g
                from item in g.DefaultIfEmpty()
                select new MonthBalance
                {
                    Currency = income.Currency,
                    SumExpenses = item?.Sum ?? 0,
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
            var dateLast = date.AddMonths(-1);
            var expenses =
               (from exp in _db.Operations
                where (exp.DataOwner == userId) && (exp.Income != null) && ((bool)exp.Income) && 
                (
                      // Unrepeatable income.
                      ((exp.Monthly == null || !(bool)exp.Monthly) &&
                      exp.Date.Month == date.Month && exp.Date.Year == date.Year)) ||
                      // https://github.com/dvmorozov/expenses/issues/3
                      // Repeatable income.
                      (((exp.Monthly != null && (bool)exp.Monthly) &&
                          exp.Date.Day == date.Day &&
                          //  FirstMonth, LastMonth are optional.
                          //  FirstMonth, LastMonth are defined as date of the first day.
                          (exp.FirstMonth == null || date >= (DateTime)exp.FirstMonth) &&
                          //  Compare with the first day of next month.
                          (exp.LastMonth == null || dateLast < (DateTime)exp.LastMonth))
                )
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

        //  Returns groups for all currencies in sinle unsorted list.
        //  https://github.com/dvmorozov/expenses/issues/10
        public List<MonthImportance> GetMonthImportances(Guid userId, DateTime now)
        {
            return (
                from monthImportance in
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
                        Importance = exp.Importance != null ? (short)exp.Importance : (short)ExpenseImportance.Necessary,
                        Currency = exp.Currency
                    }
                )
                group monthImportance by new { monthImportance.Currency, monthImportance.Importance } into g
                select new MonthImportance
                {
                    Sum = g.Sum(t => t.Sum),
                    Importance = g.FirstOrDefault().Importance,
                    Currency = g.FirstOrDefault().Currency
                }
            ).ToList();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
