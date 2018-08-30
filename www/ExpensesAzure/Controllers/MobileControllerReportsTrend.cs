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
    }
}
