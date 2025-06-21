using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{

    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public string Index()
        {
            return "Latest Version";
        }
    }
}
