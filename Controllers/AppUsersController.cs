using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetAppUsers()
        {
            var appUsers =  await _context.AppUsers
                .Include(au => au.Companies)
                .Include(au => au.EmployeeWorkDays)
                .ToListAsync();

            var appUserDTOs = new List<AppUserDTO>();

            foreach(AppUser appUser in appUsers)
            {
                var roles = await _userManager.GetRolesAsync(appUser);
                appUserDTOs.Add(new AppUserDTO
                {
                    Id = appUser.Id,
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    FullName = appUser.FullName,
                    Email = appUser.Email,
                    Companies = appUser.Companies,
                    Roles = roles
                });
            }

            if(appUsers == null || appUserDTOs == null)
            {
                return BadRequest("There are no users associated with this company");
            }

            return Ok(appUserDTOs);
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
            if (currentUser.Identity.IsAuthenticated == true)
            {
                var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                return Ok("UserId: " + currentUserId);
            }
            else
            {
                return BadRequest("You are not logged in.");
            }
        }

        [HttpGet]
        [Route("is-authenticated")]
        public async Task<IActionResult> IsLoggedIn()
        {
            ClaimsPrincipal currentUser = this.User;
            if(currentUser.Identity.IsAuthenticated == true)
            {
                var user = await _context.Users
                    .Include(au => au.Companies)
                    .FirstOrDefaultAsync(au => au.Id == Convert.ToInt32(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value));
                var roles = await _userManager.GetRolesAsync(user);
                var userDTO = new AppUserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Companies = user.Companies,
                    Roles = roles
                };
                return Ok(userDTO);
            }
            else
            {
                return BadRequest(false);
            }
        }

        // POST: api/AppUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("sign-up")]
        public async Task<ActionResult<AppUser>> SignUp([FromBody] AppUsersSignUpDTO appUser)
        {
            var isInvited = _context.Invations.Any(i => i.Email == appUser.Email);
            if(isInvited)
            {
                if (ModelState.IsValid)
                {
                    var defaultCompany = _context.Companies.FirstOrDefault(c => c.CompanyId == 1);
                    var user = new AppUser
                    {
                        FirstName = appUser.FirstName,
                        LastName = appUser.LastName,
                        Email = appUser.Email,
                        UserName = appUser.Email,
                        Companies = [defaultCompany]
                    };
                    var testUser = _context.AppUsers.FirstOrDefault(u => u.Email == user.Email);
                    if (testUser == null)
                    {
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
                    else
                    {
                        return BadRequest("A user with this email already exists.");
                    } 
                }
            }
            else
            {
                return BadRequest("You can only sign up with a valid invitation. To receive an invitation, please request one from your company's administrator");
            }
            return NoContent();
        }

        [HttpPost]
        [Route("log-in")]
        public async Task<IActionResult> LogIn([FromBody] AppUserLoginDTO appUser)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AppUsers
                    .Include(au => au.Companies)
                    .FirstOrDefault(u => u.UserName == appUser.Email);
                var roles = await _userManager.GetRolesAsync(user);
                var result = await _signInManager.PasswordSignInAsync(user, appUser.Password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded) 
                {
                    var userDTO = new AppUserDTO
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = user.FullName,
                        Email = user.Email,
                        Companies = user.Companies,
                        Roles = roles
                    };
                    return Ok(userDTO);
                }
                else if (user == null)
                {
                    return NotFound("There was no user found with this email address or username");
                }
                else
                {
                    return BadRequest("There was an error. Please review credentials");
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

        [HttpPost]
        [Route("add-to-role/{id}/{roleName}")]
        public async Task<IActionResult> AddUserToRole(int id, string roleName)
        {
            try
            {
                var user = await _context.AppUsers.FindAsync(id);
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    _context.SaveChanges();
                    return Ok("User has successfully been added to role: " + roleName);
                }
                else if(await _userManager.IsInRoleAsync(user, roleName))
                {
                    return BadRequest("User is already in role: " + roleName);
                }
                else if(user == null)
                {
                    return NotFound("There is no user that matches the id: " + id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("remove-from-role/{id}/{roleName}")]
        public async Task<IActionResult> RemoveFromRole(int id, string roleName)
        {
            try
            {
                var user = await _context.AppUsers.FindAsync(id);
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);    
                if (result.Succeeded)
                {
                    _context.SaveChanges();
                    return Ok(user.FullName + " was successfully removed from role: " + roleName);
                }
                else if (await _userManager.IsInRoleAsync(user, roleName) == false)
                {
                    return BadRequest(user.FullName + " is already not in role: " + roleName);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }

        [HttpPost]
        [Route("invite-new-user/{email}")]
        public async Task<IActionResult> InviteNewUser(string email)
        {
            var invitation = new Invitation { Email = email };
            try
            {   
                var checkInvitation = _context.Invations.Where(i => i.Email == email);
                if(checkInvitation == null)
                {
                    _context.Invations.Add(invitation);
                    _context.SaveChanges();
                
                    var client = new SmtpClient("smtp.gmail.com", 587);
                    client.Credentials = new System.Net.NetworkCredential("punchtimemanagement@gmail.com", "hjgoyvxnaqytonyg");
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("punchtimemanagement@gmail.com");
                    mailMessage.To.Add(email);
                    mailMessage.Subject = "Test";
                    mailMessage.Body = "" +
                        "<div style='padding: 25px; width: 100%; height: 50%; background-color: blue;'>" +
                            "<a href='http://localhost:5173/sign-up'>Sign up</a> " +
                        "</div>"; 
                    mailMessage.IsBodyHtml = true;

                    await client.SendMailAsync(mailMessage);

                    return Ok("Invitation sent!");
                }
                else
                {
                    return BadRequest("An invitation has already been sent to this email address!");
                }
            }
            catch (Exception)
            {
                throw;
            }
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
