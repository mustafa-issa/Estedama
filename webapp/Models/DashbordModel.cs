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
            pieGroupChartModel = new PieGroupModel();
            comparisonChartModel = new ComparisonChartModel();
            tableModel = new TableModel();
        }
        public Pie3DModel PieModel { get; set; }
        public BarChartModel barChartModel { get; set; }
        public LineChartModel lineChartModel { get; set; }
        public PieGroupModel pieGroupChartModel { get; set; }
        public ComparisonChartModel comparisonChartModel { get; set; }
        public TableModel tableModel { get; set; }
    }
}