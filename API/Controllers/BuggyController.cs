using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController :BaseApiController
    {
        [HttpGet("auth")]
        public IActionResult GetAuth()
        {
            return Unauthorized(); // 401
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("server-error")]
        public IActionResult GetServerError()
        {
            throw new Exception("This is a server error");
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest() // 400
        {
            return BadRequest("This was not a good request");
            //return BadRequest();
        }

    }
}
