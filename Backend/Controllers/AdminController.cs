using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly SmallBusinessContext _context;

    public AdminController(IMemoryCache cache, SmallBusinessContext context)
    {
        _cache = cache;
        _context = context;
    }

    [HttpGet(template: "list")]
    public async Task<IActionResult> ListAdminAction()
    {
        if (!_cache.TryGetValue("list_admin", out List<ContextModels.UserContextModel>? list))
        {
            list = await _context.Admins.AsNoTracking().ToListAsync();

            _cache.Set("list_admin", list!, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateAdminAction([FromBody] CreateUserDTO adminDTO)
    {
        var admin = new ContextModels.UserContextModel
        {
            Role = adminDTO.Role,
            Name = adminDTO.Name,
            Password = adminDTO.Password,
            Email = adminDTO.Email
        };

        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return Ok();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateAdminAction([FromBody] UpdateUserDTO updateDTO)
    {
        if (updateDTO.Id is null)
        {
            return BadRequest($"Id must be >=1, received:{updateDTO.Id}");
        }

        var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Id == updateDTO.Id);

        if (admin is null)
        {
            return NotFound($"No Admin of Id:{updateDTO.Id} was found");
        }

        if (!String.IsNullOrEmpty(updateDTO.Role))
        {
            admin.Role = updateDTO.Role!;
        }
        if (!String.IsNullOrEmpty(updateDTO.Name))
        {
            admin.Name = updateDTO.Name;
        }
        if (!String.IsNullOrEmpty(updateDTO.Password))
        {
            admin.Password = updateDTO.Password;
        }
        if (!String.IsNullOrEmpty(updateDTO.Email))
        {
            admin.Email = updateDTO.Email;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteAdminAction([FromBody] int? id)
    {
        if (id is null)
        {
            return BadRequest($"Id must be >=1, received:{id}");
        }

        var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Id == id);

        if (admin is null)
        {
            return BadRequest($"No Admin of Id:{id} was found");
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }
}