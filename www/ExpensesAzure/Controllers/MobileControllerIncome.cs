using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SocialApps.Models;
using System.Web;
using SocialApps.Repositories;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
        //  https://vision.mindjet.com/action/task/14485574
        public ActionResult AddIncome()
        {
            try
            {
                if (Session["IncomeId"] == null)
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectIncome", new { shortList = true });

                var incomeList = (ExpenseNameWithCategory[])Session["IncomeList"];
                var incomeId = (int)Session["IncomeId"];

                Operations income = null;
                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                if (incomeList.Count(t => t.Id == incomeId) != 0)
                {
                    income = _repository.GetIncome(incomeId);

                    //  https://www.evernote.com/shard/s132/nl/14501366/4d030991-c8d8-401a-a1a1-34ebe4b01a05 
                    if (income.Monthly ?? false)
                    {
                        return RedirectToAction("EditIncome", new { expenseId = income.ID });
                    }
                }

                var clientExpenseDate = Session["ClientIncomeDate"];

                FillExpenseLinks("AddIncome");

                return View("AddIncome",
                    new NewExpense
                    {
                        Day = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Day : -1),
                        Month = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Month : -1),
                        Year = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Year : -1),
                        Hour = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Hour : -1),
                        Min = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Minute : -1),
                        Sec = (clientExpenseDate != null ? ((DateTime)clientExpenseDate).Second : -1),
                        //  https://www.evernote.com/shard/s132/nl/14501366/4a2b6d8f-5d9c-4ad2-b1c2-7535341c98f4
                        ExpenseId = incomeId,
                        EncryptedName = income != null ? income.EncryptedName : null,
                        Name = income != null && income.Name != null ? income.Name.Trim() : null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/a499d49f-68c6-4370-941d-f4beb5c87c74
                        //  https://www.evernote.com/shard/s132/nl/14501366/a951297a-cff1-42d4-9e29-0a6654b8730c
                        Importance = income != null && income.Importance != null ? income.Importance : (short)MobileRepository.ExpenseImportance.Necessary,
                        Rating = income != null && income.Rating != null ? income.Rating : null,
                        Note = income != null && income.Note != null ? income.Note.Trim() : null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/333c0ad2-6962-4de1-93c1-591aa92bbcb3
                        //  https://www.evernote.com/shard/s132/nl/14501366/ac86d7cd-401e-4706-80f9-c8eeead71f35
                        Project = null,
                        //  https://www.evernote.com/shard/s132/nl/14501366/6b4be0d2-91ab-4213-b5ff-21057fe6462a
                        Currency = income != null && income.Currency != null ? income.Currency.Trim() : null,
                        Cost = income != null ? ((double)income.Cost).ToString(CultureInfo.InvariantCulture) : null
                    });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddIncome"));
            }
        }

        //  https://vision.mindjet.com/action/task/14485574
        [HttpPost]
        public ActionResult AddIncome(int day, int month, int year, int hour, int min, int sec, string cost, string currency, string note, string project)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                if (string.IsNullOrEmpty(cost))
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectIncome", new { shortList = true });

                double amount;
                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                cost = cost.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                cost = cost.Replace(" ", string.Empty);

                if (!double.TryParse(cost, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount))
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectIncome", new { shortList = true });

                if (Session["IncomeId"] == null)
                    //  https://action.mindjet.com/task/14479694
                    return RedirectToAction("SelectIncome", new { shortList = true });

                var expense = ((ExpenseNameWithCategory[])Session["IncomeList"]).First(t => t.Id == (int)Session["IncomeId"]);
                var clientIncomeDate = new DateTime(year, month, day, hour, min, sec, 0);

                Session["ClientIncomeDate"] = clientIncomeDate;

                _repository.AddExpense(clientIncomeDate, expense.Name, amount, note, false, null, null, expense.EncryptedName, currency, null, null, null, project, GetUserId(), true);

                DropSessionLinks();

                return RedirectToAction("MonthIncome");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "AddIncome"));
            }
        }

        //  https://vision.mindjet.com/action/task/14485574
        public ActionResult EditIncome(int expenseId)
        {
            try
            {
                Session["IncomeId"] = expenseId;
                var clientIncomeDate = Session["ClientIncomeDate"] != null ? (DateTime)Session["ClientIncomeDate"] : DateTime.Now;

                var expense = _repository.GetIncome(expenseId);

                //  https://www.evernote.com/shard/s132/nl/14501366/eb75b683-fead-4822-9d38-17e50ab7de2f
                FillExpenseLinks("EditIncome", new { expenseId = expenseId });

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

                return View("EditIncome",
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

                        Day = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Day : clientIncomeDate.Day,
                        Month = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Month : clientIncomeDate.Month,
                        Year = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Year : clientIncomeDate.Year,
                        Hour = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Hour : clientIncomeDate.Hour,
                        Min = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Minute : clientIncomeDate.Minute,
                        Sec = expense != null && expense.FirstMonth != null ? ((DateTime)expense.FirstMonth).Second : clientIncomeDate.Second,

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
                return View("Error", new HandleErrorInfo(e, "Mobile", "EditIncome"));
            }
        }

        [HttpPost]
        //  https://vision.mindjet.com/action/task/14485574
        public ActionResult EditIncome(int day, int month, int year, int hour,
            int min, int sec, int startYear, int startMonth, int endYear, int endMonth,
            bool? monthly, bool? forever, string cost, string name, string encryptedName,
            string currency, string note, string project)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                if (cost == null)
                    return RedirectToAction("EditIncome");

                double amount;
                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                cost = cost.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                cost = cost.Replace(" ", string.Empty);

                if (!double.TryParse(cost, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out amount))
                    return RedirectToAction("EditIncome");

                if (Session["IncomeId"] == null)
                    return RedirectToAction("SelectIncome");

                var expenseName = HttpUtility.HtmlDecode(name);
                var clientExpenseDate = new DateTime(year, month, day, hour, min, sec, 0);

                Session["ClientIncomeDate"] = clientExpenseDate;

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
                _repository.EditExpense((int)Session["IncomeId"], GetUserId(), clientExpenseDate, expenseName, amount, note, monthly, firstMonth, lastMonth, encryptedName, currency, null, null, project);

                //  Returns for adding another income.
                return RedirectToAction("MonthIncome");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "EditIncome"));
            }
        }

        //  https://vision.mindjet.com/action/task/14485574
        public ActionResult NewIncome()
        {
            try
            {
                var clientExpenseDate = Session["ClientIncomeDate"];

                FillExpenseLinks("NewIncome");

                return View("NewIncome",
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
                return View("Error", new HandleErrorInfo(e, "Mobile", "NewIncome"));
            }
        }

        [HttpPost]
        //  https://vision.mindjet.com/action/task/14485574
        public ActionResult NewIncome(int day, int month, int year, int hour, int min, int sec, int startYear, int startMonth, int endYear, int endMonth, bool? monthly, bool? forever, string cost, string name, string encryptedName, string currency, string note, string project)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/e6cd86bb-8a22-4111-8fbb-8610bb35c304
                //  https://www.evernote.com/shard/s132/nl/14501366/10e01660-9dd4-4f59-90d4-5f52009ffdb9
                if (string.IsNullOrEmpty(cost) || string.IsNullOrEmpty(name))
                    return RedirectToAction("NewIncome");

                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                cost = cost.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                cost = cost.Replace(" ", string.Empty);

                if (!double.TryParse(cost, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double amount))
                    return RedirectToAction("NewIncome");

                var expenseName = HttpUtility.HtmlDecode(name);
                var clientExpenseDate = new DateTime(year, month, day, hour, min, sec, 0);

                Session["ClientIncomeDate"] = clientExpenseDate;

                //  https://www.evernote.com/shard/s132/nl/14501366/67b5959f-63bc-4cd5-af1a-a481a2859c50
                DateTime? firstMonth = null;
                //  https://vision.mindjet.com/action/task/14485354
                if (startYear != -1 && startMonth != -1 && (monthly != null && (bool)monthly))
                    firstMonth = new DateTime(startYear, startMonth, 1);

                DateTime? lastMonth = null;
                //  https://vision.mindjet.com/action/task/14485354
                if (endYear != -1 && endMonth != -1 && (monthly != null && (bool)monthly) && !(forever != null && (bool)forever))
                    lastMonth = new DateTime(endYear, endMonth, 1);

                _repository.AddExpense(clientExpenseDate, name, amount, note, monthly, firstMonth, lastMonth, encryptedName, currency, null, null, null, project, GetUserId(), true);

                //  Returns for adding another income.
                return RedirectToAction("MonthIncome");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "NewIncome"));
            }
        }

        //  https://vision.mindjet.com/action/task/14485574
        public ActionResult SelectIncome(bool? shortList = false)
        {
            try
            {
                var userId = GetUserId();

                var start = DateTime.Now;

                //  https://action.mindjet.com/task/14479694
                var date = (Session["ClientIncomeDate"] != null ? (DateTime)Session["ClientIncomeDate"] : DateTime.Now);

                var expensesList = _repository.GetIncomeNames(userId, date, shortList);

                var end = DateTime.Now;
                var diff = end - start;
                var time = diff.Seconds * 1000 + diff.Milliseconds;

                //  https://www.evernote.com/shard/s132/nl/14501366/43810bf8-aeab-4801-af55-e61f344f548f
                ViewBag.ControlTime = time;
                //  https://action.mindjet.com/task/14479694
                ViewBag.ItemCount = expensesList.Count();

                if (Session["IncomeId"] != null)
                {
                    //  Selected item is moved to first position.
                    //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                    expensesList =
                        expensesList.Where(t => t.Id == (int)Session["IncomeId"])
                                    .Concat(expensesList.Where(t => t.Id != (int)Session["IncomeId"]))
                                    .ToArray();
                    //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                    if (expensesList.Count(t => t.Id == (int)Session["IncomeId"]) != 0)
                        ViewBag.ExpenseName = expensesList.First(t => t.Id == (int)Session["IncomeId"]).Name.Trim();
                }

                ViewBag.ExpenseIds = expensesList;
                ViewBag.ExpenseId = Session["IncomeId"];
                Session["IncomeList"] = expensesList;

                //  https://action.mindjet.com/task/14479694
                if (shortList != null && (bool)shortList)
                {
                    ViewBag.ShortView = true;
                    ViewBag.FullListMethod = "SelectIncome";
                }
                return View("SelectIncome");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectIncome"));
            }
        }

        //  https://action.mindjet.com/task/14485586
        public ActionResult SelectIncomeF(int id)
        {
            try
            {
                Session["IncomeId"] = id;
                return RedirectToAction("AddIncome");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "SelectIncomeF"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/e9be060a-5343-47e7-9441-65cbb5c80f60
        public ActionResult MonthIncome(int? year, int? month)
        {
            try
            {
                var date = (month != null && year != null) ? new DateTime((int)year, (int)month, 1) : DateTime.Now;
                var userId = GetUserId();

                var expensesList = _repository.GetIncomesForMonth(userId, date);
                //  The day is not relevant.
                Session["Day"] = date.Day;
                Session["Month"] = date.Month;
                Session["Year"] = date.Year;

                //  Used in ExpenseAccordion. List expected!
                ViewBag.TodayExpenses = expensesList;
                ViewBag.ShowOnlyMonth = true;

                //  https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
                var totals = _repository.GetMonthBudget(userId, date);
                if (totals != null)
                    ViewBag.MonthTotal = totals.MonthTotal;

                var income = _repository.GetMonthIncome(userId, date);
                if (income != null)
                {
                    //  https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
                    ViewBag.Balance = totals != null && totals.MonthTotal != null ? income - totals.MonthTotal : income;
                    return View("MonthIncome", new IncomeModel { Month = date.Month, Year = date.Year, Income = Math.Floor((decimal)income).ToString(), Reset = 0 });
                }
                else
                {
                    //  https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
                    ViewBag.Balance = totals != null && totals.MonthTotal != null ? -totals.MonthTotal : 0;
                    return View("MonthIncome", new IncomeModel { Month = date.Month, Year = date.Year, Income = "N/A", Reset = 0 });
                }
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "MonthIncome"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/e9be060a-5343-47e7-9441-65cbb5c80f60
        [HttpPost]
        public ActionResult MonthIncome(int? year, int? month, string income, int? reset)
        {
            try
            {
                if (month == null || year == null)
                {
                    var dt = DateTime.Now;
                    month = dt.Month;
                    year = dt.Year;
                }

                if (income == null)
                    return RedirectToAction("MonthIncome");

                //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                //  Allows both comma and point as decimal separator.
                var b = income.Replace(',', '.');
                //  https://www.evernote.com/shard/s132/nl/14501366/5926d2b0-49b8-4aef-8fb9-1a8e0de14da6
                b = b.Replace(" ", string.Empty);
                decimal dIncome;

                if (!decimal.TryParse(b, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dIncome))
                    return RedirectToAction("MonthIncome");

                _repository.AddMonthIncome(GetUserId(), (int)year, (int)month, dIncome, reset);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "MonthIncome"));
            }
        }
    }
}
