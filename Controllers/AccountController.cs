namespace api.Controllers;

using api.Models;
using api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Security.Claims;
using Microsoft.CodeAnalysis;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApiUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthManager _authManager;

    public AccountController(AppDbContext context, UserManager<ApiUser> userManager, ILogger<AccountController> logger, IMapper mapper, IAuthManager authManager)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
        _authManager = authManager;
    }

    // GET: api/Account
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ApiUser>>> GetUsers(int? pageNum)
    {
        if (_context.Users == null)
        {
            return NotFound();
        }

        var UsersData = _context.Users.OrderByDescending(u => u.DateCreated);
        if (pageNum != null)
        {
            int pageSize = 5;
            var data = await PaginatedList<ApiUser>.CreateAsync(UsersData, pageNum ?? 1, pageSize);
            var userIds = data.Select(u => u.Id).ToList();  // Extract IDs into a separate list

            // Roles of the users that are fetched
            var userRolesData = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .ToListAsync();

            // Create a lookup for roles by UserId for efficient lookup later
            var userRolesLookup = userRolesData
                .GroupBy(ur => ur.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Now, for each user, assign the corresponding roles from the lookup
            foreach (var user in data)
            {
                if (userRolesLookup.TryGetValue(user.Id, out var roles))
                {
                    user.UserRoles = roles.Select(ur => new UserRole { UserId = ur.UserId, RoleId = ur.RoleId }).ToList();
                }
            }
            
            return Ok(new
            {
                dataList = data,
                PageIndex = data.PageIndex,
                HasNextPage = data.HasNextPage,
                HasPreviousPage = data.HasPreviousPage,
                TotalPages = data.TotalPages
            });
        }
        else
        {
            return await UsersData.ToListAsync();
        }
    }

    // GET: api/Account/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiUser>> GetApiUser(string id)
    {
        var apiUser = await _context.Users.FindAsync(id);

        if (apiUser == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(apiUser);

        return Ok(new
        {
            user = apiUser,
            role = userRoles
        });
    }

    // PUT: api/Account/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutApiUser(string id, [FromBody] UpdateUserDTO userDTO)
    {
        var apiUser = await _context.Users.FindAsync(id);

        if (apiUser == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(apiUser);

        var checkEmail = _context.Users.FirstOrDefault(x => x.Email == userDTO.Email && x.Id != apiUser.Id);

        if (checkEmail != null)
        {
            ModelState.AddModelError("email", "Email already exist.");
            return BadRequest(ModelState);
        }

        // set data values
        apiUser.Email = userDTO.Email;
        apiUser.UserName = userDTO.Email;
        apiUser.FirstName = userDTO.FirstName;
        apiUser.LastName = userDTO.LastName;
        apiUser.PhoneNumber = userDTO.PhoneNumber;

        try
        {
            // update info of the user
            await _userManager.UpdateAsync(apiUser);

            // update roles of the user
            await _userManager.RemoveFromRolesAsync(apiUser, userRoles);
            await _userManager.AddToRolesAsync(apiUser, userDTO.Roles);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }

        return NoContent();
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] UserDTO userDTO)
    {
        if (!ModelState.IsValid)
        {
            var allErrors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors?.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
            );
            return BadRequest(new { errors = allErrors });
        }

        try
        {
            var user = _mapper.Map<ApiUser>(userDTO);
            user.UserName = userDTO.Email;
            var result = await _userManager.CreateAsync(user, userDTO.Password);

            if (!result.Succeeded)
            {
                // foreach (var e in result.Errors)
                // {
                //     ModelState.AddModelError(e.Code, e.Description);
                // }
                // return BadRequest(ModelState);
                var allErrors = result.Errors.ToDictionary(
                    e => e.Code ?? "UnknownError",
                    e => new string[] { e.Description ?? "An unknown error occurred." }
                );

                return BadRequest(new { errors = allErrors });
            }

            await _userManager.AddToRolesAsync(user, userDTO.Roles);

            return Accepted();
            // return CreatedAtAction("GetApiUser", new { id = user.Id }, user);
        }
        catch (System.Exception)
        {
            return Problem("Something went wrong", statusCode: 500);
        }
    }


    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ApiUser>> GetUserProfile()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(username))
            return Unauthorized("User ID not found.");

        var profile = await _authManager.ValidateUser(username);

        if (profile == null)
        {
            return NotFound();
        }

        return Ok(profile);
    }

    [HttpPut("profile/update")]
    [Authorize]
    public async Task<ActionResult<ApiUser>> ProfileUpdate([FromBody] UpdateProfileDTO userDTO)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
    
        if (string.IsNullOrEmpty(username))
            return Unauthorized("User ID not found.");

        var profile = await _context.Users.FirstOrDefaultAsync(p => p.UserName == username);

        if (profile == null)
        {
            return NotFound();
        }

        var checkEmail = _context.Users.FirstOrDefault(x => x.Email == userDTO.Email && x.Id != profile.Id);

        if (checkEmail != null)
        {
            ModelState.AddModelError("email", "Email already exist.");
            return BadRequest(ModelState);
        }

        // Proceed updating profile
        profile.FirstName = userDTO.FirstName;
        profile.LastName = userDTO.LastName;
        profile.PhoneNumber = userDTO.PhoneNumber;
        profile.Email = userDTO.Email;
        profile.UserName = userDTO.Email;

        try
        {
            // update profile of the user
            await _userManager.UpdateAsync(profile);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }

        return Ok(profile);
    }

    [HttpPut("profile/changepassword")]
    [Authorize]
    public async Task<ActionResult<ApiUser>> ChangePassword([FromBody] ChangePasswordDTO cpDTO)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
    
        if (string.IsNullOrEmpty(username))
            return Unauthorized("User ID not found.");

        var profile = await _context.Users.FirstOrDefaultAsync(p => p.UserName == username);
        
        if (profile == null)
        {
            return NotFound();
        }

        var result = await _userManager.ChangePasswordAsync(profile, cpDTO.CurrentPassword, cpDTO.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(new { message = "Password changed successfully." });
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginUserDTO userDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _authManager.ValidateUser(userDTO.Email);
            var pass = await _userManager.CheckPasswordAsync(user, userDTO.Password);

            if (user == null || pass == false)
            {
                if (Request.Cookies["userName"] != null)
                {
                    Response.Cookies.Delete("userName");
                }
                if (Request.Cookies["refreshToken"] != null)
                {
                    Response.Cookies.Delete("refreshToken");
                }

                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var refreshToken = await _authManager.CreateRefreshToken();

            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(1)
            };

            Response.Cookies.Append(
                "userName",
                user.Email,
                cookieOptions
            );
            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                cookieOptions
            );

            return Accepted(new
            {
                Token = await _authManager.CreateToken(),
                Roles = roles
            });
        }
        catch (System.Exception)
        {
            // _logger.LogError($"Error occurred during login: {ex.Message}, StackTrace: {ex.StackTrace}");
            return Problem($"Something went wrong {nameof(Login)}", statusCode: 500);
        }
    }

    [HttpGet("refresh")]
    public async Task<ActionResult> GetRefreshToken()
    {
        if (!(Request.Cookies.TryGetValue("userName", out var userName) && Request.Cookies.TryGetValue("refreshToken", out var refreshToken)))
            return BadRequest();

        var user = await _authManager.ValidateUser(userName);

        if (user == null)
            return BadRequest();

        // var isValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken", refreshToken);

        var isValid = _authManager.ValidateRefreshToken(refreshToken, userName);

        if (!isValid)
            return BadRequest();

        return Accepted(new
        {
            Token = await _authManager.CreateToken(),
            Roles = await _userManager.GetRolesAsync(user)
        });
    }


    [HttpGet("logout")]
    public async Task<ActionResult> Logout()
    {
        if (Request.Cookies["userName"] != null)
        {
            var email = Request.Cookies["userName"];
            var user = await _authManager.ValidateUser(email);
            await _userManager.RemoveAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken");
            await _userManager.UpdateSecurityStampAsync(user);

            // Delete the userName cookie
            Response.Cookies.Delete("userName");
        }

        if (Request.Cookies["refreshToken"] != null)
        {
            // Delete the refreshToken cookie
            Response.Cookies.Delete("refreshToken");
        }

        return Ok();
    }

    // DELETE: api/Account/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteApiUser(string id)
    {
        if (_context.Users == null)
        {
            return NotFound();
        }
        var apiUser = await _context.Users.FindAsync(id);
        if (apiUser == null)
        {
            return NotFound();
        }

        _context.Users.Remove(apiUser);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ApiUserExists(string id)
    {
        return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}