using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route(template: "[controller]")]
public class MainController : ControllerBase
{
    public MainController()
    {

    }

    [HttpPost(template: "login")]
    public async Task<IActionResult> LoginAction()
    {
        return Ok();
    }

    [HttpPost(template: "refresh")]
    public async Task<IActionResult> RefreshAction()
    {
        return Ok();
    }
}