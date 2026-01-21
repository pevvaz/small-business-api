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
            list = await _context.Clients.AsNoTracking().ToListAsync();

            _cache.Set("list_client", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateClientAction([FromBody] CreateUserDTO createUserDTO)
    {
        var client = new ContextModels.UserContextModel
        {
            Role = createUserDTO.Role,
            Name = createUserDTO.Name,
            Password = createUserDTO.Password,
            Email = createUserDTO.Email,
        };

        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        _cache.Remove("list_client");

        return Ok();
    }

    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateClientAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id, [FromBody] UpdateUserDTO updateUserDTO)
    {
        var client = await _context.Clients.SingleOrDefaultAsync(c => c.Id == id);

        if (client is null)
        {
            return NotFound($"No Client of Id:{id} was found");
        }

        if (!String.IsNullOrEmpty(updateUserDTO.Role))
        {
            client.Role = updateUserDTO.Role;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Name))
        {
            client.Name = updateUserDTO.Name;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Password))
        {
            client.Password = updateUserDTO.Password;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Email))
        {
            client.Email = updateUserDTO.Email;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_client");

        return NoContent();
    }

    [HttpDelete(template: "delete/{id:int?}")]
    public async Task<IActionResult> DeleteClientAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        var client = await _context.Clients.SingleOrDefaultAsync(c => c.Id == id);

        if (client is null)
        {
            return NotFound($"No Client of Id:{id} was found");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        _cache.Remove("list_client");

        return NoContent();
    }
}