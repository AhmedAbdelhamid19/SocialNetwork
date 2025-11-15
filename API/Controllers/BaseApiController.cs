using API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Apply the LogUserActivity action filter to all controllers that inherit from BaseApiController
    // it's attirbute tell dotnet to run the filter for all actions in derived controllers
    // parent of multiple controllers, made to allow you to put custom logic to be attached to multiple controllers at once
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
