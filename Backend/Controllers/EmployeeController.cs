using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
// [Authorize(Roles = "admin")]
[Route(template: "[controller]")]
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
        if (!_cache.TryGetValue("list_employee", out List<ContextModels.UserContextModel>? list))
        {
            list = await _context.Employees.AsNoTracking().ToListAsync();

            _cache.Set("list_employee", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateEmployeeAction([FromBody] CreateUserDTO createUserDTO)
    {
        var employee = new ContextModels.UserContextModel
        {
            Role = createUserDTO.Role,
            Name = createUserDTO.Name,
            Password = createUserDTO.Password,
            Email = createUserDTO.Email
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        _cache.Remove("list_employee");

        return NoContent();
    }

    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateEmployeeAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in rout is out of range")] int? id, [FromBody] UpdateUserDTO updateUserDTO)
    {
        var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == id);

        if (employee is null)
        {
            return NotFound($"No Employee of Id:{id} was found");
        }

        if (!String.IsNullOrEmpty(updateUserDTO.Role))
        {
            employee.Role = updateUserDTO.Role;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Name))
        {
            employee.Name = updateUserDTO.Name;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Password))
        {
            employee.Password = updateUserDTO.Password;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Email))
        {
            employee.Email = updateUserDTO.Email;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_employee");

        return NoContent();
    }

    [HttpDelete(template: "delete/{id:int?}")]
    public async Task<IActionResult> DeleteEmployeeAction([FromBody][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == id);

        if (employee is null)
        {
            return NotFound($"No Employee of Id:{id} was found");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        _cache.Remove("list_employee");

        return NoContent();
    }
}