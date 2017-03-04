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
        public ActionResult Index(DashbordModel model, int[] pieIds, int[] barIds)
        {
            PrepareChartsModel(model);

            //if (pieIds != null && pieIds.Length > 0)
            //{
            //    model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(pieIds,DateTime.MinValue,DateTime.MaxValue,model.PieModel.period);
            //}
            List<string> dates;
            //if (barIds != null && barIds.Length > 0)
            //{
                model.barChartModel.Data = new ChartsDatabaseManager().GetBarChartMeters(barIds, DateTime.MinValue, DateTime.MaxValue, model.PieModel.period, out dates);
            //}



            model.barChartModel.Dates = dates;

            //if (compIds != null && compIds.Length > 0)
            //{
            //    model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(compIds);
            //}

            //if (lineIds != null && lineIds.Length > 0)
            //{
            //    model.PieModel.Data = new ChartsDatabaseManager().GetPieChartMeters(lineIds);
            //}
            return View(model);
        }

        private void PrepareChartsModel(DashbordModel model)
        {
            db = new Models.ChartsDatabaseManager();
            model.PieModel.TreeRoot = db.GetMeterTree();
        }
    }
}