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
        private static int _seqNum;

        public ActionResult Reports()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Reports"));
            }
        }

        //  https://action.mindjet.com/task/14919145
        private CurrencyGroup[] GetCurrencyGroups<T>(IEnumerable<T> allItems)
        {
#if DEBUG
            foreach (var i in allItems)
                Debug.Assert(i.GetType().GetProperty("GROUPID1").GetValue(i) != null);
#endif
            //  Uses reflection to get data from objects of template parameter type.
            return allItems.GroupBy(item => new
            {
                GroupId = (long)item.GetType().GetProperty("GROUPID1").GetValue(item)
            })
            .Select(group => new CurrencyGroup
            {
                GroupId = group.Key.GroupId,
                //  https://github.com/dvmorozov/expenses/issues/10
                //  Selects the first item from each group.
                Currency = MobileRepository.GetCurrency(group)
            }).ToArray();
        }

        //  https://github.com/dvmorozov/expenses/issues/10
        private IEnumerable<T> FilterItemsByGroupId<T>(IEnumerable<T> allItems, int groupId)
        {
            //  Select items of given currency group.
            return allItems.Where(t => (long)t.GetType().GetProperty("GROUPID1").GetValue(t) == groupId);
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        //  Renders and returns chart image.
        public FileResult GetTrendChartContentWh(int currencyGroupId, int width, int height)
        {
            try
            {
                //  Gets chart object.
                var myChart = RenderTrendChart(currencyGroupId, width, height, (int)Session["LastMonthNumber"]);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        private Chart RenderTrendChart(int currencyGroupId, int width, int height, int lastMonthNumber)
        {
            var allItems = (List<LastYearTotalExpensesByMonthByUser>)Session["LastYearTotalExpensesResult"];
            var items = FilterItemsByGroupId(allItems, currencyGroupId);

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
        //  Renders and returns chart image.
        public FileResult GetTrendByCategoryChartContentWh(int width, int height)
        {
            try
            {
                var res = (List<LastYearCategoryExpensesByMonthByUser>)Session["LastYearByCategoryExpensesResult"];
                //  Gets chart object.
                var myChart = RenderTrendByCategoryChart(res, width, height, (int)Session["LastMonthNumber"], ((EstimatedCategoriesByUser4_Result)Session["SelectedCategory"]).NAME);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        private static Chart RenderTrendByCategoryChart(List<LastYearCategoryExpensesByMonthByUser> items, int width, int height, int lastMonthNumber, string category)
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
        public ActionResult Trend(int lastMonthNumber)
        {
            try
            {
                var allItems = _repository.GetLastYearTotalExpensesByMonth(GetUserId(), lastMonthNumber);
                ViewBag.CurrencyGroups = GetCurrencyGroups(allItems);

                Session["LastYearTotalExpensesResult"] = allItems;
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

                var allItems = _repository.GetLastYearCategoryExpensesByMonth(GetUserId(), (int)categoryId, (int)lmn);
                ViewBag.CurrencyGroups = GetCurrencyGroups(allItems);
                Session["LastYearByCategoryExpensesResult"] = allItems;

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
        public FileResult GetBalanceChartContentWh(int currencyGroupId, int width, int height)
        {
            try
            {
                //  Gets chart object.
                var myChart = RenderBalanceChart(currencyGroupId, width, height, (int)Session["LastMonthNumber"]);
                return File(myChart.GetBytes(), System.Net.Mime.MediaTypeNames.Application.Octet, _seqNum++ + ".jpg");
            }
            catch
            {
                return File(Url.Content("~/Content/failure.png"), System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }

        //  https://www.evernote.com/shard/s132/nl/14501366/47e64199-5c58-43a1-9d0d-9d3081811def
        private Chart RenderBalanceChart(int currencyGroupId, int width, int height, int lastMonthNumber)
        {
            var allItems = (List<LastYearBalanceByMonthByUser>)Session["LastYearBalanceResult"];
            var items = FilterItemsByGroupId(allItems, currencyGroupId);

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
                var allItems = _repository.GetLastYearBalanceByMonth(GetUserId(), lastMonthNumber);
                ViewBag.CurrencyGroups = GetCurrencyGroups(allItems);

                Session["LastYearBalanceResult"] = allItems;
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
