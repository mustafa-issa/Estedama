using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ChartsMix.Models
{
    public class ChartsDatabaseManager
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["cmixDbConnection"].ConnectionString;
        private string _groupConnectionString = ConfigurationManager.ConnectionStrings["groups"].ConnectionString;
        private static T GetValue<T>(object readerValue, T defaultValue = default(T))
        {
            if (readerValue == DBNull.Value)
                return defaultValue;
            else
                return (T)Convert.ChangeType(readerValue, typeof(T));
        }

        public async Task<List<PieSeriesData>> GetPieChartMeters(int[] ids, DateTime fromDate, DateTime toDate, PiePeriod period)
        {
            var result = new List<PieSeriesData>();
            foreach(var meterId in ids)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        var command = new SqlCommand();
                        command.Connection = connection;
                        command.Parameters.Add(new SqlParameter("@Id", meterId));
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
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
            }
            return result;
        }

        public async Task<List<Meter>> GetAllMeters()
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
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

        public async Task<Meter> GetMeterTree()
        {
            try
            {
                return FormTree(await GetAllMeters());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private Meter FormTree(List<Meter> meters)
        {
            var server = new Meter();
            server.Name = "Server";
            server.Type = "system.base.Folder";
            try
            {
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
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return server;
        }

        public async Task<List<GroupDataModel>> GetGroupChart(GroupModel model)
        {
            try
            {
                var result = new List<GroupDataModel>();
                var total = 0.0;
                switch (model.period)
                {
                    case PiePeriod.Day:
                        model.From = CleanDateTime(DateTime.Now).AddHours(-24);
                        model.To = DateTime.Now;
                        break;
                    case PiePeriod.Week:
                        model.From = CleanDateTime(DateTime.Now).AddDays(-7);
                        model.To = DateTime.Now;
                        break;
                    case PiePeriod.Month:
                        model.From = CleanDateTime(DateTime.Now).AddMonths(-1);
                        model.To = DateTime.Now;
                        break;
                    case PiePeriod.Year:
                        model.From = CleanDateTime(DateTime.Now).AddYears(-1);
                        model.To = DateTime.Now;
                        break;
                    case PiePeriod.Custom:
                        break;
                    default:
                        break;
                }
                foreach (var groupId in model.Ids)
                {
                    var group = new GroupDataModel();
                    var ids = new List<int>();
                    using (SqlConnection connection = new SqlConnection(_groupConnectionString))
                    {
                        var command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = "SELECT EntityId FROM Meters Where GroupId = @Id";
                        command.Parameters.Add(new SqlParameter("@Id", groupId));
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ids.Add(Convert.ToInt16(reader["EntityId"]));
                            }
                        }
                        using (SqlConnection connection2 = new SqlConnection(_groupConnectionString))
                        {
                            var command2 = new SqlCommand();
                            command2.Connection = connection2;
                            command2.CommandType = System.Data.CommandType.Text;
                            command2.CommandText = "SELECT Name FROM Groups WHERE Id = @Id";
                            command2.Parameters.Add(new SqlParameter("@Id", groupId));
                            await connection2.OpenAsync();
                            using (SqlDataReader reader2 = await command2.ExecuteReaderAsync())
                            {
                                if (await reader2.ReadAsync())
                                {
                                    group.name = group.drilldown = reader2["Name"].ToString();
                                }
                            }
                        }
                        connection.Close();
                    }
                    await GetGroupDrillDown(group, ids, model.From, model.To);
                    result.Add(group);
                    total += group.y;
                }
                foreach (var group in result)
                {
                    group.y = group.y / total;
                    group.y *= 100;
                }
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task GetGroupDrillDown(GroupDataModel group, List<int> ids,DateTime from, DateTime to)
        {
            group.y = 0;
            List<GroupDataModel> subGroups = new List<GroupDataModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    foreach (var id in ids)
                    {
                        var command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = @"SELECT sum(FloatVALUE) as Value, max(Name) as Name FROM tbLogTimeValues t1 
                                            LEFT JOIN tbTrendLogRelation t2 on t1.ParentID = t2.EntityID
                                            WHERE ParentId = @Id AND t1.DateTimeStamp BETWEEN @from AND @to
                                            GROUP BY ParentId";
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        command.Parameters.Add(new SqlParameter("@from", from));
                        command.Parameters.Add(new SqlParameter("@to", to));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                subGroups.Add(new GroupDataModel
                                {
                                    name = GetValue<string>(reader["Name"]),
                                    y = GetValue<double>(reader["Value"]),
                                    id = group.name
                                });
                                group.y += GetValue<double>(reader["Value"]);
                            }
                        }
                    }
                    foreach (var subGroup in subGroups)
                    {
                        subGroup.y = subGroup.y / group.y;
                        subGroup.y *= 100;
                    }
                }
                group.subGroups = subGroups;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<CustomLineSeries>> HandleLineChart(int numberOfReturnedValues, DateTime From, DateTime To, BarPeriod period, params int[] ids)
        {
            try
            {

                var result = new List<CustomLineSeries>();
                foreach (var meterId in ids)
                {
                    var queryResult = new List<LineChartItem>();
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        var command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "Bar_Chart_Meter";

                        command.Parameters.Add(new SqlParameter("@Id", meterId));
                        command.Parameters.Add(new SqlParameter("@From", From));
                        command.Parameters.Add(new SqlParameter("@Period", (int) period));
                        command.Parameters.Add(new SqlParameter("@To", To));

                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                queryResult.Add(new LineChartItem
                                {
                                    Name = GetValue<string>(reader["Name"], string.Empty),
                                    entityId = GetValue<int>(reader["entityID"], 0),
                                    value = GetValue<double>(reader["FloatVALUE"], 0.0),
                                    date = GetValue<DateTime>(reader["DateTimeStamp"], DateTime.MinValue)
                                });

                            }
                        }

                    }
                    var meter = queryResult.FirstOrDefault(m => m.entityId == meterId);
                    if(meter != null)
                    {
                        var dataResult = new List<LineSeriesData>();
                        while (queryResult.Count <= numberOfReturnedValues)
                        {
                            queryResult.Add(new LineChartItem
                            {
                                Name = meter.Name,
                                entityId = meter.entityId,
                                value = 0,
                                date = (meter.date < CleanDateTimeWeek(To)) ? DateTime.MaxValue : DateTime.MinValue
                            });
                        }

                        queryResult = queryResult.OrderByDescending(m => m.date).ToList();
                        for (int i = 0; i < queryResult.Count - 1; i++)
                        {
                            dataResult.Add(new LineSeriesData
                            {
                                Y = queryResult[i].value - queryResult[i + 1].value
                            });
                        }
                        dataResult.Reverse();

                        result.Add(new CustomLineSeries
                        {
                            Name = meter.Name,
                            things = dataResult
                        });
                    }
                    else
                    {
                        //
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // In Progress
        public async Task<List<CustomLineSeries>> GetLineChartMeters(ChartDetails details, DateTime fromDate, DateTime toDate, BarPeriod period, int[] ids)
        {
            try
            {
                List<CustomLineSeries> result = new List<CustomLineSeries>();
                switch (period)
                {
                    case BarPeriod.Day:
                        fromDate = DateTime.Now.AddHours(-25);
                        fromDate = CleanDateTime(fromDate);
                        toDate = CleanDateTime(DateTime.Now).AddSeconds(-1);
                        details.Title = "By Day Report";
                        details.SubTitle = "Last 24 Hours Report";

                        (details.Dates = GenerateHours()).Reverse();
                        result = await HandleLineChart(24, fromDate, toDate, period, ids);
                        break;
                    case BarPeriod.Week:
                        fromDate = DateTime.Now.AddDays(-8);
                        fromDate = CleanDateTimeWeek(fromDate);
                        toDate = CleanDateTimeWeek(DateTime.Now).AddSeconds(-1);
                        details.Title = "Week Report";
                        details.SubTitle = "Last 7 Days Report";
                        (details.Dates = GenerateDays()).Reverse();
                        result = await HandleLineChart(7, fromDate, toDate, period, ids);
                        break;
                    case BarPeriod.Year:
                        fromDate = DateTime.Now.AddMonths(-13);
                        fromDate = CleanDateTimeYear(fromDate);
                        toDate = CleanDateTimeYear(DateTime.Now).AddSeconds(-1);
                        details.Title = "Year Report";
                        details.SubTitle = "Last 12 Months Report";
                        (details.Dates = GenerateMonths()).Reverse();
                        result = await HandleLineChart(12, fromDate, toDate, period, ids);
                        break;
                    case BarPeriod.Custom:
                        double hours = (toDate - fromDate).TotalHours;
                        if (hours >= 2 && hours < 48) //period more than 2 hours and less than 2 days
                        {
                            details.Title = "Hours Report";
                            details.SubTitle = "Period from " + fromDate.ToString("dd/MM/yyyy HH:mm") + " to " + toDate.ToString("dd/MM/yyyy HH:mm") + " Report";
                            fromDate = fromDate.AddHours(-1);
                            toDate = toDate.AddHours(1);
                            (details.Dates = GenerateHours((int)hours + 1, toDate)).Reverse();
                            result = await HandleLineChart((int)hours + 1, fromDate, toDate, BarPeriod.Day, ids);
                        }
                        else if (hours >= 48 && hours <= 720) //period more than 2 days and less than or equal to 1 month
                        {
                            int days = (int)hours / 24 + 1;
                            details.Title = "Days Report";
                            details.SubTitle = "Period from " + fromDate.ToString("dd/MM/yyyy") + " to " + toDate.ToString("dd/MM/yyyy") + " Report";
                            fromDate = fromDate.AddDays(-1);
                            toDate = toDate.AddDays(1);
                            (details.Dates = GenerateDays(days, toDate)).Reverse();
                            result = await HandleLineChart(days, fromDate, toDate, BarPeriod.Week, ids);
                        }
                        else if (hours > 720)
                        {
                            int months = (int)hours / 720 + 1;
                            details.Title = "Months Report";
                            details.SubTitle = "Period from " + fromDate.ToString("MM/yyyy") + " to " + toDate.ToString("MM/yyyy") + " Report";
                            fromDate = fromDate.AddMonths(-1);
                            toDate = toDate.AddMonths(1);
                            (details.Dates = GenerateMonths(months, toDate)).Reverse();
                            result = await HandleLineChart(months, fromDate, toDate, BarPeriod.Year, ids);
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    default:
                        details.Dates = null;
                        break;
                }
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #region private Helpers

        private DateTime CleanDateTime(DateTime dateTime)
        {
            dateTime = dateTime.AddMinutes(-dateTime.Minute);
            dateTime = dateTime.AddSeconds(-dateTime.Second);
            dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
            return dateTime;
        }

        private DateTime CleanDateTimeWeek(DateTime dateTime)
        {
            dateTime = dateTime.AddHours(-dateTime.Hour);
            return CleanDateTime(dateTime);
        }

        private DateTime CleanDateTimeYear(DateTime dateTime)
        {
            dateTime = dateTime.AddDays(-dateTime.Day);
            return CleanDateTimeWeek(dateTime);
        }

        private List<string> GenerateMonths(int months = 12, DateTime date = default(DateTime))
        {
            if (date == default(DateTime))
                date = DateTime.Now;
            List<string> dates = new List<string>();
            for (int i = 1; i <= months; i++)
                dates.Add(date.AddMonths(-i).ToString("MM/yy"));
            return dates;
        }

        private List<string> GenerateDays(int days = 7, DateTime date = default(DateTime))
        {
            if (date == default(DateTime))
                date = DateTime.Now;
            List<string> dates = new List<string>();
            for (int i = 1; i <= days; i++)
                dates.Add(date.AddDays(-i).ToString("dd"));
            return dates;
        }

        private List<string> GenerateHours(int hours = 24, DateTime date = default(DateTime))
        {
            if(date == default(DateTime))
                date = DateTime.Now;
            List<string> dates = new List<string>();
            for (int i = 1; i <= hours; i++)
                dates.Add(date.AddHours(-i).ToString("HH"));
            return dates;
        }
        #endregion

        public List<Group> GetAllGroups()
        {
            var result = new List<Group>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_groupConnectionString))
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "Select * from Groups";
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Group
                            {
                                Id = GetValue<int>(reader["Id"], 0),
                                Name = GetValue<string>(reader["Name"], string.Empty),
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

        public async Task<int> AddGroup(Group group)
        {
            var result = new List<Group>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_groupConnectionString))
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "insert into Groups OUTPUT Inserted.Id values (@Name)";
                    command.Parameters.AddWithValue("@Name", group.Name);
                    await connection.OpenAsync();
                    int groupId = (int)( await command.ExecuteScalarAsync());
                    if (group.Ids != null)
                    {
                        foreach (int id in group.Ids)
                        {
                            await AssignMeterToGroup(id, groupId);
                        }
                    }

                    return groupId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AssignMeterToGroup(int EntityId, int GroupId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_groupConnectionString))
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "insert into Meters values (@EntityId, @GroupId)";
                    command.Parameters.AddWithValue("@EntityId", EntityId);
                    command.Parameters.AddWithValue("@GroupId", GroupId);
                    await connection.OpenAsync();
                    await command.ExecuteScalarAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}