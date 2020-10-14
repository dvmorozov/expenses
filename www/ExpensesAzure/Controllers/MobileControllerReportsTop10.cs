using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Diagnostics;
using SocialApps.Models;

namespace SocialApps.Controllers
{
    [Authorize]
    public partial class MobileController : PersonalizedController
    {
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

        //  https://www.evernote.com/shard/s132/nl/14501366/8334c8f9-2fe0-4178-9d7d-8ae6785318a7
        private Chart RenderTop10Chart(int currencyGroupId, int width, int height, bool? pie)
        {
            var allItems = (List<EstimatedTop10CategoriesForMonthByUser3_Result>)Session["Top10CategoriesResult"];
            var items = FilterItemsByGroupId(allItems, currencyGroupId);

            var year = (int)Session["Top10Year"];
            var month = (int)Session["Top10Month"];

            var dt = new DateTime(year, month, 1);
            //  https://www.evernote.com/shard/s132/nl/14501366/e0eb1c4e-4561-4da4-ae7c-5c26648ec6fc
            //  Chart header was hidden.
            var chart = new Chart(width, height, theme: ChartTheme.Vanilla);
            //  Numbers instead of names.
            //var xValue = items.Select(t => t.NAME).ToList();
            var yValues = items.Select(t => t.TOTAL).ToList();

            if ((pie ?? false) && Session["MonthTotal"] != null)
            {
                var positions = new List<string>();

                //  https://www.evernote.com/shard/s132/nl/14501366/a632edc9-5b3d-4f06-90e1-1e32683bc071
                for (var j = 0; j < yValues.Count(); j++)
                    positions.Add((j + 1).ToString());

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

        private void SetReportTitle()
        {
            var dt = new DateTime((int)Session["Top10Year"], (int)Session["Top10Month"], 1);
            var title = _repository.GetLocalizedResourceString("ReportTop10Of");
            ViewBag.Title = title + " " + dt.ToString("MMM yyyy");
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
                var allItems = _repository.GetTop10CategoriesWithResidue(userId, now);
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

                TempData["MonthCalendarIndex"] = 0;

                SetReportTitle();

                return View("Top10");
            }
            catch (Exception e)
            {
                Application_Error(e);
                return View("Error", new HandleErrorInfo(e, "Mobile", "Top10"));
            }
        }
    }
}