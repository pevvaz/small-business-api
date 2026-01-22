using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
// [Authorize(Roles = "admin")]
[Route(template: "[controller]")]
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
        if (!_cache.TryGetValue("list_service", out List<ContextModels.ServiceContextModel>? list))
        {
            list = await _context.Services.AsNoTracking().ToListAsync();

            _cache.Set("list_service", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateServiceAction([FromBody] ServiceDTO.CreateServiceDTO createServiceDTO)
    {
        var service = new ContextModels.ServiceContextModel
        {
            Name = createServiceDTO.Name,
            Price = createServiceDTO.Price!.Value,
            Duration = createServiceDTO.Duration!.Value,
            Status = createServiceDTO.Status
        };

        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();

        _cache.Remove("list_service");

        return NoContent();
    }

    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateServiceAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id, [FromBody] ServiceDTO.UpdateServiceDTO updateServiceDTO)
    {
        var service = await _context.Services.SingleOrDefaultAsync(s => s.Id == id);

        if (service is null)
        {
            return NotFound($"No Service of Id:{id} was found");
        }

        if (!String.IsNullOrEmpty(updateServiceDTO.Name))
        {
            service.Name = updateServiceDTO.Name;
        }
        if (updateServiceDTO.Price is not null)
        {
            service.Price = updateServiceDTO.Price.Value;
        }
        if (updateServiceDTO.Duration is not null)
        {
            service.Duration = updateServiceDTO.Duration.Value;
        }
        if (!String.IsNullOrEmpty(updateServiceDTO.Status))
        {
            service.Status = updateServiceDTO.Status;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_service");

        return NoContent();
    }

    [HttpDelete(template: "delete/{id:int?}")]
    public async Task<IActionResult> DeleteServiceAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        var service = await _context.Services.SingleOrDefaultAsync(s => s.Id == id);

        if (service is null)
        {
            return NotFound($"No Service of Id:{id} was found");
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        _cache.Remove("list_service");

        return NoContent();
    }
}