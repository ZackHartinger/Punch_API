using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Punch_API.Models;
using Punch_API.Models.Accounts;
using Punch_API.Models.Users;

namespace Punch_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly PunchDbContext _context;

        public AppUsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, PunchDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/AppUsers
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAppUsers()
        {
            return await _context.AppUsers
                .Include(au => au.Companies)
                .Include(au => au.EmployeeWorkDays)
                .ToListAsync();
        }

        // GET: api/AppUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetAppUser(int id)
        {
            var appUser = await _context.AppUsers.FindAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            return appUser;
        }

        [HttpGet]
        [Route("get-current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            return Ok("UserId: " + currentUserId);
        }

        // POST: api/AppUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("sign-up")]
        public async Task<ActionResult<AppUser>> SignUp([FromBody] AppUserDTO appUser)
        {
            if (ModelState.IsValid)
            {
                var defaultCompany = _context.Companies.FirstOrDefault(c => c.CompanyId == 1);
                var user = new AppUser
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Email = appUser.Email,
                    UserName = appUser.FirstName + appUser.LastName,
                    Companies = [defaultCompany]
                };
                var result = await _userManager.CreateAsync(user, appUser.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return CreatedAtAction("GetAppUser", new { id = user.Id }, appUser);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }
            return NoContent();
        }

        [HttpPost]
        [Route("log-in")]
        public async Task<IActionResult> LogIn([FromBody] AppUserLoginDTO appUser)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUsers.FirstOrDefault(u => u.UserName == appUser.Username);
                var result = await _signInManager.PasswordSignInAsync(user, appUser.Password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded) 
                {
                    return Ok("Log in successfull!");
                }
            }
            return NoContent();
        }

        [HttpPost]
        [Route("log-out")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok("Log out successfull!");
        }

        // DELETE: api/AppUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppUser(int id)
        {
            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            _context.AppUsers.Remove(appUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppUserExists(int id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }
    }
}
