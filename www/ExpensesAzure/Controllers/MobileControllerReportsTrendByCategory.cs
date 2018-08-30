using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using SocialApps.Models;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
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
                    return RedirectToAction("TrendByCategory", new { lastMonthNumber = (object)12 });

                if (lastMonthNumber != null)
                    //  Resets the month number in subsequent requests.
                    Session["LastMonthNumber"] = (int)lastMonthNumber;

                var categoryId = Session["CategoryId"];
                if (categoryId == null)
                    return RedirectToAction("TrendByCategory", new { lastMonthNumber = lmn });

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
    }
}
