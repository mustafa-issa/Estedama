using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace ChartsMix.Models
{
    public class ChartsDatabaseManager
    {
        public string _connectionString = ConfigurationManager.ConnectionStrings["cmixDbConnection"].ConnectionString;
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        private static T GetValue<T>(object readerValue, T defaultValue = default(T))
        {
            if (readerValue == DBNull.Value)
                return defaultValue;
            else
                return (T)Convert.ChangeType(readerValue, typeof(T));
        }

        public List<PieSeriesData> GetPieChartMeters(int[] ids, DateTime fromDate, DateTime toDate, PiePeriod period)
        {
            var result = new List<PieSeriesData>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.Parameters.Add(new SqlParameter("@Ids", string.Join(",", ids)));
                    switch (period)
                    {
                        case PiePeriod.Day:
                            fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);
                            fromDate = fromDate.AddSeconds(-fromDate.Second);
                            toDate = DateTime.Now;
                            break;
                        case PiePeriod.Week:
                            fromDate = DateTime.Now;
                            fromDate = fromDate.AddDays(-7);
                            fromDate = fromDate.AddSeconds(-fromDate.Second);
                            toDate = DateTime.Now;
                            break;
                        case PiePeriod.Month:
                            fromDate = DateTime.Now;
                            fromDate = fromDate.AddMonths(-1);
                            fromDate = fromDate.AddSeconds(-fromDate.Second);
                            toDate = DateTime.Now;
                            break;
                        case PiePeriod.Year:
                            fromDate = DateTime.Now;
                            fromDate = fromDate.AddYears(-1);
                            fromDate = fromDate.AddSeconds(-fromDate.Second);
                            toDate = DateTime.Now;
                            break;
                        case PiePeriod.Custom:
                            fromDate = fromDate.AddSeconds(-fromDate.Second);
                            toDate = toDate.AddSeconds(59 - toDate.Second);
                            break;
                    }
                    command.Parameters.Add(new SqlParameter("@From", fromDate));
                    command.Parameters.Add(new SqlParameter("@To", toDate));

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "Get_Pie3D_Chart";
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new PieSeriesData
                            {
                                Name = GetValue<string>(reader["Name"], string.Empty),
                                Y = GetValue<double>(reader["FloatVALUE"], 0.0)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public List<Meter> GetAllMeters()
        {
            var result = new List<Meter>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("Select * from tbTrendLogRelation", connection);
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "Select GUID, ParentGuid, EntityID, Name, Description, Path, Type from tbTrendLogRelation";
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Meter
                            {
                                Id = new Guid(reader["GUID"].ToString()),
                                ParentId = new Guid(reader["ParentGUID"].ToString()),
                                EntityId = GetValue<int>(reader["EntityID"], 0),
                                Name = GetValue<string>(reader["Name"], string.Empty),
                                Description = GetValue<string>(reader["Description"], string.Empty),
                                Path = GetValue<string>(reader["Path"], string.Empty),
                                Type = GetValue<string>(reader["Type"], string.Empty)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public Meter GetMeterTree()
        {
            return FormTree(GetAllMeters());
        }

        private Meter FormTree(List<Meter> meters)
        {
            var server = new Meter();
            server.Name = "Server";
            foreach (var meter in meters)
            {
                var node = meters.FirstOrDefault(m => m.Id == meter.ParentId);
                if (node == null)
                {
                    meter.Parent = server;
                    server.Children.Add(meter);
                }
                else
                {
                    meter.Parent = node;
                    node.Children.Add(meter);
                }
            }

            return server;
        }

        public List<Series> GetBarChartMeters(int[] ids, DateTime fromDate, DateTime toDate, BarPeriod period, out List<string> dates)
        {
            var result = new List<Series>();

            List<double> tokyoValues = new List<double> { 49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4 };
            List<double> nyValues = new List<double> { 83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3 };
            List<double> berlinValues = new List<double> { 42.4, 33.2, 34.5, 39.7, 52.6, 75.5, 57.4, 60.4, 47.6, 39.1, 46.8, 51.1 };
            List<double> londonValues = new List<double> { 48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2 };
            List<ColumnSeriesData> tokyoData = new List<ColumnSeriesData>();
            List<ColumnSeriesData> nyData = new List<ColumnSeriesData>();
            List<ColumnSeriesData> berlinData = new List<ColumnSeriesData>();
            List<ColumnSeriesData> londonData = new List<ColumnSeriesData>();

            tokyoValues.ForEach(p => tokyoData.Add(new ColumnSeriesData { Y = p }));
            nyValues.ForEach(p => nyData.Add(new ColumnSeriesData { Y = p }));
            berlinValues.ForEach(p => berlinData.Add(new ColumnSeriesData { Y = p }));
            londonValues.ForEach(p => londonData.Add(new ColumnSeriesData { Y = p }));
            result = new List<Series>
            {
                new ColumnSeries
                {
                    Name = "Tokyo",
                    Data = tokyoData
                },
                new ColumnSeries
                {
                    Name = "New York",
                    Data = nyData
                },
                new ColumnSeries
                {
                    Name = "Berlin",
                    Data = berlinData
                },
                new ColumnSeries
                {
                    Name = "London",
                    Data = londonData
                }
            };

            dates = new List<string> {
                        "1",
                        "2",
                        "Mar",
                        "Apr",
                        "May",
                        "Jun",
                        "Jul",
                        "Aug",
                        "Sep",
                        "Oct",
                        "Nov",
                        "Dec"
                    };

            //try
            //{
            //    using (SqlConnection connection = new SqlConnection(_connectionString))
            //    {
            //        var command = new SqlCommand();
            //        command.Connection = connection;
            //        command.Parameters.Add(new SqlParameter("@Ids", string.Join(",", ids)));
            //        switch (period)
            //        {
            //            case PiePeriod.Day:
            //                fromDate = DateTime.Now;
            //                fromDate.AddDays(-1);
            //                fromDate.AddSeconds(-fromDate.Second);
            //                toDate = DateTime.Now;
            //                break;
            //            case PiePeriod.Week:
            //                fromDate = DateTime.Now;
            //                fromDate.AddDays(-7);
            //                fromDate.AddSeconds(-fromDate.Second);
            //                toDate = DateTime.Now;
            //                break;
            //            case PiePeriod.Month:
            //                fromDate = DateTime.Now;
            //                fromDate.AddMonths(-1);
            //                fromDate.AddSeconds(-fromDate.Second);
            //                toDate = DateTime.Now;
            //                break;
            //            case PiePeriod.Year:
            //                fromDate = DateTime.Now;
            //                fromDate.AddYears(-1);
            //                fromDate.AddSeconds(-fromDate.Second);
            //                toDate = DateTime.Now;
            //                break;
            //            case PiePeriod.Custom:
            //                fromDate.AddSeconds(-fromDate.Second);
            //                toDate.AddSeconds(59 - toDate.Second);
            //                break;
            //        }
            //        command.Parameters.Add(new SqlParameter("@From", fromDate));
            //        command.Parameters.Add(new SqlParameter("@To", toDate));

            //        command.CommandType = System.Data.CommandType.StoredProcedure;
            //        command.CommandText = "Get_Bar_Chart";
            //        connection.Open();
            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                result.Add(new Series
            //                {

            //                });
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return result;
        }

        public List<Series> GetLineChartMeters(int[] ids, DateTime fromDate, DateTime toDate, BarPeriod period, out List<string> dates)
        {
            var result = new List<Series>();

            List<double> tokyoValues = new List<double> { 49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4 };
            List<double> nyValues = new List<double> { 83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3 };
            List<double> berlinValues = new List<double> { 42.4, 33.2, 34.5, 39.7, 52.6, 75.5, 57.4, 60.4, 47.6, 39.1, 46.8, 51.1 };
            List<double> londonValues = new List<double> { 48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2 };
            List<LineSeriesData> tokyoData = new List<LineSeriesData>();
            List<LineSeriesData> nyData = new List<LineSeriesData>();
            List<LineSeriesData> berlinData = new List<LineSeriesData>();
            List<LineSeriesData> londonData = new List<LineSeriesData>();

            tokyoValues.ForEach(p => tokyoData.Add(new LineSeriesData { Y = p }));
            nyValues.ForEach(p => nyData.Add(new LineSeriesData { Y = p }));
            berlinValues.ForEach(p => berlinData.Add(new LineSeriesData { Y = p }));
            londonValues.ForEach(p => londonData.Add(new LineSeriesData { Y = p }));
            result = new List<Series>
            {
                new LineSeries
            {
                Name = "Tokyo",
                Data = tokyoData
            },
            new LineSeries
            {
                Name = "London",
                Data = londonData
            }

               
            };

            dates = new List<string> {
                        "1",
                        "2",
                        "Mar",
                        "Apr",
                        "May",
                        "Jun",
                        "Jul",
                        "Aug",
                        "Sep",
                        "Oct",
                        "Nov",
                        "Dec"
                    };

           
            return result;
        }
    }
}