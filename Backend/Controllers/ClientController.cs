using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin, employee")]
public class ClientController : ControllerBase
{
    private readonly SmallBusinessContext _context;

    public ClientController(SmallBusinessContext context)
    {
        _context = context;
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateClientAction([FromBody] ClientModel client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateClientAction([FromBody] ClientModel newClient)
    {
        var oldClient = await _context.Clients.FirstAsync(c => c.Id == newClient.Id);
        oldClient.Id = newClient.Id;
        oldClient.Role = newClient.Role;
        oldClient.Name = newClient.Name;
        oldClient.Password = newClient.Password;
        oldClient.Email = newClient.Email;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteClientAction([FromBody] ClientModel client)
    {
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return Ok();
    }
}