using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using SocialApps.Models;
using SocialApps.Repositories;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
        //  https://www.evernote.com/shard/s132/nl/14501366/6ad181b9-a410-4aab-b47a-7ea111aefb04
        //  Renders and returns chart image.
        public FileResult GetImportanceChartContentWh(int currencyGroupId, int width, int height, bool? pie)
        {
            try
            {
                //  Gets chart object.
                var myChart = RenderImportanceChart(currencyGroupId, width, height, (int)Session["Top10Year"], (int)Session["Top10Month"], pie);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/6ad181b9-a410-4aab-b47a-7ea111aefb04
        private Chart RenderImportanceChart(int currencyGroupId, int width, int height, int year, int month, bool? pie)
        {
            var allItems = (List<MonthImportance>)Session["ImportanceResult"];
            var items = FilterItemsByGroupId(allItems, currencyGroupId);

            var dt = new DateTime(year, month, 1);
            //  https://www.evernote.com/shard/s132/nl/14501366/e0eb1c4e-4561-4da4-ae7c-5c26648ec6fc
            //  Chart header was hidden.
            var chart = new Chart(width, height, theme: ChartTheme.Vanilla);
            //  Proper ordering is already done in the controller.
            var xValue = items.Select(t => ((ExpenseImportance)t.Importance).ToString()).ToList();
            var yValues = items.Select(t => t.Sum).ToList();

            if ((pie ?? false) && Session["MonthTotal"] != null)
            {
                chart.AddSeries("Importance", "Pie",  // markerStep: 1, 
                                                      //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                                                      //  This gives more convenient chart representation.
                    xValue: xValue, xField: "Importance",
                    yValues: yValues, yFields: "Total"
                    );
                //chart.AddLegend("Importance", "");
            }
            else
            {
                chart.AddSeries("Importance", "Column",  // markerStep: 1, 
                                                         //  Must be string to provide desired chart rendering.
                                                         //xValue: new List<string>{index.ToString(CultureInfo.InvariantCulture)}, xField: "Position",
                                                         //xValue: xValue, xField: "Name",
                                                         //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                                                         //  This gives more convenient chart representation.
                    xValue: xValue, xField: "Importance",
                    yValues: yValues, yFields: "Total"
                    );
            }

            return chart;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/6ad181b9-a410-4aab-b47a-7ea111aefb04
        public ActionResult Importance(int? year, int? month)
        {
            try
            {
                var now = (month != null && year != null) ? new DateTime((int)year, (int)month, 1) : DateTime.Now;
                var userId = GetUserId();

                Session["Top10Month"] = now.Month;
                Session["Top10Year"] = now.Year;

                var allItems = _repository.GetMonthImportances(userId, now);
                Session["ImportanceResult"] = allItems;

                ViewBag.CurrencyGroups = GetCurrencyGroups(allItems);

                var totals = _repository.GetTodayAndMonthTotals(userId, now);
                //  https://action.mindjet.com/task/14672437
                PutTotalsIntoSession(totals);

                if (totals != null)
                    ViewBag.MonthTotal = totals.MonthTotal;

                return View("Importance");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Importance"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/a6faca57-602d-44d8-ba4a-94ce5054a642
        public ActionResult ExpensesByImportance(int importance)
        {
            try
            {
                //  https://www.evernote.com/shard/s132/nl/14501366/990cdfaa-11f8-4d6c-b83a-c139f78bdc53
                var date = (Session["Top10Month"] != null && Session["Top10Year"] != null) ? new DateTime((int)Session["Top10Year"], (int)Session["Top10Month"], 1) : DateTime.Now;

                var expensesList = _repository.GetExpensesByImportance(GetUserId(), date, importance);
                ViewBag.Year = date.Year;
                ViewBag.Month = date.Month;
                ViewBag.ExpenseIds = expensesList;
                //  https://www.evernote.com/shard/s132/nl/14501366/626c4826-6474-433a-aa7e-1f626f2f29d0
                ViewBag.ExpenseTotal = expensesList.Sum(t => t.Cost);
                return View("ExpensesByCategory");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "ExpensesByCategory"));
            }
        }
    }
}