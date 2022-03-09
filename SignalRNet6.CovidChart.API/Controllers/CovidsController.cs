using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalRNet6.CovidChart.API.Models;

namespace SignalRNet6.CovidChart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsController : ControllerBase
    {
        private readonly CovidService _service;

        public CovidsController(CovidService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid covid)
        {
            await _service.SaveCovid(covid);

            //IQueryable<Covid> covids = _service.GetList();

            //return Ok(covids);

            return Ok(_service.GetCovidChart());
        }

        [HttpPut]
        public IActionResult GenerateCovid()
        {
            Random random = new Random();

            Enumerable.Range(1, 10).ToList().ForEach(x =>
            {
                foreach (ECity item in Enum.GetValues(typeof(ECity)))
                {
                    var newCovid = new Covid { City = item, Count = random.Next(100, 1000), CovidDate = DateTime.Now.AddDays(x) };

                    _service.SaveCovid(newCovid).Wait();

                    System.Threading.Thread.Sleep(1000);
                }
            });

            return Ok("Saved generated covid data to database");
        } 
    }
}
