using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route(template: "[controller]")]
public class MainController : ControllerBase
{
    private readonly SmallBusinessContext _context;

    public MainController(SmallBusinessContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpPost(template: "login")]
    public async Task<IActionResult> LoginAction()
    {
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost(template: "refresh")]
    public async Task<IActionResult> RefreshAction()
    {
        return Ok();
    }
}