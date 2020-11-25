using SocialApps.Models;
using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Globalization;

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
            //  https://github.com/dvmorozov/expenses/issues/122
            _db.Database.CommandTimeout = 60;
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

        public static string GetLocalizedResourceString(string resourceName)
        {
            var resourceManager = Resources.Resources.ResourceManager;
            var userCulture = new CultureInfo(CultureInfo.CurrentUICulture.TextInfo.CultureName);
            return resourceManager.GetString(resourceName, userCulture);
        }

        //  Adds to each currency group special row containing residue not covered by any group (supplementing to total).
        //  https://github.com/dvmorozov/expenses/issues/21
        public List<EstimatedTop10CategoriesForMonthByUser3_Result> GetTop10CategoriesWithResidue(Guid userId, DateTime now)
        {
            var allItems = GetTop10Categories(userId, now);
            //  https://action.mindjet.com/task/14919145
            var monthTotalsWithCurrencies = GetMonthTotalsWithCurrencies(userId, now.Year, now.Month);

            var currencyGroups =
                (
                from i in allItems
                group i by new { Currency = GetCurrency(i) } into g
                select new CurrencySum
                {
                    Currency = g.Key.Currency,
                    Residue = (monthTotalsWithCurrencies.Where(t => GetCurrency(t) == GetCurrency(g)).First().Total ?? 0) - 
                        (g.Sum(t => t.TOTAL) ?? 0), 
                    Sum = g.Sum(t => t.TOTAL) ?? 0,
                    GROUPID1 = g.FirstOrDefault().GROUPID1 ?? 0,
                    //  This is used as index of row.
                    GROUPID2 = (g.Max(t => t.GROUPID2) ?? 0) + 1
                }
                );

            //  Add items representing residues.
            foreach (var g in currencyGroups)
            {
                if (g.Residue > 0)
                    allItems.Add(
                        new EstimatedTop10CategoriesForMonthByUser3_Result
                        {
                            Currency = g.Currency,
                            NAME = GetLocalizedResourceString("Residue"),
                            TOTAL = g.Residue,
                            GROUPID1 = g.GROUPID1,
                            GROUPID2 = g.GROUPID2,
                            ID = -1
                        }
                );
            }

            return allItems;
        }

        public List<LastYearTotalExpensesByMonthByUser> GetLastYearTotalExpensesByMonth(Guid userId, int lastMonthNumber)
        {
            //  https://github.com/dvmorozov/expenses/issues/23
            //  Original structure doesn't contain groud identifier i.e. should be extended.
            var extendedList = (
               from v1Item in _db.LastYearTotalExpensesByMonthByUser2(lastMonthNumber, userId).ToList()
               select new LastYearTotalExpensesByMonthByUser
               {
                    M = v1Item.M,
                    Y = v1Item.Y,
                    Total = v1Item.Total,
                    Currency = v1Item.Currency,
                    Month = v1Item.Month,
                    GROUPID1 = 0
               }).ToList();

            return CreateCurrencyGroupId(extendedList);
        }

        public List<LastYearBalanceByMonthByUser> GetLastYearBalanceByMonth(Guid userId, int lastMonthNumber)
        {
            //  https://github.com/dvmorozov/expenses/issues/23
            //  Original structure doesn't contain groud identifier i.e. should be extended.
            var extendedList = (
               from v1Item in _db.LastYearBalanceByMonthByUser2(lastMonthNumber, userId).ToList()
               select new LastYearBalanceByMonthByUser
               {
                   M = v1Item.M,
                   Y = v1Item.Y,
                   Balance = v1Item.Balance,
                   Currency = v1Item.Currency,
                   GROUPID1 = 0
               }).ToList();

            return CreateCurrencyGroupId(extendedList);
        }

        public List<LastYearCategoryExpensesByMonthByUser> GetLastYearCategoryExpensesByMonth(Guid userId, int categoryId, int lmn)
        {
            //  https://github.com/dvmorozov/expenses/issues/23
            //  Original structure doesn't contain groud identifier i.e. should be extended.
            var extendedList = (
               from v1Item in _db.LastYearCategoryExpensesByMonthByUser2(categoryId, lmn, userId).ToList()
                select new LastYearCategoryExpensesByMonthByUser
                {
                   M = v1Item.M,
                   Y = v1Item.Y,
                   Total = v1Item.Total,
                   Month = v1Item.Month,
                   Currency = v1Item.Currency,
                   GROUPID1 = 0
                }).ToList();

            return CreateCurrencyGroupId(extendedList);
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

        public static string GetCurrency<a, T>(IGrouping<a, T> o)
        {
            return ((string)o.First().GetType().GetProperty("Currency").GetValue(o.First()))?.Trim() ?? "";
        }

        public static string GetCurrency<T>(T o)
        {
            return ((string)o.GetType().GetProperty("Currency").GetValue(o))?.Trim() ?? "";
        }

        private List<T> CreateCurrencyGroupId<T>(List<T> query)
        {
            //  Selects different currencies.
            var currencies =
                (
                from g in query
                group g by new { Currency = GetCurrency(g) } into gc
                select new CurrencyGroup { Currency = GetCurrency(gc) }
                ).ToArray();

            //  Initializes group numbers.
            long i = 0;
            foreach (var c in currencies)
                c.GroupId = i++;

            //  Copies group identifier into resulting set.
            var result = query.ToList();
            foreach (var item in result)
            {
                foreach (var c in currencies)
                {
                    if (c.Currency == GetCurrency(item))
                    {
                        item.GetType().GetProperty("GROUPID1").SetValue(item, c.GroupId);
                        break;
                    }
                }
            }
            return result;
        }

        //  Returns groups for all currencies in sinle unsorted list.
        //  https://github.com/dvmorozov/expenses/issues/10
        public List<MonthImportance> GetMonthImportances(Guid userId, DateTime now)
        {
            var query =
                from sorted in
                (
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
                        Currency = g.FirstOrDefault().Currency != null ? g.FirstOrDefault().Currency.Trim() : ""
                    }
            )
            //  https://github.com/dvmorozov/expenses/issues/84
            orderby sorted.Importance descending
            select
            new MonthImportance
            {
                Sum = sorted.Sum,
                Importance = sorted.Importance,
                Currency = sorted.Currency
            };
            return CreateCurrencyGroupId(query.ToList());
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
