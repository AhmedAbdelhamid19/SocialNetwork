using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(
                    Directory.GetCurrentDirectory(), 
                    "wwwroot", 
                    "index.html"
                ), 
                "text/html"
            );
        }
    }
}
