using ChartsMix.Models;
using System.Web.Mvc;
using Highsoft.Web;
using Highsoft.Web.Mvc;
using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace ChartsMix.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ChartsDatabaseManager db;

        // Pie Chart Ajax 
        public ActionResult GetPieChart(PieChartModel model)
        {
            var result = new ChartsDatabaseManager().GetPieChartMeters(model.Ids, model.From, model.To, model.period);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // Line Chart Ajax
        public ActionResult GetLineChart(LineChartModel model)
        {
            var db = new ChartsDatabaseManager();
            var response = new LineChartDataModel();
            var details = new ChartDetails();
            response.Result = db.GetLineChartMeters(out details, model.From, model.To, model.period, model.Ids);
            response.Details = details;
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        // GET: home/index
        public ActionResult Index()
        {
            var model = new DashbordModel();
            PrepareChartsModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DashbordModel model, string GroupName)
        {
            PrepareChartsModel(model);

            PieGroupModel pieDrilldown = new PieGroupModel();
            pieDrilldown = PieDrilldown();
            model.pieGroupChartModel = pieDrilldown;
            Group group = new Group();
            group.Name = GroupName;
            return View(model);
        }

        private void PrepareChartsModel(DashbordModel model)
        {
            db = new ChartsDatabaseManager();
            Meter meter = new Meter();
            meter = db.GetMeterTree();
            model.PieModel.TreeRoot = meter;
            model.lineChartModel.TreeRoot = meter;
            model.barChartModel.TreeRoot = meter;
            model.pieGroupChartModel.Group.TreeRoot = meter;
        }

        public ActionResult AddGroup(Group model)
        {
            var db = new ChartsDatabaseManager();
            int response = db.AddGroup(model);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPieGroupChart(PieGroupModel model)
        {
            var db = new ChartsDatabaseManager();
            var response = new LineChartDataModel();
            var dates = new List<string>();
            //response.Result = db.GetLineChartMeters(out dates, model.From, model.To, model.period, model.Ids);
            //response.Dates = dates;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public PieGroupModel PieDrilldown()
        {
            PieGroupModel model = new PieGroupModel();
            var result = new List<PieSeriesData>();
            var result1 = new List<PieSeriesData>();
            var resultGroup = new List<PieSeriesData>();
            result.Add(new PieSeriesData
            {
                Name = "1",
                Y = 30
            });
            result.Add(new PieSeriesData
            {
                Name = "2",
                Y = 40
            });
            result.Add(new PieSeriesData
            {
                Name = "3",
                Y = 50
            });

            result1.Add(new PieSeriesData
            {
                Name = "4",
                Y = 19
            });
            result1.Add(new PieSeriesData
            {
                Name = "5",
                Y = 17
            });
            result1.Add(new PieSeriesData
            {
                Name = "6",
                Y = 13
            });

            resultGroup.Add(new PieSeriesData
            {
                Name = "1",
                Drilldown = "Microsoft Internet Explorer",
                Y = 60
            });
            resultGroup.Add(new PieSeriesData
            {
                Name = "2",
                Drilldown = "Microsoft Internet Explorer",
                Y = 40
            });
            model.GroupData = resultGroup;



            List<Series> Series = new List<Series>
                {
                    new PieSeries
                    {
                        Name = "Microsoft Internet Explorer",
                        Id = "Microsoft Internet Explorer",
                        Data = result
                    },
                    new PieSeries
                    {
                        Name = "Chrome",
                        Id = "Chrome",
                        Data = result1
                    }
                };
            model.Series = Series;

            return model;
        }
    }
}