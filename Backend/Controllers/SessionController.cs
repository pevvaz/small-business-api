using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[AllowAnonymous]
[Route(template: "[controller]")]
public class SessionController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly SmallBusinessContext _context;

    public SessionController(IConfiguration configuration, IMemoryCache cache, SmallBusinessContext context)
    {
        _configuration = configuration;
        _cache = cache;
        _context = context;
    }

    [HttpPost(template: "login")]
    public async Task<IActionResult> LoginAction([FromBody] LoginSessionDTO loginSessionDTO)
    {
        var user = await _context.Users.Include(u => u.User).SingleOrDefaultAsync(u => (u.User.Name == loginSessionDTO.Name || u.User.Email == loginSessionDTO.Email) && u.User.Password == loginSessionDTO.Password);

        if (user is null)
        {
            return BadRequest("Role in body doesn't exist");
        }

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _configuration["Settings:Issuer"]!,
            Audience = user.User.Role.ToLower(),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, user.User.Role.ToLower()),
                new Claim(ClaimTypes.Name, user.User.Name),
                new Claim(ClaimTypes.Email, user.User.Email),
                new Claim("ClaimUserId", $"{user.Id}"),
                new Claim(ClaimTypes.Hash, Encoding.UTF8.GetHashCode().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Settings:Secret"]!)), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        return Ok(token);
    }

    [HttpPost(template: "refresh")]
    public async Task<IActionResult> RefreshAction()
    {
        return Ok();
    }
}