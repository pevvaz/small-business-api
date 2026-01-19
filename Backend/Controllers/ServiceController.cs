using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin")]
public class ServiceController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly SmallBusinessContext _context;

    public ServiceController(IMemoryCache cache, SmallBusinessContext context)
    {
        _cache = cache;
        _context = context;
    }

    [HttpGet(template: "list")]
    public async Task<IActionResult> ListServiceAction()
    {
        if (!_cache.TryGetValue("list_service", out List<ServiceModel>? list))
        {
            list = await _context.Services.AsNoTracking().ToListAsync();

            if (list == null)
            {
                return NoContent();
            }

            _cache.Set("list_service", list!, TimeSpan.FromMinutes(1));
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateServiceAction([FromBody] ServiceModel newService)
    {
        await _context.Services.AddAsync(newService);
        await _context.SaveChangesAsync();

        _cache.Remove("list_service");

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

        _cache.Remove("list_service");

        return NoContent();
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteServiceAction([FromBody] int id)
    {
        try
        {
            var deletedService = await _context.Services.FirstAsync(s => s.Id == id);
            _context.Services.Remove(deletedService);
            await _context.SaveChangesAsync();

            _cache.Remove("list_service");

            return NoContent();
        }
        catch
        {
            return NotFound($"A Service of Id:{id} was not found");
        }
    }
}