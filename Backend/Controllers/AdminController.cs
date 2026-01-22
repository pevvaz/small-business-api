using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
// [Authorize(Roles = "admin")]
[Route(template: "[controller]")]
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
            list = await _context.Users.Where(u => u.Role == "admin").AsNoTracking().ToListAsync();

            _cache.Set("list_admin", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateAdminAction([FromBody] UserDTO.CreateUserDTO createUserDTO)
    {
        if (await _context.Sessions.AnyAsync(s => s.User.Password == createUserDTO.Password || s.User.Email == createUserDTO.Email))
        {
            return BadRequest("Password or Email in body is already used");
        }

        var admin = new ContextModels.UserContextModel
        {
            Role = "admin",
            Name = createUserDTO.Name,
            Password = createUserDTO.Password,
            Email = createUserDTO.Email
        };

        await _context.Users.AddAsync(admin);

        var session = new ContextModels.SessionContextModel
        {
            User = admin,
        };

        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }

    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateAdminAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id, [FromBody] UserDTO.UpdateUserDTO updateUserDTO)
    {
        var admin = await _context.Users.SingleOrDefaultAsync(a => a.Id == id);

        if (admin is null)
        {
            return NotFound($"No Admin of Id:{id} was found");
        }

        if (!String.IsNullOrEmpty(updateUserDTO.Name))
        {
            admin.Name = updateUserDTO.Name;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Password))
        {
            if (await _context.Sessions.AnyAsync(s => s.User.Password == updateUserDTO.Password && s.UserId != admin.Id))
            {
                return BadRequest("Password is already in use");
            }

            admin.Password = updateUserDTO.Password;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Email))
        {
            if (await _context.Sessions.AnyAsync(s => s.User.Email == updateUserDTO.Email && s.UserId != admin.Id))
            {
                return BadRequest("Email is already in use");
            }

            admin.Email = updateUserDTO.Email;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }

    [HttpDelete(template: "delete/{id:int?}")]
    public async Task<IActionResult> DeleteAdminAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        var admin = await _context.Users.SingleOrDefaultAsync(a => a.Id == id);

        if (admin is null)
        {
            return NotFound($"No Admin of Id:{id} was found");
        }

        _context.Users.Remove(admin);
        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return NoContent();
    }
}