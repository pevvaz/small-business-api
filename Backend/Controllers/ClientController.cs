using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
// [Authorize(Roles = "admin, employee")]
[Route(template: "[controller]")]
public class ClientController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly SmallBusinessContext _context;

    public ClientController(IMemoryCache cache, SmallBusinessContext context)
    {
        _cache = cache;
        _context = context;
    }

    [HttpGet(template: "list")]
    public async Task<IActionResult> ListClientAction()
    {
        if (!_cache.TryGetValue("list_client", out List<ContextModels.UserContextModel>? list))
        {
            list = await _context.Users.Where(u => u.Role == ContextModels.UserContextModel.EnumUserRoles.Client).AsNoTracking().ToListAsync();

            _cache.Set("list_client", list, TimeSpan.FromMinutes(1));
        }

        if (!list!.Any())
        {
            return NoContent();
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateClientAction([FromBody] UserDTO.CreateUserDTO createUserDTO)
    {
        if (await _context.Sessions.AnyAsync(s => s.User.Password == createUserDTO.Password || s.User.Email == createUserDTO.Email))
        {
            return BadRequest("Password or Email in body is already in use");
        }

        var client = new ContextModels.UserContextModel
        {
            Role = ContextModels.UserContextModel.EnumUserRoles.Client,
            Name = createUserDTO.Name,
            Password = createUserDTO.Password,
            Email = createUserDTO.Email,
        };

        await _context.Users.AddAsync(client);

        var session = new ContextModels.SessionContextModel
        {
            User = client
        };

        await _context.Sessions.AddAsync(session);

        await _context.SaveChangesAsync();

        _cache.Remove("list_client");

        return Ok();
    }

    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateClientAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id, [FromBody] UserDTO.UpdateUserDTO updateUserDTO)
    {
        var client = await _context.Users.Where(u => u.Role == ContextModels.UserContextModel.EnumUserRoles.Client).SingleOrDefaultAsync(c => c.Id == id);

        if (client is null)
        {
            return NotFound($"No Client of Id:{id} was found");
        }

        if (!String.IsNullOrEmpty(updateUserDTO.Name))
        {
            client.Name = updateUserDTO.Name;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Password))
        {
            if (await _context.Sessions.AnyAsync(s => s.User.Password == updateUserDTO.Password && s.User.Id != client.Id))
            {
                return BadRequest("Password is already in use");
            }

            client.Password = updateUserDTO.Password;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Email))
        {
            if (await _context.Sessions.AnyAsync(s => s.User.Email == updateUserDTO.Email && s.User.Id != client.Id))
            {
                return BadRequest("Email is already in use");
            }

            client.Email = updateUserDTO.Email;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_client");

        return NoContent();
    }

    [HttpDelete(template: "delete/{id:int?}")]
    public async Task<IActionResult> DeleteClientAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        var client = await _context.Users.Where(u => u.Role == ContextModels.UserContextModel.EnumUserRoles.Client).SingleOrDefaultAsync(c => c.Id == id);

        if (client is null)
        {
            return NotFound($"No Client of Id:{id} was found");
        }

        _context.Users.Remove(client);
        await _context.SaveChangesAsync();

        _cache.Remove("list_client");

        return NoContent();
    }
}