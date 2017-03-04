namespace ChartsMix.Models
{
    public class DashbordModel
    {
        public DashbordModel()
        {
            PieModel = new Pie3DModel();
            barChartModel = new BarChartModel();
        }
        public Pie3DModel PieModel { get; set; }
        public BarChartModel barChartModel { get; set; }
    }
}