using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin")]
public class EmployeeController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly SmallBusinessContext _context;

    public EmployeeController(IMemoryCache cache, SmallBusinessContext context)
    {
        _cache = cache;
        _context = context;
    }

    [HttpGet(template: "list")]
    public async Task<IActionResult> ListEmployeeAction()
    {
        if (!_cache.TryGetValue("list_employee", out List<EmployeeModel>? list))
        {
            list = await _context.Employees.AsNoTracking().ToListAsync();

            if (list == null)
            {
                return NoContent();
            }

            _cache.Set("list_employee", list!, TimeSpan.FromMinutes(1));
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateEmployeeAction([FromBody] EmployeeModel employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        _cache.Remove("list_employee");

        return NoContent();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateEmployeeAction([FromBody] UpdateEmployeeModel newData)
    {
        try
        {
            var employee = await _context.Employees.FirstAsync(e => e.Id == newData.Id);

            if (!String.IsNullOrEmpty(newData.Role))
            {
                employee.Role = newData.Role;
            }
            if (!String.IsNullOrEmpty(newData.Name))
            {
                employee.Name = newData.Name;
            }
            if (!String.IsNullOrEmpty(newData.Password))
            {
                employee.Password = newData.Password;
            }
            if (!String.IsNullOrEmpty(newData.Email))
            {
                employee.Email = newData.Email;
            }
            await _context.SaveChangesAsync();

            _cache.Remove("list_employee");

            return NoContent();
        }
        catch
        {
            return NotFound($"An Employee of Id:{newData.Id} was not found");
        }
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteEmployeeAction([FromBody] int id)
    {
        try
        {
            var employee = await _context.Employees.FirstAsync(e => e.Id == id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _cache.Remove("list_employee");

            return NoContent();
        }
        catch
        {
            return NotFound($"An Employee of Id:{id} was not found");
        }
    }
}