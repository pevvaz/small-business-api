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
            list = await _context.Users.Where(u => u.Role == "employee").AsNoTracking().ToListAsync();

            _cache.Set("list_employee", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateEmployeeAction([FromBody] UserDTO.CreateUserDTO createUserDTO)
    {
        if (await _context.Sessions.AnyAsync(s => s.User.Password == createUserDTO.Password || s.User.Email == createUserDTO.Email))
        {
            return BadRequest("Passowrd or Email is already in use");
        }

        var employee = new ContextModels.UserContextModel
        {
            Role = "employee",
            Name = createUserDTO.Name,
            Password = createUserDTO.Password,
            Email = createUserDTO.Email
        };

        await _context.Users.AddAsync(employee);

        var session = new ContextModels.SessionContextModel
        {
            User = employee
        };

        await _context.Sessions.AddAsync(session);

        await _context.SaveChangesAsync();

        _cache.Remove("list_employee");

        return NoContent();
    }

    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateEmployeeAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in rout is out of range")] int? id, [FromBody] UserDTO.UpdateUserDTO updateUserDTO)
    {
        var employee = await _context.Users.SingleOrDefaultAsync(e => e.Id == id);

        if (employee is null)
        {
            return NotFound($"No Employee of Id:{id} was found");
        }

        if (!String.IsNullOrEmpty(updateUserDTO.Name))
        {
            employee.Name = updateUserDTO.Name;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Password))
        {
            if (await _context.Sessions.AnyAsync(s => s.User.Password == updateUserDTO.Password && s.User.Id != employee.Id))
            {
                return BadRequest("Password is already in use");
            }

            employee.Password = updateUserDTO.Password;
        }
        if (!String.IsNullOrEmpty(updateUserDTO.Email))
        {
            if (await _context.Sessions.AnyAsync(s => s.User.Email == updateUserDTO.Email && s.User.Id != employee.Id))
            {
                return BadRequest("Email is already in use");
            }

            employee.Email = updateUserDTO.Email;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_employee");

        return NoContent();
    }

    [HttpDelete(template: "delete/{id:int?}")]
    public async Task<IActionResult> DeleteEmployeeAction([FromBody][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        var employee = await _context.Users.SingleOrDefaultAsync(e => e.Id == id);

        if (employee is null)
        {
            return NotFound($"No Employee of Id:{id} was found");
        }

        _context.Users.Remove(employee);
        await _context.SaveChangesAsync();

        _cache.Remove("list_employee");

        return NoContent();
    }
}