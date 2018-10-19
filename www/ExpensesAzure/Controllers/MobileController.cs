using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SocialApps.Models;
using SocialApps.Repositories;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using System.Web.Routing;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
        //  https://action.mindjet.com/task/14509395
        private MobileRepository _repository;

        //  Session object isn't initialized at the time of controller construction.
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _repository = new MobileRepository(requestContext.HttpContext.Session);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _repository.Dispose();
            base.Dispose(disposing);
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/c707248c-3cab-47d7-838a-ec2b791e4ea7
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/14e369f7-348f-4f68-aa65-6a5e7dda1da7
                var date = DateTime.Now;
                ViewBag.Month = date.Month;
                ViewBag.Year = date.Year;

                //  https://www.evernote.com/shard/s132/nl/14501366/c707248c-3cab-47d7-838a-ec2b791e4ea7
                if (User.IsInRole("User") || User.IsInRole("Administrator"))
                {
                    var totals = _repository.GetMonthBudget(GetUserId(), date);
                    if (totals != null)
                    {
                        ViewBag.MonthBudget = totals.MonthBudget;
                        ViewBag.MonthTotal = totals.MonthTotal;
                        ViewBag.Estimation = totals.Estimation;
                    }
                }

                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Index"));
            }
        }

        //  Perform consistent storing of totals data.
        //  A bit of refactoring.
        //  https://action.mindjet.com/task/14672437
        private void PutTotalsIntoSession(TodayAndMonthTotalByUser2_Result totals)
        {
            if (totals != null)
            {
                Session["TodayTotal"] = totals.TodayTotal;
                Session["TodayCurrency"] = totals.TodayCurrency;
                Session["MonthTotal"] = totals.MonthTotal;
                Session["MonthCurrency"] = totals.MonthCurrency;
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/3db6842f-dd5c-49e0-8536-e637ea009cd5
        public ActionResult DayExpenseTotals(int? day, int? month, int? year)
        {
            try
            {
                var date = (day != null && month != null && year != null) ?
                    new DateTime((int)year, (int)month, (int)day) :
                    (Session["ClientExpenseDate"] != null ? (DateTime)Session["ClientExpenseDate"] : DateTime.Now);

                var userId = GetUserId();
                //  For this application it's more convenient to order the list in reverse cost order.
                var expenseList = _repository.GetDayExpenseTotals(userId, date);
                ViewBag.TodayExpenses = expenseList;

                //  https://www.evernote.com/shard/s132/nl/14501366/cadee374-b60a-451f-bed5-d9237644dac3
                Session["ClientExpenseDate"] = date;

                //  https://www.evernote.com/shard/s132/nl/14501366/b1a0ac6d-6a39-42c0-9d99-8647d7572bc2
                Session["Day"] = date.Day;
                Session["Month"] = date.Month;
                Session["Year"] = date.Year;

                //  https://action.mindjet.com/task/14672437
                PutTotalsIntoSession(_repository.GetTodayAndMonthTotals(userId, date));

                var pairs = new List<EncryptedPair>();

                //  https://www.evernote.com/shard/s132/nl/14501366/d03bc138-ab63-470b-8b99-df02ec42f205
                for (var i = 0; i < expenseList.Count(); i++)
                {
                    var expense = expenseList[i];
                    var expenseId = "ExpenseName" + i.ToString();
                    var categoryId = "CategoryName" + i.ToString();
                    if (expense.ExpenseEncryptedName != null && expense.ExpenseEncryptedName != string.Empty)
                    {
                        pairs.Add(new EncryptedPair{Id = expenseId, EncryptedName = expense.ExpenseEncryptedName});
                    }
                    if (expense.CategoryEncryptedName != null && expense.CategoryEncryptedName != string.Empty)
                    {
                        pairs.Add(new EncryptedPair{Id = categoryId, EncryptedName = expense.CategoryEncryptedName});
                    }
                }

                ViewBag.Pairs = JsonConvert.SerializeObject(new EncryptedPairs { encryptedPairs = pairs.ToArray() });
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "DayExpenseTotals"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/89c5640f-d270-462e-a494-4db32a6c8c01
        public ActionResult DayExpenses(int? day, int? month, int? year)
        {
            try
            {
                var date = (day != null && month != null && year != null) ?
                    new DateTime((int)year, (int)month, (int)day) :
                    (Session["ClientExpenseDate"] != null ? (DateTime)Session["ClientExpenseDate"] : DateTime.Now);

                var userId = GetUserId();

                var todayExpenses = _repository.GetDayExpenses(userId, date);
                ViewBag.TodayExpenses = todayExpenses;

                //  https://www.evernote.com/shard/s132/nl/14501366/cadee374-b60a-451f-bed5-d9237644dac3
                Session["ClientExpenseDate"] = date;

                //  https://www.evernote.com/shard/s132/nl/14501366/b1a0ac6d-6a39-42c0-9d99-8647d7572bc2
                Session["Day"] = date.Day;
                Session["Month"] = date.Month;
                Session["Year"] = date.Year;

                //  https://action.mindjet.com/task/14672437
                PutTotalsIntoSession(_repository.GetTodayAndMonthTotals(userId, date));

                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "DayExpenses"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/14e369f7-348f-4f68-aa65-6a5e7dda1da7
        public ActionResult MonthBudget(int? year, int? month)
        {
            try
            {
                var date = (month != null && year != null) ? new DateTime((int)year, (int)month, 1) : DateTime.Now;

                var budget = _repository.GetMonthBudget(GetUserId(), date);
                if (budget != null)
                {
                    var t = budget.MonthTotal ?? 0;
                    var b = budget.MonthBudget ?? 0;

                    ViewBag.Rest = b > t ? b - t : 0;
                    ViewBag.RestPercent = b != 0 ? (decimal)ViewBag.Rest * (decimal)100.0 / b : 100;
                    ViewBag.RestPercent = b != 0 ? (decimal)ViewBag.Rest * (decimal)100.0 / b : 100;

                    ViewBag.Overdraft = t > b ? t - b : 0;
                    ViewBag.OverdraftPercent = b != 0 ? (decimal)ViewBag.Overdraft * (decimal)100.0 / b : 100;

                    return View("MonthBudget", new BudgetModel { Month = date.Month, Year = date.Year, Budget = (budget.MonthBudget != null ? budget.MonthBudget.ToString() : "N/A") });
                }
                else
                {
                    return View("MonthBudget", new BudgetModel { Month = date.Month, Year = date.Year, Budget = "N/A" });
                }
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "MonthBudget"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/14e369f7-348f-4f68-aa65-6a5e7dda1da7
        [HttpPost]
        public ActionResult MonthBudget(int? year, int? month, string budget, string currency)
        {
            try
            {
                if (month == null || year == null) {
                    var dt = DateTime.Now;
                    month = dt.Month;
                    year = dt.Year;
                }

                if (budget == null)
                    return RedirectToAction("MonthBudget");

                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                var b = budget.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                b = b.Replace(" ", string.Empty);
                decimal dBudget;

                if (!decimal.TryParse(b, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dBudget))
                    return RedirectToAction("MonthBudget");

                _repository.AddMonthBudget((int)year, (int)month, dBudget, GetUserId(), currency);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "MonthBudget"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/d10dab75-37a4-4ead-a53c-f00988d12474
        [AllowAnonymous]
        public ActionResult About()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "About"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/d10dab75-37a4-4ead-a53c-f00988d12474
        [AllowAnonymous]
        public ActionResult Contribute()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Contribute"));
            }
        }

        //  https://github.com/dvmorozov/expenses/issues/86
        public static string ImportanceToString(short i)
        {
            var result = Resources.Resources.Necessary;
            switch ((ExpenseImportance)i)
            {
            case ExpenseImportance.Liability:
                result = Resources.Resources.Liability;
                break;

            case ExpenseImportance.Asset:
                result = Resources.Resources.Asset;
                break;

            case ExpenseImportance.Necessary:
                result = Resources.Resources.Necessary;
                break;

            case ExpenseImportance.Pleasure:
                result = Resources.Resources.Pleasure;
                break;

            case ExpenseImportance.Unnecessary:
                result = Resources.Resources.Unnecessary;
                break;
            }
            return result;
        }

        //  https://action.mindjet.com/task/14672437
        public static string TotalsToString(int? day, int? month, int? year, double? dayTotal, double? monthTotal, string todayCurrency, string monthCurrency)
        {
            var date = (day != null && month != null && year != null ? new DateTime((int)year, (int)month, (int)day) : DateTime.Now);
            return date.ToString("dd MMM | ") + 
                Math.Truncate(dayTotal ?? 0) + (todayCurrency != "" ? " " + todayCurrency : "") + " | " + 
                Math.Truncate(monthTotal ?? 0) + (monthCurrency != "" ? " " + monthCurrency : "");
        }

        public static string TotalsToString(int? day, int? month, int? year, double? dayTotal, double? monthTotal)
        {
            return TotalsToString(day, month, year, dayTotal, monthTotal, "", "");
        }

        /// <summary>
        /// Takes totals data from session and convert them into string.
        /// Simplifies maintenance of views. Shoud be static to guarantee availability from any view.
        /// </summary>
        //  https://action.mindjet.com/task/14672437
        public static string TotalsToStringSession(ControllerBase context)
        {
            if (context as Controller != null)
            {
                var session = (context as Controller).Session;

                return TotalsToString(
                    (int?)session["Day"], (int?)session["Month"], (int?)session["Year"], 
                    (double?)session["TodayTotal"], (double?)session["MonthTotal"], 
                    //  Nulls are converted to strings automatically.
                    (string)session["TodayCurrency"], (string)session["MonthCurrency"]);
            }
            else
                return "";
        }

        //  https://action.mindjet.com/task/14672437
        public static string TotalsToStringWithCurrency()
        {
            //int? day, int? month, int? year, double? dayTotal, double? monthTotal;
            return "";
        }

        public static string MonthYearToString(int month, int year)
        {
            var date = new DateTime(year, month, 1);
            return date.ToString("MMM yyyy");
        }

        public ActionResult SelectCategory()
        {
            try
            {
                //  Sets name of action which will be performed after category selection.
                Session["SelectCategoryResult"] = "SelectExpense";
                //  https://action.mindjet.com/task/14479694
                //  Redirect to the category selection page.
                return RedirectToAction("SelectCategoryS", new { shortList = true });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectCategory"));
            }
        }

        private DateTime GetBusinessDate() {
            //  https://www.evernote.com/shard/s132/nl/14501366/b89158d1-e84f-4d3b-8cbe-fd8a6ea28c95
            var date = (Session["ClientExpenseDate"] != null ? (DateTime)Session["ClientExpenseDate"] : DateTime.Now);

            //  https://www.evernote.com/shard/s132/nl/14501366/ea14a9a9-1feb-4a14-97df-b75e497623f3
            Session["Day"] = date.Day;
            Session["Month"] = date.Month;
            Session["Year"] = date.Year;

            return date;
        }

        //  https://action.mindjet.com/task/14479694
        //  Renders category selection page.
        public ActionResult SelectCategoryS(bool? shortList = false)
        {
            try
            {
                var start = DateTime.Now;
                var userId = GetUserId();
                var date = GetBusinessDate();

                //  https://action.mindjet.com/task/14509395
                var categoryList = _repository.SelectEstimatedCategories(date, userId, shortList);

                var end = DateTime.Now;
                var diff = end - start;
                var time = diff.Seconds * 1000 + diff.Milliseconds;

                //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
                ViewBag.ControlTime = time;
                //  https://action.mindjet.com/task/14479694
                ViewBag.ItemCount = categoryList.Count();

                //  https://action.mindjet.com/task/14672437
                PutTotalsIntoSession(_repository.GetTodayAndMonthTotals(userId, date));
            
                //  Category name is used to form the quick link.
                if (Session["CategoryId"] != null)
                {
                    //  Selected item is moved to first position.
                    //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                    var categoryId = (int) Session["CategoryId"];
                    categoryList =
                        categoryList.Where(t => t.ID == categoryId)
                                    .Concat(categoryList.Where(t => t.ID != categoryId))
                                    .ToArray();
                    //  Checks if the category exists.
                    //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                    if (categoryList.Count(t => t.ID == categoryId) != 0)
                        ViewBag.CategoryName = categoryList.First(t => t.ID == categoryId).NAME.Trim();
                }

                ViewBag.CategoryIds = categoryList;
                ViewBag.CategoryId = Session["CategoryId"];

                if (shortList != null && (bool)shortList)
                {
                    ViewBag.ShortView = true;
                    ViewBag.FullListMethod = "SelectCategoryS";
                }
                return View("SelectCategory");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectCategoryS"));
            }
        }

        //  Handles category selection.
        public ActionResult SelectCategoryF(int categoryIds)
        {
            try
            {
                if (!_repository.CategoriesLoaded())
                    return RedirectToAction("SelectCategoryS", new { shortList = true });

                if (Session["CategoryId"] == null || (int)Session["CategoryId"] != categoryIds)
                    //  Resets income id.
                    Session["ExpenseId"] = null;
                //  Saves id for subsequent use.
                Session["CategoryId"] = categoryIds;
                //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
                //  It's used for the page "trend by category". 
                //  Saves data of the category.
                //  https://www.evernote.com/shard/s132/nl/14501366/4a2b6d8f-5d9c-4ad2-b1c2-7535341c98f4
                Session["SelectedCategory"] = _repository.GetCategories().First(t => t.ID == categoryIds);

                //  https://action.mindjet.com/task/14479694
                var returnAction = (string)(Session["SelectCategoryResult"] ?? "SelectExpense");
                if (returnAction == "SelectExpense")
                    return RedirectToAction(returnAction, new { shortList = true });
                else
                    return RedirectToAction(returnAction);
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectCategoryF"));
            }
        }

        /*
        private class Expense
        {
            public string name;
            public long id;
        }
        */

        //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
        public JsonResult ExpenseList()
        {
            var result = new ExpenseListReturn();
            try
            {
                if (Session["CategoryId"] == null)
                    throw new Exception("Category hasn't selected.");

                var cat = (int)Session["CategoryId"];

                //  https://action.mindjet.com/task/14479694
                var date = (Session["ClientExpenseDate"] != null ? (DateTime)Session["ClientExpenseDate"] : DateTime.Now);

                //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
                result.ExpenseList = _repository.GetExpenseNamesWithCategory(GetUserId(), date, cat);
                result.Success = true;
                result.Message = "Ok.";
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //  https://action.mindjet.com/task/14479694
        public ActionResult SelectExpense(bool? shortList = false)
        {
            try
            {
                if (Session["CategoryId"] == null)
                    return RedirectToAction("SelectCategory");

                var cat = (int) Session["CategoryId"];
                var user = GetUserId();

                var start = DateTime.Now;

                //  https://action.mindjet.com/task/14479694
                var date = (Session["ClientExpenseDate"] != null ? (DateTime)Session["ClientExpenseDate"] : DateTime.Now);

                //  https://action.mindjet.com/task/14509395
                var expensesList = _repository.SelectExpenses(cat, user, date, shortList);

                var end = DateTime.Now;
                var diff = end - start;
                var time = diff.Seconds * 1000 + diff.Milliseconds;

                //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
                ViewBag.ControlTime = time;
                //  https://action.mindjet.com/task/14479694
                ViewBag.ItemCount = expensesList.Count();

                if (Session["ExpenseId"] != null)
                {
                    //  Selected item is moved to first position.
                    //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                    expensesList =
                        expensesList.Where(t => t.Id == (int)Session["ExpenseId"])
                                    .Concat(expensesList.Where(t => t.Id != (int) Session["ExpenseId"]))
                                    .ToArray();
                    //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                    if (expensesList.Count(t => t.Id == (int)Session["ExpenseId"]) != 0)
                        ViewBag.ExpenseName = expensesList.First(t => t.Id == (int)Session["ExpenseId"]).Name.Trim();
                }

                ViewBag.ExpenseIds = expensesList;
                ViewBag.ExpenseId = Session["ExpenseId"];
                Session["GetExpenseNamesResult"] = expensesList;

                //  https://action.mindjet.com/task/14479694
                if (shortList != null && (bool)shortList)
                {
                    ViewBag.ShortView = true;
                    ViewBag.FullListMethod = "SelectExpense";
                }
                return View("SelectExpense");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectExpense"));
            }
        }

        public ActionResult SelectExpenseF(int expenseId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Session["ExpenseId"] = expenseId;
                    return RedirectToAction("AddExpense");
                }
                return RedirectToAction("SelectCategory");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectExpenseF"));
            }
        }

        public ActionResult AddExpense()
        {
            try
            {
                if (Session["CategoryId"] == null)
                    return RedirectToAction("SelectCategory");

                if (Session["ExpenseId"] == null)
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                var expensesList = (ExpenseNameWithCategory[])Session["GetExpenseNamesResult"];
                var expenseId = (int)Session["ExpenseId"];

                Expenses expense = null;
                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                if (expensesList.Count(t => t.Id == expenseId) != 0)
                {
                    expense = _repository.GetExpense(GetUserId(), expenseId);

                    //  https://www.evernote.com/shard/s132/nl/14501366/4d030991-c8d8-401a-a1a1-34ebe4b01a05 
                    if (expense.Monthly ?? false)
                    {
                        return RedirectToAction("EditExpense", new { expenseId = expense.ID });
                    }
                }

                var clientExpenseDate = Session["ClientExpenseDate"];

                FillExpenseLinks("AddExpense");

                return View(
#if EXPENSES
                    "AddExpense"
#elif FITNESS
                    "AddExpenseFitness"
#endif
                    , 
                    new NewExpense {
                        Day = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Day : -1),
                        Month = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Month : -1),
                        Year = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Year : -1),
                        Hour = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Hour : -1),
                        Min = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Minute : -1),
                        Sec = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Second : -1),
                        //  https://www.evernote.com/shard/s132/nl/14501366/4a2b6d8f-5d9c-4ad2-b1c2-7535341c98f4
                        ExpenseId = expenseId,
                        EncryptedName = expense != null ? expense.EncryptedName : null,
                        Name = expense != null && expense.Name != null ? expense.Name.Trim() : null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/a499d49f-68c6-4370-941d-f4beb5c87c74
                        //  https://www.evernote.com/shard/s132/nl/14501366/a951297a-cff1-42d4-9e29-0a6654b8730c
                        Importance = expense != null && expense.Importance != null ? expense.Importance : (short)ExpenseImportance.Necessary,
                        Rating = expense != null && expense.Rating != null ? expense.Rating : null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
                        //  https://www.evernote.com/shard/s132/nl/14501366/ac86d7cd-401e-4706-80f9-c8eeead71f35
                        Project = null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/6b4be0d2-91ab-4213-b5ff-21057fe6462a
                        Currency = expense != null && expense.Currency != null ? expense.Currency.Trim() : null,
                        Cost = expense != null ? ((double)expense.Cost).ToString(CultureInfo.InvariantCulture) : null
                    });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddExpense"));
            }
        }

        // https://github.com/dvmorozov/expenses/issues/70
        public ActionResult AddRestOfReceipt()
        {
            try
            {
                if (Session["CategoryId"] == null)
                    return RedirectToAction("SelectCategory");

                if (Session["ExpenseId"] == null)
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                var expensesList = (ExpenseNameWithCategory[])Session["GetExpenseNamesResult"];
                var expenseId = (int)Session["ExpenseId"];

                Expenses expense = null;
                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                if (expensesList.Count(t => t.Id == expenseId) != 0)
                {
                    expense = _repository.GetExpense(GetUserId(), expenseId);

                    //  https://www.evernote.com/shard/s132/nl/14501366/4d030991-c8d8-401a-a1a1-34ebe4b01a05 
                    if (expense.Monthly ?? false)
                    {
                        return RedirectToAction("EditExpense", new { expenseId = expense.ID });
                    }
                }

                var clientExpenseDate = Session["ClientExpenseDate"];

                FillExpenseLinks("AddExpense");

                return View(
                    "AddRestOfReceipt"
                    ,
                    new NewExpense
                    {
                        Day = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Day : -1),
                        Month = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Month : -1),
                        Year = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Year : -1),
                        Hour = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Hour : -1),
                        Min = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Minute : -1),
                        Sec = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Second : -1),
                        //  https://www.evernote.com/shard/s132/nl/14501366/4a2b6d8f-5d9c-4ad2-b1c2-7535341c98f4
                        ExpenseId = expenseId,
                        EncryptedName = expense?.EncryptedName,
                        Name = expense != null && expense.Name != null ? expense.Name.Trim() : null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/a499d49f-68c6-4370-941d-f4beb5c87c74
                        //  https://www.evernote.com/shard/s132/nl/14501366/a951297a-cff1-42d4-9e29-0a6654b8730c
                        Importance = expense != null && expense.Importance != null ? expense.Importance : (short)ExpenseImportance.Necessary,
                        Rating = expense != null && expense.Rating != null ? expense.Rating : null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
                        //  https://www.evernote.com/shard/s132/nl/14501366/ac86d7cd-401e-4706-80f9-c8eeead71f35
                        Project = null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/6b4be0d2-91ab-4213-b5ff-21057fe6462a
                        Currency = expense != null && expense.Currency != null ? expense.Currency.Trim() : null,
                        Cost = expense != null ? ((double)expense.Cost).ToString(CultureInfo.InvariantCulture) : null
                    });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddExpense"));
            }
        }

        [HttpPost]
        public ActionResult AddExpense(int day, int month, int year, int hour, int min, int sec, string cost, string currency, string note, short? rating, short? importance, string project, int multiplier)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                if (string.IsNullOrEmpty(cost))
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                cost = cost.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                cost = cost.Replace(" ", string.Empty);

                if (!double.TryParse(cost, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double amount))
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                if (Session["CategoryId"] == null)
                    return RedirectToAction("SelectCategory");

                if (Session["ExpenseId"] == null)
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                var expense = ((ExpenseNameWithCategory[])Session["GetExpenseNamesResult"]).First(t => t.Id == (int)Session["ExpenseId"]);
                var clientExpenseDate = new DateTime(year, month, day, hour, min, sec, 0);
                var categoryId = (int)Session["CategoryId"];
                var userId = GetUserId();

                Session["ClientExpenseDate"] = clientExpenseDate;

                //  https://github.com/dvmorozov/expenses/issues/101
                if (multiplier > 0)
                    amount = amount * multiplier;

                _repository.AddExpense(clientExpenseDate, expense.Name, amount, note, false, null, null, 
                    expense.EncryptedName, currency, rating, categoryId, importance, project, userId);

                DropSessionLinks();
                
                //  Returns for adding another income.
                return RedirectToAction("DayExpenseTotals");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddExpense"));
            }
        }

        [HttpPost]
        //  https://github.com/dvmorozov/expenses/issues/70
        public ActionResult AddRestOfReceipt(int day, int month, int year, int hour, int min, int sec, string cost, string currency, string note, short? rating, short? importance, string project, int multiplier)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                if (string.IsNullOrEmpty(cost))
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                cost = cost.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                cost = cost.Replace(" ", string.Empty);

                if (!double.TryParse(cost, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double amount))
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                if (Session["CategoryId"] == null)
                    return RedirectToAction("SelectCategory");

                if (Session["ExpenseId"] == null)
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });

                var expense = ((ExpenseNameWithCategory[])Session["GetExpenseNamesResult"]).First(t => t.Id == (int)Session["ExpenseId"]);
                var clientExpenseDate = new DateTime(year, month, day, hour, min, sec, 0);
                var categoryId = (int)Session["CategoryId"];
                var userId = GetUserId();

                Session["ClientExpenseDate"] = clientExpenseDate;

                //  https://github.com/dvmorozov/expenses/issues/101
                if (multiplier > 0)
                    amount = amount * multiplier;

                _repository.AddExpense(clientExpenseDate, expense.Name, amount, note, false, null, null,
                    expense.EncryptedName, currency, rating, categoryId, importance, project, userId);

                DropSessionLinks();

                //  Returns for adding another income.
                return RedirectToAction("DayExpenseTotals");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddExpense"));
            }
        }

        public ActionResult DeleteExpense(int expenseId)
        {
            try
            {
                var income = _repository.IsIncome(expenseId);

                //  https://action.mindjet.com/task/14509395
                _repository.DeleteExpense(expenseId, GetUserId());

                var date = DateTime.Now;
                if (Session["Day"] != null && Session["Month"] != null && Session["Year"] != null)
                {
                    var day = (int)Session["Day"];
                    var month = (int)Session["Month"];
                    var year = (int)Session["Year"];
                    date = new DateTime(year, month, day);
                }

                if (income != null && (bool)income)
                {
                    return RedirectToAction("MonthIncome", new { year = date.Year, month = date.Month });
                }
                else
                    return RedirectToAction("DayExpenses", new { date.Day, date.Month, date.Year });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "DeleteExpense"));
            }
        }

        public ActionResult NewCategory()
        {
            try
            {
                return View(new CategoryModel());
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddExpense"));
            }
        }

        [HttpPost]
        public ActionResult NewCategory(CategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    double? limit = null;
                    //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                    if (model.Limit != null)
                    {
                        model.Limit = model.Limit.Replace(',', '.');
                        //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                        model.Limit = model.Limit.Replace(" ", string.Empty);


                        if (!double.TryParse(model.Limit, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double l))
                            return RedirectToAction("NewCategory");
                        limit = l;
                    }

                    if (model.Name.Trim() == string.Empty)
                        return RedirectToAction("NewCategory");

                    Session["CategoryId"] = _repository.NewCategory(model.Name, limit, GetUserId(), model.EncryptedName != string.Empty ? model.EncryptedName : null);

                    DropSessionLinks();

                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectExpense", new { shortList = true });
                }
                return RedirectToAction("NewCategory");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "NewCategory"));
            }
        }

        public ActionResult EditCategory(int categoryId)
        {
            try
            {
                return View(_repository.GetCategory(categoryId));
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddExpense"));
            }
        }

        [HttpPost]
        public ActionResult EditCategory(CategoryModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    float? limit = null;

                    //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                    if (model.Limit != null)
                    {
                        model.Limit = model.Limit.Replace(',', '.');
                        //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                        model.Limit = model.Limit.Replace(" ", string.Empty);


                        if (!float.TryParse(model.Limit, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float l))
                            return RedirectToAction("EditCategory", model);

                        limit = (l == 0.0) ? (float?)null : l;
                    }

                    _repository.EditCategory(model.Id, model.Name, GetUserId(), limit, model.EncryptedName);
                    return RedirectToAction("SelectCategory");
                }
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "NewCategory"));
            }
        }

        public ActionResult NewExpense()
        {
            try
            {
                if (Session["CategoryId"] == null)
                    return RedirectToAction("SelectCategory");

                var clientExpenseDate = Session["ClientExpenseDate"];

                FillExpenseLinks("NewExpense");

                return View(
#if EXPENSES
                    "NewExpense"
#elif FITNESS
                    "NewExpenseFitness"
#endif
                    ,
                    new NewExpense
                    {
                        Day = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Day : -1),
                        Month = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Month : -1),
                        Year = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Year : -1),
                        Hour = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Hour : -1),
                        Min = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Minute : -1),
                        Sec = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Second : -1)
                    });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "NewExpense"));
            }
        }

        [HttpPost]
        public ActionResult NewExpense(int day, int month, int year, int hour, int min, int sec, int startYear, int startMonth, int endYear, int endMonth, bool? monthly, bool? forever, string cost, string name, string encryptedName, string currency, string note, short? rating, short? importance, string project)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                //  https://www.evernote.com/shard/s132/nl/14501366/10e01660-9dd4-4f59-90d4-5f52009ffdb9
                if (string.IsNullOrEmpty(cost) || string.IsNullOrEmpty(name))
                    return RedirectToAction("NewExpense");

                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                cost = cost.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                cost = cost.Replace(" ", string.Empty);

                if (!double.TryParse(cost, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double amount))
                    return RedirectToAction("NewExpense");

                if (Session["CategoryId"] == null)
                    return RedirectToAction("SelectCategory");

                var expenseName = HttpUtility.HtmlDecode(name);
                var clientExpenseDate = new DateTime(year, month, day, hour, min, sec, 0);
                var categoryId = (int)Session["CategoryId"];
                var userId = GetUserId();

                Session["ClientExpenseDate"] = clientExpenseDate;

                //  https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
                DateTime? firstMonth = null;
                //  https://vision.mindjet.com/action/task/14485354
                if (startYear != -1 && startMonth != -1 && (monthly != null && (bool)monthly))
                    firstMonth = new DateTime(startYear, startMonth, 1);

                DateTime? lastMonth = null;
                //  https://vision.mindjet.com/action/task/14485354
                if (endYear != -1 && endMonth != -1 && (monthly != null && (bool)monthly) && !(forever != null && (bool)forever))
                    lastMonth = new DateTime(endYear, endMonth, 1);

                _repository.NewExpense(clientExpenseDate, name, amount, note, monthly, firstMonth, lastMonth, encryptedName, currency, rating, categoryId, importance, project, userId);

                //  Returns for adding another income.
                return RedirectToAction("DayExpenseTotals");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "NewExpense"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
        public ActionResult EditExpense(int expenseId)
        {
            try
            {
                Session["ExpenseId"] = expenseId;
                
                var expense = _repository.GetIncome(expenseId);
                //  https://vision.mindjet.com/action/task/14485574
                if (expense.Income != null && (bool)expense.Income)
                    return RedirectToAction("EditIncome", new { expenseId = expenseId });

                var clientExpenseDate = Session["ClientExpenseDate"] != null ? (DateTime)Session["ClientExpenseDate"] : DateTime.Now;
                //  https://www.evernote.com/shard/s132/nl/14501366/eb75b683-fead-4822-9d38-17e50ab7de2f
                FillExpenseLinks("EditExpense", new { expenseId = expenseId });

                //  https://www.evernote.com/shard/s132/nl/14501366/d264af15-c0bf-4945-a331-86fcb467b020
                //  Documents added earlier.
                var expenseLinks = _repository.GetLinkedDocs(expenseId, GetUserId());
                if (expenseLinks != null)
                {
                    //  Documents added in this session.
                    var sessionLinks = (List<LinkModel>)ViewBag.Links;
                    if (sessionLinks != null)
                        sessionLinks.AddRange(expenseLinks);
                    else
                        ViewBag.Links = expenseLinks;
                }

                return View(
#if EXPENSES
                    "EditExpense"
#elif FITNESS
                    "EditExpenseFitness"
#endif
                    ,
                    new NewExpense
                    {
                        //  https://www.evernote.com/shard/s132/nl/14501366/a951297a-cff1-42d4-9e29-0a6654b8730c
                        ExpenseId = expenseId,
                        Name = expense != null && expense.Name != null ? expense.Name.Trim() : null,
                        Cost = expense != null && expense.Cost != null ? ((double)expense.Cost).ToString(CultureInfo.InvariantCulture) : string.Empty,
                        Currency = expense != null && expense.Currency != null ? expense.Currency.Trim() : null,
                        EncryptedName = expense != null ? expense.EncryptedName : null,
                        EndMonth = expense != null && expense.LastMonth != null ? ((DateTime)expense.LastMonth).Month : -1,
                        EndYear = expense != null && expense.LastMonth != null ? ((DateTime)expense.LastMonth).Year : -1,
                        Forever = expense != null && expense.LastMonth == null,
                        Monthly = expense != null && expense.Monthly != null ? (bool)expense.Monthly : false,
                        StartMonth = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Month : -1,
                        StartYear = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Year : -1,
                        
                        Day = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Day : clientExpenseDate.Day,
                        Month = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Month : clientExpenseDate.Month,
                        Year = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Year : clientExpenseDate.Year,
                        Hour = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Hour : clientExpenseDate.Hour,
                        Min = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Minute : clientExpenseDate.Minute,
                        Sec = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Second : clientExpenseDate.Second,

                        //  https://www.evernote.com/shard/s132/nl/14501366/a499d49f-68c6-4370-941d-f4beb5c87c74
                        Importance = expense != null ? expense.Importance : null,
                        Rating = expense != null ? expense.Rating : null,
                        Note = expense != null && expense.Note != null ? expense.Note.Trim() : null,

                        //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
                        Project = expense != null && expense.Project != null ? expense.Project.Trim() : null
                    });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "EditExpense"));
            }
        }
        
        [HttpPost]
        //  https://www.evernote.com/shard/s132/nl/14501366/adbb4c02-3975-460d-88f1-8a65312ca83f
        public ActionResult EditExpense(int day, int month, int year, int hour, 
            int min, int sec, int startYear, int startMonth, int endYear, int endMonth, 
            bool? monthly, bool? forever, string cost, string name, string encryptedName,
            string currency, short? rating, short? importance, string note, string project)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                if (cost == null)
                    return RedirectToAction("EditExpense");

                double amount;
                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                cost = cost.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                cost = cost.Replace(" ", string.Empty);

                if (!double.TryParse(cost, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount))
                    return RedirectToAction("EditExpense");

                if (Session["ExpenseId"] == null)
                    return RedirectToAction("SelectCategory");

                var expenseName = HttpUtility.HtmlDecode(name);
                var clientExpenseDate = new DateTime(year, month, day, hour, min, sec, 0);

                Session["ClientExpenseDate"] = clientExpenseDate;

                //  https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
                DateTime? firstMonth = null;
                //  https://vision.mindjet.com/action/task/14485354
                if (startYear != -1 && startMonth != -1 && (monthly != null && (bool)monthly))
                    firstMonth = new DateTime(startYear, startMonth, 1);

                DateTime? lastMonth = null;
                //  https://vision.mindjet.com/action/task/14485354
                if (endYear != -1 && endMonth != -1 && (monthly != null && (bool)monthly) && !(forever != null && (bool)forever))
                    lastMonth = new DateTime(endYear, endMonth, 1);

                //  https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
                //  https://action.mindjet.com/task/14509395
                _repository.EditExpense((int)Session["ExpenseId"], GetUserId(), clientExpenseDate, expenseName, amount, note, monthly, firstMonth, lastMonth, encryptedName, currency, rating, importance, project);
                //  Returns for adding another income.
                return RedirectToAction("DayExpenses");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "NewExpense"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/751d5935-68c5-42be-8f12-c5ab2315da02
        public ActionResult ExpensesByCategory(int categoryId, string currency)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/990cdfaa-11f8-4d6c-b83a-c139f78bdc53
                var date = (Session["Top10Month"] != null && Session["Top10Year"] != null) ? 
                    new DateTime((int)Session["Top10Year"],  (int)Session["Top10Month"], 1) : DateTime.Now;

                var expensesList = categoryId != -1 ? 
                    //  Gets expenses for given category.
                    _repository.GetExpensesByCategory(GetUserId(), date, categoryId, currency) :
                    //  https://github.com/dvmorozov/expenses/issues/22
                    //  Gets all other expenses except those covered by "top 10" categories.
                    _repository.GetExpensesExceptCategory(GetUserId(), date, 
                        ((List<EstimatedTop10CategoriesForMonthByUser3_Result>)Session["Top10CategoriesResult"]).Select(t => t.ID).ToArray(), currency);

                ViewBag.Year = date.Year;
                ViewBag.Month = date.Month;
                ViewBag.ExpenseIds = expensesList;
                //  https://www.evernote.com/shard/s132/nl/14501366/626c4826-6474-433a-aa7e-1f626f2f29d0
                ViewBag.ExpenseTotal = expensesList.Sum(t => t.Cost);
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "ExpensesByCategory"));
            }
        }

        //  https://action.mindjet.com/task/14896530
        public ActionResult ExpensesByName(int expenseId)
        {
            try
            {
                ViewBag.ExpenseIds = _repository.GetExpensesByName(GetUserId(), expenseId); ;
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "ExpensesByName"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
        public ActionResult Encryption()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Encryption"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/eac2dc0a-f87d-4a6d-9fd6-3329b2f11a71
        [HttpPost]
        //  https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
        public ActionResult UpdateExpenses(string expenseList)
        {
            try
            {
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(expenseList));
                var expenses = JsonConvert.DeserializeObject<MobileRepository.EncryptedList>(json);
                _repository.UpdateExpenses(GetUserId(), expenses);
                return null;
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "UpdateExpenses"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/eac2dc0a-f87d-4a6d-9fd6-3329b2f11a71
        [HttpPost]
        //  https://www.evernote.com/shard/s132/nl/14501366/5ea53405-2fc4-4166-a9e3-e918f3583785
        public ActionResult UpdateCategories(string categoryList)
        {
            try
            {
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(categoryList));
                var categories = JsonConvert.DeserializeObject<MobileRepository.EncryptedList>(json);
                _repository.UpdateCategories(GetUserId(), categories);
                //  https://www.evernote.com/shard/s132/nl/14501366/eac2dc0a-f87d-4a6d-9fd6-3329b2f11a71
                return null;    //  This will be converted to EmptyResult.
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "UpdateCategories"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/5b6f473a-b5ec-4a62-adf2-17362aea5d81
        public ActionResult SelectCurrency()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectCurrency"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/83a03e66-6551-43c0-816e-2b32be9640df
        public ActionResult Error()
        {
            try
            {
                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
