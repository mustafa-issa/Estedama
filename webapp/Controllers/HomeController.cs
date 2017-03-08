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
        // GET: home/index
        public ActionResult Index()
        {
            var model = new DashbordModel();
            PrepareChartsModel(model);
            return View(model);
        }

        
        public ActionResult GetLineChart(LineChartModel model)
        {
            var db = new ChartsDatabaseManager();
            var response = new LineChartDataModel();
            var dates = new List<string>();
            response.Result = db.GetLineChartMeters(out dates, model.From, model.To, model.period, model.Ids);
            response.Dates = dates;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Index(DashbordModel model, int[] pieIds)
        {
            PrepareChartsModel(model);

            if (pieIds != null && pieIds.Length > 0)
            {
                model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(pieIds, model.PieModel.From, model.PieModel.To, model.PieModel.period);
            }
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
        }
    }
}