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
