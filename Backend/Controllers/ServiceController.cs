using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin")]
public class ServiceController : ControllerBase
{
    private readonly SmallBusinessContext _context;

    public ServiceController(SmallBusinessContext context)
    {
        _context = context;
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateServiceAction([FromBody] ServiceModel newService)
    {
        await _context.Services.AddAsync(newService);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateServiceAction([FromBody] UpdateServiceModel newData)
    {
        var service = await _context.Services.FirstAsync(s => s.Id == newData.Id);


        if (!String.IsNullOrEmpty(newData.Name))
        {
            service.Name = newData.Name;
        }
        if (newData.Duration != null)
        {
            service.Duration = TimeSpan.FromMinutes(newData.Duration.Value);
        }
        if (newData.Price != null)
        {
            service.Price = newData.Price.Value;
        }
        if (!String.IsNullOrEmpty(newData.Status))
        {
            service.Status = newData.Status;
        }

        return NoContent();
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteServiceAction(ServiceModel service)
    {
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}