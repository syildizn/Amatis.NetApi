using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectService.WebAPI.Data;
using ProjectService.WebAPI.SeedData;
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

        // GET api/users/project/{projectId}?ids=1,2,3
        // GET api/users/project/{projectId}
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetUsersByProjectId(int projectId)
        {
            // Bu çağrıda ikinci parametre olarak boş bir dizi geçiriyoruz çünkü 
            // belirli kullanıcı id'leri ile ilgilenmiyoruz.
            var users = await _usersService.Get(projectId, new int[] { });

            if (!users.Any())
            {
                return NotFound($"Users for project ID {projectId} not found.");
            }

            return Ok(users);
        }


        // POST api/users
        [HttpPost]
        public async Task<IActionResult> CreateUser( Project project)
        {



            // Random nesnesini oluştur
            Random random = new Random();

            // 1 ile 9999 arasında rastgele bir sayı üret (örnek olarak)
            int randomId = random.Next(1, 100000);

            var user = new User
            {
                Id = randomId,
                Name = project.Name,
                ProjectId = project.Id,
                AddedDate = project.AddedDate

            };

            var createdUser = await _usersService.Add(user);
            if (createdUser == null)
            {
                return BadRequest("Could not create user.");
            }

            return CreatedAtAction(nameof(_usersService.Get), new { id = createdUser.ProjectId, createdUser.Id }, createdUser);
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
        public async Task<IActionResult> DeleteUser(int projectId, int[] ids )
        {
            var user = await _usersService.Get(projectId: projectId, ids: ids);
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
