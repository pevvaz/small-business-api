using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route(template: "[controller]")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly SmallBusinessContext _context;

    public AdminController(SmallBusinessContext context)
    {
        _context = context;
    }

    [HttpPost(template: "create")]
    public async Task<IActionResult> CreateAdminAction([FromBody] AdminModel newAdmin)
    {
        await _context.Admins.AddAsync(newAdmin);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut(template: "update")]
    public async Task<IActionResult> UpdateAdminAction()
    {
        return Ok();
    }

    [HttpDelete(template: "delete")]
    public async Task<IActionResult> DeleteAdminAction()
    {
        return Ok();
    }
}