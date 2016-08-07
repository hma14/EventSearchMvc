using EventSearchMvc.Models;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace EventSearchMvc.Controllers
{
    public class SearchController : Controller
    {
        public string Location { get; set; }
        public string Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Page { get; set; }
        

        // GET: Search
        public ActionResult Index(string location, string category, DateTime? startDate, DateTime? endDate, int? page)
        {
            if (!string.IsNullOrEmpty(location))
            {
                Session["Location"] = location;
                ViewBag.Location = location;
            }
            Location = (string)Session["Location"];

            if (!string.IsNullOrEmpty(category))
            {
                Session["Category"] = category;
                ViewBag.Category = category;
            }
            Category = (string)Session["Category"];


            if (startDate.HasValue)
            {
                Session["StartDate"] = (DateTime)startDate;
                ViewBag.StartDate = (DateTime)startDate;
            }
            StartDate = (DateTime)Session["StartDate"];

            if (endDate.HasValue)
            {
                Session["EndDate"] = (DateTime)endDate;
                ViewBag.EndDate = (DateTime)endDate;
            }
            EndDate = (DateTime)Session["EndDate"];

            if (page.HasValue)
            {
                Page = (int)page;
            }
            else
            {
                Page = 1;
            }
            

            string url = ConfigurationManager.AppSettings["URL"];
            string appKey = ConfigurationManager.AppSettings["AppKey"];
            RestClient client = new RestClient(url, HttpVerb.GET);
            int totalPages = 1000;
            int page_size = 10;
            string json = null;

            if (DataCache.GetCachedObject("CachedData") == null || location != null || category != null || startDate.HasValue || endDate.HasValue)
            {
                json = client.MakeRequest(string.Format("?app_key={0}&location={1}&q={2}&date={3}-{4}&page_size={5}&sort_order=start_time&sort_direction=descending",
                                                appKey, Location, Category, StartDate.ToString("yyyyMMdd00"), EndDate.ToString("yyyyMMdd00"), totalPages));
                DataCache.SetCachedObjectPermanent("CachedData", json);
            }
            else
            {
                json = (string) DataCache.GetCachedObject("CachedData");
            }

            JObject jobject = JObject.Parse(json);
            if (jobject["events"].HasValues)
            {
                var evts = jobject["events"]["event"];

                IList<Events> events = new List<Events>();
                foreach (var ev in evts)
                {
                    Events e = new Events
                    {

                        Title = (string) ev["title"],
                        Performers =
                            ev["performers"].HasValues ? ev["performers"].ToObject<Dictionary<string, JToken>>() : null,
                        Image = ev["image"].HasValues ? ev["image"].ToObject<Dictionary<string, JToken>>() : null,
                        StartDatetime = (DateTime) ev["start_time"],
                        Location =
                            (string) ev["venue_address"] + ", " + (string) ev["city_name"] + ", " +
                            (string) ev["region_abbr"] + ", " + (string) ev["country_name"],
                    };
                    events.Add(e);
                }
                ViewBag.CurrentSort = "start_time";

                ViewBag.Model = events.ToPagedList(Page, page_size);

                return View();
            }
            HandleErrorInfo error = new HandleErrorInfo(new Exception("Found nothing"), "Search",  "Index");
            return View("_Error", error);
        }

    }

}