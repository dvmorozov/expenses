using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Diagnostics;
using SocialApps.Models;
using SocialApps.Repositories;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
        private static int _seqNum;

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        //  Renders and returns chart image.
        public FileResult GetTop10ChartContentWh(int currencyGroupId, int width, int height, bool? pie)
        {
            try
            {
                //  Gets chart object.
                var myChart = RenderTop10Chart(currencyGroupId, width, height, pie);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://action.mindjet.com/task/14919145
        private CurrencyGroup[] GetCurrencyGroups(List<EstimatedTop10CategoriesForMonthByUser3_Result> allItems)
        {
            return allItems.GroupBy(t => new { GroupId = (int)t.GROUPID1 })
                .Select(t => new CurrencyGroup { GroupId = t.Key.GroupId, Currency = t.First().Currency.Trim() }).ToArray();
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        private Chart RenderTop10Chart(int currencyGroupId, int width, int height, bool? pie)
        {
            var allItems = (List<EstimatedTop10CategoriesForMonthByUser3_Result>)Session["Top10CategoriesResult"];

            var year = (int)Session["Top10Year"];
            var month = (int)Session["Top10Month"];

            //  Group ids are extracted.
            var groupIds = GetCurrencyGroups(allItems);
            Debug.Assert(groupIds.Count() >= 1);

            //  Select items of given currency group.
            var items = allItems.Where(t => (int)t.GROUPID1 == currencyGroupId);

            var dt = new DateTime(year, month, 1);
            //  https://www.evernote.com/shard/s132/nl/14501366/e0eb1c4e-4561-4da4-ae7c-5c26648ec6fc
            //  Chart header was hidden.
            var chart = new Chart(width, height, theme: ChartTheme.Vanilla);
            //  Proper ordering is already done in the stored procedure.
            //var xValue = items.Select(t => t.NAME).ToList();
            var yValues = items.Select(t => t.TOTAL).ToList();

            //  Attempt to create different colors. It works but looks poorly.
            //  Saved for possible subsequent improvement.
            /*
            var positions = new int[items.Count()];

            for (var j = 0; j < positions.Count(); j++)
                positions[j] = j + 1;

            var legendName = Guid.NewGuid().ToString();
            chart.AddLegend("Legend", legendName);
            
            for (var index = 0; index < xValue.Count(); index++)
            {
                var name = xValue[index];
                //var total = yValues[index] ?? 0.0;

                var curValues = new List<double?>(yValues);
                for (var j = 0; j < curValues.Count; j++)
                {
                    if (j != index)
                        curValues[j] = 0.0;
                }

                chart.AddSeries(name, "Column", markerStep: xValue.Count(), legend: legendName,
                        //  Must be string to provide desired chart rendering.
                        //xValue: new List<string>{index.ToString(CultureInfo.InvariantCulture)}, xField: "Position",
                        //xValue: xValue, xField: "Name",
                        xValue: positions, xField: "Positions",
                        //yValues: new List<double> { total }, yFields: "Total"
                        yValues: curValues, yFields: "Total"
                        );
            }
            */

            if ((pie ?? false) && Session["MonthTotal"] != null)
            {
                var positions = new List<string>();
                //  Total value for selected "top 10" categories.
                var top10Total = 0.0;

                //  https://www.evernote.com/shard/s132/nl/14501366/a632edc9-5b3d-4f06-90e1-1e32683bc071
                for (var j = 0; j < yValues.Count(); j++)
                {
                    positions.Add((j + 1).ToString());
                    top10Total += (double)yValues[j];
                }

                var monthTotalsWithCurrencies = (MonthTotalByUser3_Result[])Session["MonthTotalsWithCurrencies"];
                var groupCurrency = groupIds.Where(t => t.GroupId == currencyGroupId).First().Currency.Trim();
                //  Select month total for given currency.
                var monthTotal = (double)monthTotalsWithCurrencies.Where(t => t.Currency.Trim() == groupCurrency).First().Total;
                //  Check that the residue is positive.
                var residue = Math.Floor(monthTotal) - Math.Floor(top10Total);
                Debug.Assert(residue >= 0);
                if (residue > 0)
                {
                    //  https://www.evernote.com/shard/s132/nl/14501366/41e0b392-d4cb-4843-bf6d-2dea63b9c42f
                    //  https://action.mindjet.com/task/15016557
                    //  Looks better without word.
                    positions.Add(/*"Others"*/"");
                    //  Add complementary value by subtracting total of "top 10" for given currency.
                    yValues.Add(monthTotal - top10Total);
                    Debug.Assert(positions.Count() == yValues.Count());
                }
                chart.AddSeries("Top 10 cat.", "Pie",   // markerStep: 1, 
                                                        //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                                                        //  This gives more convenient chart representation.
                    xValue: positions, xField: "Position",
                    yValues: yValues, yFields: "Total"
                    );
            }
            else
            {
                var positions = new int[items.Count()];

                for (var j = 0; j < positions.Count(); j++)
                    positions[j] = j + 1;

                Debug.Assert(positions.Count() == yValues.Count());

                chart.AddSeries("Top 10 cat.", "Column",  // markerStep: 1, 
                    //  Must be string to provide desired chart rendering.
                    //xValue: new List<string>{index.ToString(CultureInfo.InvariantCulture)}, xField: "Position",
                    //xValue: xValue, xField: "Name",
                    //  https://www.evernote.com/shard/s132/nl/14501366/9f1ae7a1-a257-4f6b-9af0-292da085ec15
                    //  This gives more convenient chart representation.
                    xValue: positions, xField: "Positions",
                    yValues: yValues, yFields: "Total"
                    );
            }
            return chart;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        //  Renders and returns chart image.
        public FileResult GetTrendChartContentWh(int width, int height)
        {
            try
            {
                var res = (List<LastYearTotalExpensesByMonthByUser_Result>)Session["LastYearTotalExpensesResult"];
                //  Gets chart object.
                var myChart = RenderTrendChart(res, width, height, (int)Session["LastMonthNumber"]);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        private static Chart RenderTrendChart(List<LastYearTotalExpensesByMonthByUser_Result> items, int width, int height, int lastMonthNumber)
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/e0eb1c4e-4561-4da4-ae7c-5c26648ec6fc
            //  Chart header was hidden.
            var chart = new Chart(width, height, theme: ChartTheme.Vanilla);
            //  Proper ordering is already done in the stored procedure.
            //  https://www.evernote.com/shard/s132/nl/14501366/fc3cc68c-0d2e-47d6-87a5-7ebdef9e818a
            var xValue = items.Select(t => ((int)t.Y).ToString() + "/" + ((int)t.M).ToString("D2")).ToList();
            var yValues = items.Select(t => t.Total).ToList();

            /*
            var toAdd = 5 - lastMonthNumber % 5;
            for (var i = 0; i < toAdd; i++)
            {
                xValue.Add("");
                yValues.Add(0.0);
            }
            */

            chart.AddSeries("Totals", lastMonthNumber > 24 ? "Area" : "Column",
                    //  Must be string to provide desired chart rendering.
                    //xValue: new List<string>{index.ToString(CultureInfo.InvariantCulture)}, xField: "Position",
                    xValue: xValue, xField: "Month",
                    yValues: yValues, yFields: "Total"
                    );
            return chart;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        //  Renders and returns chart image.
        public FileResult GetTrendByCategoryChartContentWh(int width, int height)
        {
            try
            {
                var res = (List<LastYearCategoryExpensesByMonthByUser_Result>)Session["LastYearByCategoryExpensesResult"];
                //  Gets chart object.
                var myChart = RenderTrendByCategoryChart(res, width, height, (int)Session["LastMonthNumber"], ((EstimatedCategoriesByUser3_Result)Session["SelectedCategory"]).NAME);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        private static Chart RenderTrendByCategoryChart(List<LastYearCategoryExpensesByMonthByUser_Result> items, int width, int height, int lastMonthNumber, string category)
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/e0eb1c4e-4561-4da4-ae7c-5c26648ec6fc
            //  Chart header was hidden.
            var chart = new Chart(width, height, theme: ChartTheme.Vanilla);
            //  Proper ordering is already done in the stored procedure.
            //  https://www.evernote.com/shard/s132/nl/14501366/fc3cc68c-0d2e-47d6-87a5-7ebdef9e818a
            var xValue = items.Select(t => ((int)t.Y).ToString() + "/" + ((int)t.M).ToString("D2")).ToList();
            var yValues = items.Select(t => t.Total).ToList();

            chart.AddSeries("Totals", lastMonthNumber > 24 ? "Area" : "Column",
                //  Must be string to provide desired chart rendering.
                //xValue: new List<string>{index.ToString(CultureInfo.InvariantCulture)}, xField: "Position",
                xValue: xValue, xField: "Month",
                yValues: yValues, yFields: "Total"
                );
            return chart;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        public ActionResult Top10(int? year, int? month)
        {
            try
            {
                var userId = GetUserId();
                var now = (month != null && year != null) ? new DateTime((int)year, (int)month, 1) : DateTime.Now;
                Session["Top10Month"] = now.Month;
                Session["Top10Year"] = now.Year;
                //  Caching repository data.
                var allItems = _repository.GetTop10Categories(userId, now);
                Session["Top10CategoriesResult"] = allItems;

                //  https://action.mindjet.com/task/14919145
                Session["MonthTotalsWithCurrencies"] = _repository.GetMonthTotalsWithCurrencies(GetUserId(), (int)Session["Top10Year"], (int)Session["Top10Month"]);

                ViewBag.CurrencyGroups = GetCurrencyGroups(allItems);
                //  This is used to show total in the top-right corner of form.
                var totals = _repository.GetTodayAndMonthTotals(userId, now);
                //  https://action.mindjet.com/task/14672437
                PutTotalsIntoSession(totals);

                if (totals != null)
                    ViewBag.MonthTotal = totals.MonthTotal;

                return View("Top10");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Top10"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/6ad181b9-a410-4aab-b47a-7ea111aefb04
        //  Renders and returns chart image.
        public FileResult GetImportanceChartContentWh(int width, int height, bool? pie)
        {
            try
            {
                var res = (List<MonthImportance>)Session["ImportanceResult"];
                //  Gets chart object.
                var myChart = RenderImportanceChart(res, width, height, (int)Session["Top10Year"], (int)Session["Top10Month"], pie);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/6ad181b9-a410-4aab-b47a-7ea111aefb04
        private Chart RenderImportanceChart(List<MonthImportance> items, int width, int height, int year, int month, bool? pie)
        {
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
                Session["ImportanceResult"] = _repository.GetMonthImportances(userId, now);

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
                return View("Error", new HandleErrorInfo(e, "Mobile", "Top10"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        public ActionResult Trend(int lastMonthNumber)
        {
            try
            {
                Session["LastYearTotalExpensesResult"] = _repository.GetLastYearTotalExpensesByMonth(GetUserId(), lastMonthNumber);
                Session["LastMonthNumber"] = lastMonthNumber;
                return View("Trend");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Trend"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        public ActionResult TrendByCategory(int lastMonthNumber)
        {
            try
            {
                Session["SelectCategoryResult"] = "TrendByCategoryF";
                Session["LastMonthNumber"] = lastMonthNumber;
                //  https://action.mindjet.com/task/14479694
                return RedirectToAction("SelectCategoryS", new { shortList = true });
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "TrendByCategory"));
            }

        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        //  Finalizes the process of category selection and proceeds with rendering of diagram.
        //  Used also for subsequent rendering of the diagram for other time intervals.
        public ActionResult TrendByCategoryF(int? lastMonthNumber)
        {
            try
            {
                var lmn = lastMonthNumber ?? Session["LastMonthNumber"];
                if (lmn == null)
                    return RedirectToAction("TrendByCategory", new {lastMonthNumber = (object) 12});

                if (lastMonthNumber != null)
                    //  Resets the month number in subsequent requests.
                    Session["LastMonthNumber"] = (int) lastMonthNumber;

                var categoryId = Session["CategoryId"];
                if (categoryId == null)
                    return RedirectToAction("TrendByCategory", new {lastMonthNumber = lmn});

                Session["LastYearByCategoryExpensesResult"] = _repository.GetLastYearCategoryExpensesByMonth(GetUserId(), (int)categoryId, (int)lmn);
                
                return View("TrendByCategory");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "TrendByCategoryF"));
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def 
        //  Renders and returns chart image.
        public FileResult GetBalanceChartContentWh(int width, int height)
        {
            try
            {
                var res = (List<LastYearBalanceByMonthByUser_Result>)Session["LastYearBalanceResult"];
                //  Gets chart object.
                var myChart = RenderBalanceChart(res, width, height, (int)Session["LastMonthNumber"]);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
        private static Chart RenderBalanceChart(List<LastYearBalanceByMonthByUser_Result> items, int width, int height, int lastMonthNumber)
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/e0eb1c4e-4561-4da4-ae7c-5c26648ec6fc
            //  Chart header was hidden.
            var chart = new Chart(width, height, theme: ChartTheme.Vanilla);
            //  Proper ordering is already done in the stored procedure.
            //  https://www.evernote.com/shard/s132/nl/14501366/fc3cc68c-0d2e-47d6-87a5-7ebdef9e818a
            var xValue = items.Select(t => ((int)t.Y).ToString() + "/" + ((int)t.M).ToString("D2")).ToList();
            var yValues = items.Select(t => t.Balance).ToList();

            chart.AddSeries("Balance", lastMonthNumber > 24 ? "Line" : "Column",
                //  Must be string to provide desired chart rendering.
                //xValue: new List<string>{index.ToString(CultureInfo.InvariantCulture)}, xField: "Position",
                    xValue: xValue, xField: "Month",
                    yValues: yValues, yFields: "Balance"
                    );
            return chart;
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
        public ActionResult Balance(int lastMonthNumber)
        {
            try
            {
                Session["LastYearBalanceResult"] = _repository.GetLastYearBalanceByMonth(GetUserId(), lastMonthNumber);
                Session["LastMonthNumber"] = lastMonthNumber;
                return View("Balance");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Balance"));
            }
        }
    }
}
