using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController(UserManager<AppUser> userManger) :BaseApiController
    {
        [Authorize(Policy ="RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManger.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users) {
                var roles = await userManger.GetRolesAsync(user);
                userList.Add(new 
                { 
                  user.Id,
                  user.Email,
                  Roles = roles.ToList(),
                });
            }

            return Ok(userList);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Only admins can see this");
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult <IList<string>>> EditRoles(string userId, [FromQuery] string rolesName)
        {
            if (string.IsNullOrEmpty(rolesName)) return BadRequest("You must select at least one role");

            var selectedRoles = rolesName.Split(',').ToArray();

            var user = await userManger.FindByIdAsync(userId);

            if (user == null) return BadRequest("Could not retrive user");

            var userRoles = await userManger.GetRolesAsync(user);

            var result = await userManger.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await userManger.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            return Ok(await userManger.GetRolesAsync(user));
        }
    }
}
