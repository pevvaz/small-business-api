using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Authorize]
[Route(template: "[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IMemoryCache _cache;
    private readonly SmallBusinessContext _context;
    public AppointmentController(IMemoryCache cache, SmallBusinessContext context)
    {
        _cache = cache;
        _context = context;
    }

    [Authorize(Roles = "admin, client")]
    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateAppointmentAction([FromBody] AppointmentModel appointment)
    {
        var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == appointment.Employee);
        var client = await _context.Clients.SingleOrDefaultAsync(c => c.Id == appointment.Client);
        var service = await _context.Services.SingleOrDefaultAsync(s => s.Id == appointment.Service);

        if (employee is null)

            appointment.EndDate = appointment.StartDate.AddMinutes(service!.Duration);


        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        _context.Remove("list_appointment");

        return NoContent();


        return BadRequest("At least one Id doesn't exist");


        return NoContent();
    }

    [Authorize(Roles = "admin")]
    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateAppointmentAction([FromBody] UpdateAppointmentModel newData)
    {
        return NoContent();
    }
}