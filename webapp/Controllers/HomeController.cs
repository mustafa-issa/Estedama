using ChartsMix.Models;
using System.Web.Mvc;
using Highsoft.Web;
using Highsoft.Web.Mvc;
using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;

namespace ChartsMix.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ChartsDatabaseManager db;
        // GET: home/index
        public ActionResult Index()
        {
            db = new Models.ChartsDatabaseManager();
            var model = new DashbordModel();
            PrepareChartsModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(DashbordModel model, int[] pieIds)
        {
            PrepareChartsModel(model);

            if (pieIds != null && pieIds.Length > 0)
            {
                model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(pieIds, model.PieModel.From, model.PieModel.To, model.PieModel.period);
            }

            //if (lineIds != null && lineIds.Length > 0)
            //{
            //    model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(lineIds);
            //}

            //if (columnIds != null && columnIds.Length > 0)
            //{
            //    model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(columnIds);
            //}

            //if (compIds != null && compIds.Length > 0)
            //{
            //    model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(compIds);
            //}
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