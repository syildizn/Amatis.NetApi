using Microsoft.AspNetCore.Mvc;
using ProjectService.WebAPI.Data;
using ProjectService.WebAPI.SeedData;
using ProjectService.WebAPI.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        private readonly IUsersService _usersService;

        public ProjectsController(IProjectsService projectsService, IUsersService usersService)
        {
            _projectsService = projectsService;
            _usersService = usersService;
        }

        // POST api/projects/{projectId}/users
        [HttpPost("{projectId}/users")]
        public async Task<IActionResult> AddUserToProject(int projectId, [FromBody] ProjectForm projectForm)
        {
            var project = await _projectsService.Get(new int[] { projectId });
            if(project.Any())
            {
                return BadRequest($"This {projectId} already exists");
            }

            var project1 = new Project
            {
                Id = projectId,
                Name = projectForm.Name,
                IsAvailable = true,
                AddedDate = DateTime.UtcNow
        };

            var addedProject = await _projectsService.Add(project1);
            if (addedProject == null)
            {
                return BadRequest("User could not be added.");
            }

            var projectToPass = new Project
            {
                Id = addedProject.Id, 
                Name = addedProject.Name,
                AddedDate = addedProject.AddedDate,
                IsAvailable=addedProject.IsAvailable
                
            };

            return CreatedAtAction(nameof(UsersController.CreateUser), "Users", new { id = addedProject.Id }, projectToPass);
        }

       
       /* private async Task<IActionResult> GetUserById(int id)
        {
            var user = await _usersService.Get(projectId: 0, new int[] { id }); 
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }*/

        // GET api/projects/{projectId}/users
        [HttpGet("{projectId}/users")]
        public async Task<IActionResult> GetUsersForProject(int projectId)
        {


            int[] projectIds = new[] { projectId };

            var users = await _projectsService.Get(projectIds);
            if (users == null || !users.Any())
            {
                return NotFound($"Users for project ID {projectId} not found.");
            }

            return Ok(users);
        }

        // DELETE api/projects/{projectId}
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var project = await _projectsService.Get(new int[] { projectId });
            if (!project.Any())
            {
                return NotFound($"Project with ID {projectId} not found.");
            }

            var projectToDelete = project.First(); 
            var result = await _projectsService.Delete(projectToDelete);
            if (!result)
            {
                return BadRequest("Project could not be deleted.");
            }
            return NoContent();
        }
    }
}
