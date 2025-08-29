using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BugController : ControllerBase
    {
        [HttpGet("auth")] 
        public IActionResult GetAuth() 
        {
            return Unauthorized();
        }
        [HttpGet("not-found")] 
        public IActionResult GetNotFound() 
        {
            return NotFound();
        }
        [HttpGet("server-error")] 
        public IActionResult GetServerError() 
        {
            throw new Exception("Internal Server Error happen");
        }
        [HttpGet("bad-request")] 
        public IActionResult GetBadRequest() 
        {
            return BadRequest("This is not Good Request");
        }
    }
}
