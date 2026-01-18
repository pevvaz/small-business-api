using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin, employee")]
public class AppointmentController : ControllerBase
{
    public AppointmentController()
    {

    }
}