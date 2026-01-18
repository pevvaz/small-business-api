using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin")]
public class EmployeeController : ControllerBase
{
    private readonly SmallBusinessContext _context;

    public EmployeeController(SmallBusinessContext context)
    {
        _context = context;
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateEmployeeAction([FromBody] EmployeeModel employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateEmployeeAction([FromBody] EmployeeModel newEmployee)
    {
        var oldEmployee = await _context.Employees.FirstAsync(e => e.Id == newEmployee.Id);
        oldEmployee.Id = newEmployee.Id;
        oldEmployee.Role = newEmployee.Role;
        oldEmployee.Name = newEmployee.Name;
        oldEmployee.Password = newEmployee.Password;
        oldEmployee.Email = newEmployee.Email;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteEmployeeAction([FromBody] EmployeeModel employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return Ok();
    }
}