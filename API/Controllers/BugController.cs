using API.Entities;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class BugController : BaseApiController
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
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult GetUnauthorized() 
        {
            return Ok("You are authorized");
        }
    }
}
