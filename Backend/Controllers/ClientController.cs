using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin, employee")]
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
        if (!_cache.TryGetValue("list_client", out List<ClientModel>? list))
        {
            list = await _context.Clients.AsNoTracking().ToListAsync();

            _cache.Set("list_client", list!, TimeSpan.FromMinutes(1));

            if (list == null)
            {
                return NoContent();
            }
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateClientAction([FromBody] ClientModel client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        _cache.Remove("list_client");

        return Ok();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateClientAction([FromBody] UpdateClientModel newData)
    {
        try
        {
            var client = await _context.Clients.FirstAsync(c => c.Id == newData.Id);

            if (!String.IsNullOrEmpty(newData.Role))
            {
                client.Role = newData.Role;
            }
            if (!String.IsNullOrEmpty(newData.Name))
            {
                client.Name = newData.Name;
            }
            if (!String.IsNullOrEmpty(newData.Password))
            {
                client.Password = newData.Password;
            }
            if (!String.IsNullOrEmpty(newData.Email))
            {
                client.Email = newData.Email;
            }

            await _context.SaveChangesAsync();

            _cache.Remove("list_client");

            return NoContent();
        }
        catch
        {
            return NotFound($"A Client of Id:{newData.Id} was not found");
        }
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteClientAction([FromBody] int id)
    {
        try
        {
            var client = await _context.Clients.FirstAsync(c => c.Id == id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            _cache.Remove("list_client");

            return NoContent();
        }
        catch
        {
            return NotFound($"A Client of Id:{id} was not found");
        }
    }
}