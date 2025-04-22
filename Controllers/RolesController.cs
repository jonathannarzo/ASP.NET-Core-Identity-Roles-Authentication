namespace api.Controllers;

using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public RolesController(AppDbContext context, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _context = context;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    // GET: api/Roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles(int? pageNum)
    {
        var RoleData = _roleManager.Roles;
        
        if (RoleData == null)
        {
            return NotFound();
        }

        if (pageNum != null)
        {
            int pageSize = 5;
            var data = await PaginatedList<IdentityRole>.CreateAsync(RoleData, pageNum ?? 1, pageSize);
            
            return Ok(new
            {
                dataList = data,
                PageIndex = data.PageIndex,
                HasNextPage = data.HasNextPage,
                HasPreviousPage = data.HasPreviousPage,
                TotalPages = data.TotalPages
            });
        }
        
        return await RoleData.ToListAsync();
        // return await _context.Roles.ToListAsync();
    }

    // GET: api/Roles/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Roles>> GetRoles(string id)
    {
        if (_context.Roles == null)
        {
            return NotFound();
        }
        var roles = await _context.Roles.FindAsync(id);

        if (roles == null)
        {
            return NotFound();
        }

        return roles;
    }

    // PUT: api/Roles/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRoles(string id, Roles roles)
    {
        if (id != roles.Id)
        {
            return BadRequest();
        }

        _context.Entry(roles).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!RolesExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Roles
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult> PostRoles([FromBody] RoleDTO roleDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var role = _mapper.Map<Roles>(roleDTO);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetRoles), new { role.Name }, role);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        catch (Exception)
        {
            return Problem("Something went wrong", statusCode: 500);
        }
    }

    // DELETE: api/Roles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoles(string id)
    {
        if (_context.Roles == null)
        {
            return NotFound();
        }
        var roles = await _context.Roles.FindAsync(id);
        if (roles == null)
        {
            return NotFound();
        }

        _context.Roles.Remove(roles);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool RolesExists(string id)
    {
        return (_context.Roles?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}