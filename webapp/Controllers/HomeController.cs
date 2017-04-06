using ChartsMix.Models;
using System.Web.Mvc;
using Highsoft.Web.Mvc.Charts;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System;

namespace ChartsMix.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ChartsDatabaseManager db;

        // GET: home/index
        public async Task<ActionResult> Index()
        {
            try
            {
                var model = new DashbordModel();
                await PrepareChartsModel(model);
                return View(model);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // Pie Chart Ajax 
        public async Task<ActionResult> GetPieChart(PieChartModel model)
        {
            try
            {
                db = new ChartsDatabaseManager();
                var result = await db.GetPieChartMeters(model.Ids, model.From, model.To, model.period);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        // Line Chart Ajax
        public async Task<ActionResult> GetLineChart(LineChartModel model)
        {
            try
            {
                db = new ChartsDatabaseManager();
                var response = new LineChartDataModel();
                var details = new ChartDetails();
                response.Result = await db.GetLineChartMeters(details, model.From, model.To, model.period, model.Ids);
                response.Details = details;
                FillSummary(response);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        private void FillSummary(LineChartDataModel response)
        {
            response.MaxConsumption = double.MinValue;
            response.MinConsumption = double.MaxValue;
            var counter = 0;
            var sum = 0.0;
            foreach (var v in response.Result)
            {
                foreach(var c in v.things)
                {
                    response.MaxConsumption = Math.Max(response.MaxConsumption, c.Y.Value);
                    response.MinConsumption = Math.Min(response.MinConsumption, c.Y.Value);
                    sum += c.Y.Value;
                    counter++;
                }
            }
            response.AverageCunsumption = (sum / counter);
        }

        public async Task<ActionResult> GetBarChart(BarChartModel model)
        {
            try
            {
                db = new ChartsDatabaseManager();
                var response = new LineChartDataModel();
                var details = new ChartDetails();
                response.Result = await db.GetLineChartMeters(details, model.From, model.To, model.period, model.Ids);
                response.Details = details;
                FillSummary(response);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> GetGroupChart(GroupModel model)
        {
            try
            {
                db = new ChartsDatabaseManager();
                var list = await db.GetGroupChart(model);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task PrepareChartsModel(DashbordModel model)
        {
            try
            {
                db = new ChartsDatabaseManager();
                Meter meter = new Meter();
                meter = await db.GetMeterTree();
                model.PieModel.TreeRoot = meter;
                model.lineChartModel.TreeRoot = meter;
                model.barChartModel.TreeRoot = meter;
                model.pieGroupChartModel.Group.TreeRoot = meter;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> AddGroup(Group model)
        {
            try
            {
                db = new ChartsDatabaseManager();
                int response = await db.AddGroup(model);
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPieGroupChart(PieGroupModel model)
        {
            db = new ChartsDatabaseManager();
            var response = new LineChartDataModel();
            var dates = new List<string>();
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}