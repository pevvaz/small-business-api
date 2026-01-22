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
    [HttpGet(template: "list/all")]
    public async Task<IActionResult> ListAppointmentAllAction()
    {
        if (!_cache.TryGetValue("list_appointment_all", out List<ContextModels.AppointmentContextModel>? list))
        {
            list = await _context.Appointments.AsNoTracking().ToListAsync();

            _cache.Set("list_appointment_all", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    // [Authorize(Roles = "admin, employee")]
    [HttpGet(template: "list/employee/{id:int?}")]
    public async Task<IActionResult> ListAppointmentEmployeeAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        if (!_cache.TryGetValue($"list_appointment_employee_{id}", out List<ContextModels.AppointmentContextModel>? list))
        {
            list = await _context.Appointments.AsNoTracking().Where(a => a.HistoryEmployeeId == id).ToListAsync();

            _cache.Set($"list_appointment_employee_{id}", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    // [Authorize(Roles = "admin, employee")]
    [HttpGet(template: "list/client/{id:int?}")]
    public async Task<IActionResult> ListAppointmentClientAction([FromRoute][Required(ErrorMessage = "Id in route is required")][Range(1, int.MaxValue, ErrorMessage = "Id in route is out of range")] int? id)
    {
        if (!_cache.TryGetValue($"list_appointment_client_{id}", out List<ContextModels.AppointmentContextModel>? list))
        {
            list = await _context.Appointments.AsNoTracking().Where(a => a.HistoryClientId == id).ToListAsync();

            _cache.Set($"list_appointment_client_{id}", list, TimeSpan.FromMinutes(1));
        }

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    // [Authorize(Roles = "client")]
    [HttpGet(template: "list/mine")]
    public async Task<IActionResult> ListAppointmentMeAction()
    {
        int userId = int.Parse(User.FindFirst("ClaimUserId")!.Value);

        var list = await _context.Appointments.AsNoTracking().Where(a => a.HistoryClientId == userId).ToListAsync();

        if (list is null)
        {
            return NoContent();
        }

        return Ok(list!);
    }

    // [Authorize(Roles = "admin, client")]
    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateAppointmentAction([FromBody] AppointmentDTO.CreateAppointmentDTO createAppointmentDTO)
    {
        var employee = await _context.Employees.SingleOrDefaultAsync(e => e.Id == createAppointmentDTO.EmployeeId);
        if (employee is null)
        {
            NotFound($"No Employee of Id:{createAppointmentDTO.EmployeeId} was found");
        }

        var client = await _context.Clients.SingleOrDefaultAsync(c => c.Id == createAppointmentDTO.ClientId);
        if (client is null)
        {
            NotFound($"No Client of Id:{createAppointmentDTO.ClientId} was found");
        }
        var service = await _context.Services.SingleOrDefaultAsync(s => s.Id == createAppointmentDTO.ServiceId);
        if (service is null)
        {
            NotFound($"No Service of Id:{createAppointmentDTO.ServiceId} was found");
        }

        var appointment = new ContextModels.AppointmentContextModel
        {
            HistoryEmployeeId = employee!.Id,
            HistoryEmployeeName = employee!.Name,
            HistoryClientId = client!.Id,
            HistoryClientName = client!.Name,
            HistoryServiceId = service!.Id,
            HistoryServiceName = service!.Name,

            StartDate = createAppointmentDTO.StartDate!.Value,
            EndDate = createAppointmentDTO.StartDate!.Value.AddMinutes(service!.Duration)
        };

        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        _context.Remove("list_appointment_all");
        _context.Remove($"list_appointment_employee_{employee.Id}");
        _context.Remove($"list_appointment_client_{client.Id}");

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
            return BadRequest("Status in body is not recognized. Try one of these: 'scheduled', 'canceled', 'expired', 'done'");
        }

        if (updateAppointmentDTO.StartDate is not null)
        {
            appointment.StartDate = updateAppointmentDTO.StartDate.Value;
        }
        if (updateAppointmentDTO.EndDate is not null)
        {
            appointment.EndDate = updateAppointmentDTO.EndDate.Value;
        }
        if (updateAppointmentDTO.Status is not null)
        {
            appointment.Status = status;
        }
        if (status == ContextModels.AppointmentContextModel.EnumAppointmentStatus.Done)
        {
            appointment.Resolved = true;
        }
        else if (updateAppointmentDTO.Resolved is not null)
        {
            appointment.Resolved = updateAppointmentDTO.Resolved.Value;
        }

        await _context.SaveChangesAsync();

        _context.Remove("list_appointment_all");
        _context.Remove($"list_appointment_employee_{id}");
        _context.Remove($"list_appointment_client_{id}");

        return NoContent();
    }
}