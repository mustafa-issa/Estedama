using System;
namespace ChartsMix.Models
{
    public class DashbordModel
    {
        public DashbordModel()
        {
            PieModel = new Pie3DModel();
            barChartModel = new BarChartModel();
            lineChartModel = new LineChartModel();
        }
        public Pie3DModel PieModel { get; set; }
        public BarChartModel barChartModel { get; set; }
        public LineChartModel lineChartModel { get; set; }
    }
}