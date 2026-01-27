using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
// [Authorize]
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

    // [Authorize(Roles = "admin")]
    [HttpGet(template: "list")]
    public async Task<IActionResult> ListAppointmentAction(int? employee, int? client, int? service, string? status)
    {
        var query = _context.Appointments.AsQueryable();

        if (employee is not null)
        {
            query = query.Where(a => a.EmployeeId == employee);
        }
        if (client is not null)
        {
            query = query.Where(a => a.ClientId == client);
        }
        if (service is not null)
        {
            query = query.Where(a => a.ServiceId == service);
        }
        if (!String.IsNullOrEmpty(status))
        {
            if (Enum.TryParse(status, true, out ContextModels.AppointmentContextModel.EnumAppointmentStatus enumStatus))
            {
                query = query.Where(a => a.Status == enumStatus);
            }
        }

        var list = await query.ToListAsync();

        if (!list.Any())
        {
            return NoContent();
        }

        return Ok(list);
    }

    // [Authorize(Roles = "client")]
    [HttpGet(template: "list/mine")]
    public async Task<IActionResult> ListAppointmentMeAction() // TEST
    {
        int userId = int.Parse(User.FindFirst("ClaimUserId")!.Value);

        if (!_cache.TryGetValue($"listme_{userId}", out List<ContextModels.AppointmentContextModel>? list))
        {
            list = await _context.Appointments.AsNoTracking().Where(a => a.ClientId == userId).ToListAsync();

            _cache.Set($"listme_{userId}", list, TimeSpan.FromMinutes(1));
        }

        if (!list!.Any())
        {
            return NoContent();
        }

        return Ok(list!);
    }

    // [Authorize(Roles = "admin, client")]
    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateAppointmentAction([FromBody] AppointmentDTO.CreateAppointmentDTO createAppointmentDTO)
    {
        var employee = await _context.Users.Where(u => u.Role == ContextModels.UserContextModel.EnumUserRoles.Employee).AsNoTracking().SingleOrDefaultAsync(e => e.Id == createAppointmentDTO.EmployeeId);
        if (employee is null)
        {
            return NotFound($"No Employee of Id:{createAppointmentDTO.EmployeeId} was found");
        }

        var client = await _context.Users.Where(u => u.Role == ContextModels.UserContextModel.EnumUserRoles.Client).AsNoTracking().SingleOrDefaultAsync(c => c.Id == createAppointmentDTO.ClientId);
        if (client is null)
        {
            return NotFound($"No Client of Id:{createAppointmentDTO.ClientId} was found");
        }

        var service = await _context.Services.AsNoTracking().SingleOrDefaultAsync(s => s.Id == createAppointmentDTO.ServiceId);
        if (service is null)
        {
            return NotFound($"No Service of Id:{createAppointmentDTO.ServiceId} was found");
        }
        if (service.Status == ContextModels.ServiceContextModel.EnumServiceStatus.Deactive)
        {
            return BadRequest("Only 'Active' Services can be assigned to an Appointment");
        }

        if (createAppointmentDTO.StartDate < DateTime.UtcNow)
        {
            return BadRequest("Start Date can't be before now");
        }
        if (await _context.Appointments.AnyAsync(a => a.StartDate <= createAppointmentDTO.StartDate && a.EndDate >= createAppointmentDTO.StartDate && (a.EmployeeId == employee.Id || a.ClientId == client.Id)))
        {
            return BadRequest($"Employee or Client is already assigned to an Appointment around this time");
        }

        var appointment = new ContextModels.AppointmentContextModel
        {
            EmployeeId = employee.Id,
            HistoryEmployeeName = employee.Name,
            ClientId = client.Id,
            HistoryClientName = client.Name,
            ServiceId = service.Id,
            HistoryServiceName = service.Name,
            StartDate = createAppointmentDTO.StartDate!.Value,
            EndDate = createAppointmentDTO.StartDate.Value.AddMinutes(service!.Duration)
        };

        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        _cache.Remove("list_appointment_all");
        _cache.Remove($"list_appointment_employee_{employee.Id}");
        _cache.Remove($"list_appointment_client_{client.Id}");

        return NoContent();
    }

    // [Authorize(Roles = "admin, employee")]
    [HttpPut(template: "update/{id:int?}")]
    public async Task<IActionResult> UpdateAppointmentAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id, [FromBody] AppointmentDTO.UpdateAppointmentDTO updateAppointmentDTO)
    {
        var appointment = await _context.Appointments.SingleOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
        {
            return NotFound($"No Appointment of Id:{id} was found");
        }

        if (!Enum.TryParse(updateAppointmentDTO.Status, true, out ContextModels.AppointmentContextModel.EnumAppointmentStatus status))
        {
            return BadRequest("Status in body is not recognized. Try one of these: 'Scheduled', 'Cancelled', 'Expired', 'Done'");
        }
        else
        {
            appointment.Status = status;
        }

        if (updateAppointmentDTO.StartDate is not null)
        {
            if (updateAppointmentDTO.StartDate < DateTime.UtcNow)
            {
                return BadRequest("Start Date can't be before now");
            }

            var duration = appointment.EndDate - appointment.StartDate;

            appointment.StartDate = updateAppointmentDTO.StartDate.Value;

            appointment.EndDate = appointment.StartDate.Add(duration);
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_appointment_all");
        _cache.Remove($"list_appointment_employee_{id}");
        _cache.Remove($"list_appointment_client_{id}");

        return NoContent();
    }
}