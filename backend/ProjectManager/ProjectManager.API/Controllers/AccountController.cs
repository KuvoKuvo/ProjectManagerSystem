using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.BLL.DTOs.Account;
using ProjectManager.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<Employee> _signInManager;
        private readonly UserManager<Employee> _userManager;

        public AccountController(SignInManager<Employee> signInManager, UserManager<Employee> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // POST: api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                var roles = await _userManager.GetRolesAsync(user!);

                return Ok(new
                {
                    Message = "Logged in successfully.",
                    Id = user!.Id,  
                    Email = user!.Email,
                    Role = roles.FirstOrDefault(),
                    IsTemporaryPassword = user.IsTemporaryPassword
                });
            }

            return BadRequest(new { Message = "Invalid email or password." });
        }

        // POST: api/account/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }

        // GET: api/account/me
        // Useful for Vue.js to check if the user session is still alive on page reload
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new { Message = "Not authenticated." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                IsTemporaryPassword = user.IsTemporaryPassword
            });
        }
    }
}
