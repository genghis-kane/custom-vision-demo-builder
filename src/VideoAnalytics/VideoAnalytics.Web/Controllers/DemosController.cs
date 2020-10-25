using Microsoft.AspNetCore.Mvc;
using VideoAnalytics.Web.Configuration.Interfaces;

namespace VideoAnalytics.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemosController : ControllerBase
    {
        private readonly IDemoSettings _demoSettings;

        public DemosController(IDemoSettings demoSettings)
        {
            _demoSettings = demoSettings;
        }

        [HttpGet]
        public IDemoSettings Get()
        {
            return _demoSettings;
        }
    }
}
