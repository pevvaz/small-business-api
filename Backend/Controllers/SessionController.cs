using System.ComponentModel.DataAnnotations;
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
    public async Task<IActionResult> LoginAction([FromBody] SessionDTO.LoginSessionDTO loginSessionDTO)
    {
        if (String.IsNullOrEmpty(loginSessionDTO.NameOrEmail))
        {
            return BadRequest("Name or Email in body is required");
        }

        var session = await _context.Sessions.Include(s => s.User).SingleOrDefaultAsync(s => (s.User.Name == loginSessionDTO.NameOrEmail || s.User.Email == loginSessionDTO.NameOrEmail) && s.User.Password == loginSessionDTO.Password);

        if (session is null)
        {
            return BadRequest("User not found");
        }

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _configuration["Settings:Issuer"]!,
            Audience = session.User.Role.ToString().ToLower(),
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role,session.User.Role.ToString().ToLower()),
                new Claim(ClaimTypes.Name,session.User.Name),
                new Claim(ClaimTypes.Email,session.User.Email),
                new Claim("ClaimUserId", $"{session.Id}"),
                new Claim(ClaimTypes.Hash, Encoding.UTF8.GetHashCode().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Settings:NonSuspiciousString"]!)), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        // Refresh Token
        var oldToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.UserId == session.UserId);

        var rngBytes = RandomNumberGenerator.GetBytes(45);
        var rtHash = Convert.ToBase64String(SHA256.HashData(rngBytes));
        var refreshTokenString = Convert.ToBase64String(rngBytes);

        if (oldToken is not null)
        {
            oldToken.Token = refreshTokenString;
            oldToken.Expire = DateTime.UtcNow.AddMinutes(30);
        }
        else
        {
            var refreshToken = new ContextModels.RefreshTokenContextModel
            {
                User = session.User,
                Token = rtHash,
                Expire = DateTime.UtcNow.AddMinutes(30),
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
        }
        await _context.SaveChangesAsync();

        return Ok(new
        {
            securityToken,
            refreshTokenString,
        });
    }

    [HttpPost(template: "refresh")]
    public async Task<IActionResult> RefreshAction([FromBody][Required(ErrorMessage = "RefreshToken in string is required")] string? rtString)
    {
        if (!String.IsNullOrEmpty(rtString))
        {
            return BadRequest("RefreshToken in body can't be empty");
        }

        var rtBytes = Encoding.UTF8.GetBytes(rtString!);
        var rtHash = Convert.ToBase64String(rtBytes);

        var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == rtHash);

        if (refreshToken is null)
        {
            return BadRequest("RefreshToken doesn't exist or it got deactivated");
        }

        var login = new SessionDTO.LoginSessionDTO
        {
            NameOrEmail = refreshToken!.User.Email,
            Password = refreshToken!.User.Password
        };

        return RedirectToAction(nameof(LoginAction), login);
    }
}