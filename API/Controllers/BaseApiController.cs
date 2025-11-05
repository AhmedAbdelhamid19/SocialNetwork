using API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Apply the LogUserActivity action filter to all controllers that inherit from BaseApiController
    // it's attirbute tell dotnet to run the filter for all actions in derived controllers
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
