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
        if (!_cache.TryGetValue("list_admin", out List<AdminModel>? list))
        {
            list = await _context.Admins.AsNoTracking().ToListAsync();

            _cache.Set("list_admin", list!, TimeSpan.FromMinutes(1));

            if (list == null)
            {
                return NoContent();
            }
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateAdminAction([FromBody] AdminModel newAdmin)
    {
        await _context.Admins.AddAsync(newAdmin);
        await _context.SaveChangesAsync();

        _cache.Remove("list_admin");

        return Ok();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateAdminAction([FromBody] UpdateAdminModel newData)
    {
        try
        {
            var admin = await _context.Admins.FirstAsync(a => a.Id == newData.Id);

            if (!String.IsNullOrEmpty(newData.Role))
            {
                admin.Role = newData.Role!;
            }
            if (!String.IsNullOrEmpty(newData.Name))
            {
                admin.Name = newData.Name;
            }
            if (!String.IsNullOrEmpty(newData.Password))
            {
                admin.Password = newData.Password;
            }
            if (!String.IsNullOrEmpty(newData.Email))
            {
                admin.Email = newData.Email;
            }

            await _context.SaveChangesAsync();

            _cache.Remove("list_admin");

            return NoContent();
        }
        catch
        {
            return NotFound($"An Admin of Id:{newData.Id} was not found");
        }
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteAdminAction([FromBody] int id)
    {
        try
        {
            var admin = await _context.Admins.FirstAsync(a => a.Id == id);
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            _cache.Remove("list_admin");

            return NoContent();
        }
        catch
        {
            return NotFound($"An Admin of Id:{id} was not found");
        }
    }
}