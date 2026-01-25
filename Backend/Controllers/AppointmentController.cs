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
    public async Task<IActionResult> ListAppointmentAction(int? employee, int? client, int? service, string? status, bool? resolved)
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
        if (status is not null)
        {
            if (Enum.TryParse(status, true, out ContextModels.AppointmentContextModel.EnumAppointmentStatus enumStatus))
            {
                query = query.Where(a => a.Status == enumStatus);
            }
        }
        if (resolved is not null)
        {
            query = query.Where(a => a.Resolved == resolved);
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
    public async Task<IActionResult> ListAppointmentMeAction()
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

        var appointment = new ContextModels.AppointmentContextModel
        {
            EmployeeId = employee!.Id,
            HistoryEmployeeName = employee!.Name,
            ClientId = client!.Id,
            HistoryClientName = client!.Name,
            ServiceId = service!.Id,
            HistoryServiceName = service!.Name,

            StartDate = createAppointmentDTO.StartDate!.Value,
            EndDate = createAppointmentDTO.StartDate!.Value.AddMinutes(service!.Duration)
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
        if (appointment.Resolved == true)
        {
            return Conflict($"Appointment of Id:{id} is resolved. It can't be customized anymore");
        }

        if (!Enum.TryParse(updateAppointmentDTO.Status, true, out ContextModels.AppointmentContextModel.EnumAppointmentStatus status))
        {
            return BadRequest("Status in body is not recognized. Try one of these: 'Scheduled', 'Cancelled', 'Expired', 'Done'");
        }

        if (updateAppointmentDTO.StartDate is not null)
        {
            var duration = appointment.EndDate - appointment.StartDate;

            appointment.StartDate = updateAppointmentDTO.StartDate.Value;

            appointment.EndDate = appointment.StartDate.Add(duration);
        }
        if (!String.IsNullOrEmpty(updateAppointmentDTO.Status))
        {
            appointment.Status = status;
        }

        if (status == ContextModels.AppointmentContextModel.EnumAppointmentStatus.Done || status == ContextModels.AppointmentContextModel.EnumAppointmentStatus.Cancelled || (updateAppointmentDTO.Resolved != null && updateAppointmentDTO.Resolved != false))
        {
            appointment.Resolved = true;
        }

        await _context.SaveChangesAsync();

        _cache.Remove("list_appointment_all");
        _cache.Remove($"list_appointment_employee_{id}");
        _cache.Remove($"list_appointment_client_{id}");

        return NoContent();
    }
}