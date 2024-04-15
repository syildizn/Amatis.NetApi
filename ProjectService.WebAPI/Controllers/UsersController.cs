using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectService.WebAPI.Data;
using ProjectService.WebAPI.Services;

namespace ProjectService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        // GET api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _usersService.Get(projectId: 0, ids: new int[] { id });
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var createdUser = await _usersService.Add(user);
            if (createdUser == null)
            {
                return BadRequest("Could not create user.");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        // PUT api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var updatedUser = await _usersService.Update(user);
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }

        // DELETE api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _usersService.Get(projectId: 0, ids: new int[] { id });
            if (user == null)
            {
                return NotFound();
            }

            var result = await _usersService.Delete((User)user);
            if (!result)
            {
                return BadRequest("Could not delete user.");
            }

            return NoContent();
        }

        
    }
}
