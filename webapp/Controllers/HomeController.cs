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
            List<string> dates;
            var v = db.GetLineChartMeters(new int[] { 246, 99 }, DateTime.MinValue, DateTime.MaxValue, BarPeriod.Year,out dates);
            var model = new DashbordModel();
            PrepareDashboardModel(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Index(DashbordModel model, int[] pieIds, int[] barIds)
        {
            PrepareDashboardModel(model);

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

        private void PrepareDashboardModel(DashbordModel model)
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