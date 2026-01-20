using System.ComponentModel.DataAnnotations;
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
    public async Task<IActionResult> CreateAdminAction([FromBody] CreateUserDTO createUserDTO)
    {
        // TEST WITH AND WITHOUT Id=0
        var admin = new ContextModels.UserContextModel
        {
            Role = createUserDTO.Role,
            Name = createUserDTO.Name,
            Password = createUserDTO.Password,
            Email = createUserDTO.Email
        };

        await _context.Admins.AddAsync(admin);
        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }

    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateAdminAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id, [FromBody] UpdateUserDTO updateUserDTO)
    {
        var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Id == id);

        if (admin is null)
        {
            return NotFound($"No Admin of Id:{id} was found");
        }

        if (!String.IsNullOrEmpty(updateUserDTO.Role))
        {
            admin.Role = updateUserDTO.Role!;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Name))
        {
            admin.Name = updateUserDTO.Name;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Password))
        {
            admin.Password = updateUserDTO.Password;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Email))
        {
            admin.Email = updateUserDTO.Email;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }

    [HttpDelete(template: "delete/{id:int?}")]
    public async Task<IActionResult> DeleteAdminAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Id == id);

        if (admin is null)
        {
            return NotFound($"No Admin of Id:{id} was found");
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }
}