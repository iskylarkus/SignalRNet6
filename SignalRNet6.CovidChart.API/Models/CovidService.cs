using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRNet6.CovidChart.API.Hubs;

namespace SignalRNet6.CovidChart.API.Models
{
    public class CovidService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;

        public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IQueryable<Covid> GetList()
        {
            return _context.Covids.AsQueryable();
        }

        public async Task SaveCovid(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChart());
        }

        public List<CovidChart> GetCovidChart()
        {
            List<CovidChart> covidCharts = new List<CovidChart>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"
                    SELECT [CovidDate], [1], [2], [3], [4], [5] FROM (
	                    SELECT [City], [Count], CAST([CovidDate] AS DATE) AS [CovidDate] FROM [dbo].[Covids]
                    ) AS CovidSource
                    PIVOT (
	                    SUM([Count]) FOR [City] IN ([1], [2], [3], [4], [5])
                    ) AS CovidPivot
                    ORDER BY [CovidDate] ASC";

                command.CommandType = System.Data.CommandType.Text;

                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart covidChart = new CovidChart();

                        covidChart.CovidDate = reader.GetDateTime(0).ToShortDateString();

                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                        {
                            if (DBNull.Value.Equals(reader[x]))
                            {
                                covidChart.Counts.Add(0);
                            }
                            else
                            {
                                covidChart.Counts.Add(reader.GetInt32(x));
                            }
                        });

                        covidCharts.Add(covidChart);
                    }
                }

                _context.Database.CloseConnection();
            }

            return covidCharts;
        }
    }
}
