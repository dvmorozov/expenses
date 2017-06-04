﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialApps.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ExpensesEntities : DbContext
    {
        public ExpensesEntities()
            : base("name=ExpensesEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<ExpensesLinks> ExpensesLinks { get; set; }
        public virtual DbSet<Links> Links { get; set; }
        public virtual DbSet<ExpensesCategories> ExpensesCategories { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<Expenses> Expenses { get; set; }
    
        public virtual int AddMonthIncomeByUser(Nullable<int> year, Nullable<int> month, Nullable<decimal> income, Nullable<System.Guid> dataOwner)
        {
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var incomeParameter = income.HasValue ?
                new ObjectParameter("Income", income) :
                new ObjectParameter("Income", typeof(decimal));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddMonthIncomeByUser", yearParameter, monthParameter, incomeParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> MonthIncomeByUser(Nullable<System.DateTime> today, Nullable<System.Guid> dataOwner)
        {
            var todayParameter = today.HasValue ?
                new ObjectParameter("Today", today) :
                new ObjectParameter("Today", typeof(System.DateTime));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("MonthIncomeByUser", todayParameter, dataOwnerParameter);
        }
    
        public virtual int ResetMonthIncomeByUser(Nullable<int> year, Nullable<int> month, Nullable<decimal> income, Nullable<System.Guid> dataOwner)
        {
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var incomeParameter = income.HasValue ?
                new ObjectParameter("Income", income) :
                new ObjectParameter("Income", typeof(decimal));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("ResetMonthIncomeByUser", yearParameter, monthParameter, incomeParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> AddCategoryByUser(string name, Nullable<double> limit, Nullable<System.Guid> dataOwner, string encryptedName)
        {
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var limitParameter = limit.HasValue ?
                new ObjectParameter("Limit", limit) :
                new ObjectParameter("Limit", typeof(double));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            var encryptedNameParameter = encryptedName != null ?
                new ObjectParameter("EncryptedName", encryptedName) :
                new ObjectParameter("EncryptedName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("AddCategoryByUser", nameParameter, limitParameter, dataOwnerParameter, encryptedNameParameter);
        }
    
        public virtual ObjectResult<ExpensesByCategoryForMonthByUser2_Result> ExpensesByCategoryForMonthByUser2(Nullable<int> categoryId, Nullable<int> year, Nullable<int> month, Nullable<System.Guid> dataOwner)
        {
            var categoryIdParameter = categoryId.HasValue ?
                new ObjectParameter("CategoryId", categoryId) :
                new ObjectParameter("CategoryId", typeof(int));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ExpensesByCategoryForMonthByUser2_Result>("ExpensesByCategoryForMonthByUser2", categoryIdParameter, yearParameter, monthParameter, dataOwnerParameter);
        }
    
        public virtual int UpdateExpenseByUser(Nullable<int> expenseID, string name, string encryptedName, Nullable<System.Guid> dataOwner)
        {
            var expenseIDParameter = expenseID.HasValue ?
                new ObjectParameter("ExpenseID", expenseID) :
                new ObjectParameter("ExpenseID", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var encryptedNameParameter = encryptedName != null ?
                new ObjectParameter("EncryptedName", encryptedName) :
                new ObjectParameter("EncryptedName", typeof(string));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateExpenseByUser", expenseIDParameter, nameParameter, encryptedNameParameter, dataOwnerParameter);
        }
    
        public virtual int UpdateCategoryByUser(Nullable<int> categoryID, string name, string encryptedName, Nullable<System.Guid> dataOwner)
        {
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var encryptedNameParameter = encryptedName != null ?
                new ObjectParameter("EncryptedName", encryptedName) :
                new ObjectParameter("EncryptedName", typeof(string));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateCategoryByUser", categoryIDParameter, nameParameter, encryptedNameParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<LastYearCategoryExpensesByMonthByUser_Result> LastYearCategoryExpensesByMonthByUser(Nullable<int> categoryID, Nullable<int> lastMonthNumber, Nullable<System.Guid> dataOwner)
        {
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            var lastMonthNumberParameter = lastMonthNumber.HasValue ?
                new ObjectParameter("LastMonthNumber", lastMonthNumber) :
                new ObjectParameter("LastMonthNumber", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LastYearCategoryExpensesByMonthByUser_Result>("LastYearCategoryExpensesByMonthByUser", categoryIDParameter, lastMonthNumberParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<LastYearTotalExpensesByMonthByUser_Result> LastYearTotalExpensesByMonthByUser(Nullable<int> lastMonthNumber, Nullable<System.Guid> dataOwner)
        {
            var lastMonthNumberParameter = lastMonthNumber.HasValue ?
                new ObjectParameter("LastMonthNumber", lastMonthNumber) :
                new ObjectParameter("LastMonthNumber", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LastYearTotalExpensesByMonthByUser_Result>("LastYearTotalExpensesByMonthByUser", lastMonthNumberParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<LastYearBalanceByMonthByUser_Result> LastYearBalanceByMonthByUser(Nullable<int> lastMonthNumber, Nullable<System.Guid> dataOwner)
        {
            var lastMonthNumberParameter = lastMonthNumber.HasValue ?
                new ObjectParameter("LastMonthNumber", lastMonthNumber) :
                new ObjectParameter("LastMonthNumber", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<LastYearBalanceByMonthByUser_Result>("LastYearBalanceByMonthByUser", lastMonthNumberParameter, dataOwnerParameter);
        }
    
        public virtual int AddMonthBudgetByUser2(Nullable<int> year, Nullable<int> month, Nullable<decimal> budget, Nullable<System.Guid> dataOwner, string currency)
        {
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var budgetParameter = budget.HasValue ?
                new ObjectParameter("Budget", budget) :
                new ObjectParameter("Budget", typeof(decimal));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            var currencyParameter = currency != null ?
                new ObjectParameter("Currency", currency) :
                new ObjectParameter("Currency", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddMonthBudgetByUser2", yearParameter, monthParameter, budgetParameter, dataOwnerParameter, currencyParameter);
        }
    
        public virtual ObjectResult<MonthBudgetByUser_Result> MonthBudgetByUser(Nullable<System.DateTime> today, Nullable<System.Guid> dataOwner)
        {
            var todayParameter = today.HasValue ?
                new ObjectParameter("Today", today) :
                new ObjectParameter("Today", typeof(System.DateTime));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<MonthBudgetByUser_Result>("MonthBudgetByUser", todayParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<EstimatedCategoriesByUser3_Result> EstimatedCategoriesByUser3(Nullable<int> year, Nullable<int> month, Nullable<int> day, Nullable<System.Guid> dataOwner, Nullable<bool> shortList)
        {
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var dayParameter = day.HasValue ?
                new ObjectParameter("Day", day) :
                new ObjectParameter("Day", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            var shortListParameter = shortList.HasValue ?
                new ObjectParameter("ShortList", shortList) :
                new ObjectParameter("ShortList", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<EstimatedCategoriesByUser3_Result>("EstimatedCategoriesByUser3", yearParameter, monthParameter, dayParameter, dataOwnerParameter, shortListParameter);
        }
    
        public virtual ObjectResult<GetExpenseNamesWithCategoryByUser5_Result> GetExpenseNamesWithCategoryByUser5(Nullable<int> categoryID, Nullable<System.Guid> dataOwner, Nullable<int> year, Nullable<int> month, Nullable<int> day, Nullable<bool> shortList)
        {
            var categoryIDParameter = categoryID.HasValue ?
                new ObjectParameter("CategoryID", categoryID) :
                new ObjectParameter("CategoryID", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var dayParameter = day.HasValue ?
                new ObjectParameter("Day", day) :
                new ObjectParameter("Day", typeof(int));
    
            var shortListParameter = shortList.HasValue ?
                new ObjectParameter("ShortList", shortList) :
                new ObjectParameter("ShortList", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetExpenseNamesWithCategoryByUser5_Result>("GetExpenseNamesWithCategoryByUser5", categoryIDParameter, dataOwnerParameter, yearParameter, monthParameter, dayParameter, shortListParameter);
        }
    
        public virtual ObjectResult<GetIncomeNamesByUser_Result> GetIncomeNamesByUser(Nullable<System.Guid> dataOwner, Nullable<int> year, Nullable<int> month, Nullable<int> day, Nullable<bool> shortList)
        {
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var dayParameter = day.HasValue ?
                new ObjectParameter("Day", day) :
                new ObjectParameter("Day", typeof(int));
    
            var shortListParameter = shortList.HasValue ?
                new ObjectParameter("ShortList", shortList) :
                new ObjectParameter("ShortList", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetIncomeNamesByUser_Result>("GetIncomeNamesByUser", dataOwnerParameter, yearParameter, monthParameter, dayParameter, shortListParameter);
        }
    
        public virtual int DeleteOperationByUser(Nullable<int> expenseId, Nullable<System.Guid> dataOwner)
        {
            var expenseIdParameter = expenseId.HasValue ?
                new ObjectParameter("ExpenseId", expenseId) :
                new ObjectParameter("ExpenseId", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteOperationByUser", expenseIdParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<TodayAndMonthTotalByUser2_Result> TodayAndMonthTotalByUser2(Nullable<System.DateTime> today, Nullable<System.Guid> dataOwner)
        {
            var todayParameter = today.HasValue ?
                new ObjectParameter("Today", today) :
                new ObjectParameter("Today", typeof(System.DateTime));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TodayAndMonthTotalByUser2_Result>("TodayAndMonthTotalByUser2", todayParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<EstimatedTop10CategoriesForMonthByUser3_Result> EstimatedTop10CategoriesForMonthByUser3(Nullable<int> year, Nullable<int> month, Nullable<int> day, Nullable<System.Guid> dataOwner)
        {
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            var dayParameter = day.HasValue ?
                new ObjectParameter("Day", day) :
                new ObjectParameter("Day", typeof(int));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<EstimatedTop10CategoriesForMonthByUser3_Result>("EstimatedTop10CategoriesForMonthByUser3", yearParameter, monthParameter, dayParameter, dataOwnerParameter);
        }
    
        public virtual ObjectResult<MonthTotalByUser3_Result> MonthTotalByUser3(Nullable<System.DateTime> today, Nullable<System.Guid> dataOwner)
        {
            var todayParameter = today.HasValue ?
                new ObjectParameter("Today", today) :
                new ObjectParameter("Today", typeof(System.DateTime));
    
            var dataOwnerParameter = dataOwner.HasValue ?
                new ObjectParameter("DataOwner", dataOwner) :
                new ObjectParameter("DataOwner", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<MonthTotalByUser3_Result>("MonthTotalByUser3", todayParameter, dataOwnerParameter);
        }
    }
}
