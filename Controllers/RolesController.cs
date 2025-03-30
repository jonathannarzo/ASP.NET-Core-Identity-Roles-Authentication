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

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(AppDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;

    }

    // GET: api/Roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
    {
        if (_roleManager.Roles == null)
        {
            return NotFound();
        }
        return await _roleManager.Roles.ToListAsync();
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
    public async Task<ActionResult<Roles>> PostRoles(Roles roles)
    {
        if (_context.Roles == null)
        {
            return Problem("Entity set 'AppDbContext.Roles'  is null.");
        }
        _context.Roles.Add(roles);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (RolesExists(roles.Id))
            {
                return Conflict();
            }
            else
            {
                throw;
            }
        }

        return CreatedAtAction("GetRoles", new { id = roles.Id }, roles);
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