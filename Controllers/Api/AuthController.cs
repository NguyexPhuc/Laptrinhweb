using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web_BanHang.Models;
using Microsoft.AspNetCore.Authorization;

namespace Web_BanHang.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _cfg;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration cfg)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _cfg = cfg;
    }

    // Cookie-based login (for SPA use fetch with credentials: 'include')
    [HttpPost("login-cookie")]
    public async Task<IActionResult> LoginCookie([FromBody] LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, dto.Remember, lockoutOnFailure: true);
        if (result.Succeeded) return Ok();
        if (result.IsLockedOut) return Forbid("LockedOut");
        return Unauthorized();
    }

    // JWT token issuance
    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null) return Unauthorized();
        if (!await _userManager.CheckPasswordAsync(user, dto.Password)) return Unauthorized();
        if (!user.EmailConfirmed) return Forbid("EmailNotConfirmed");

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        foreach (var r in roles) claims.Add(new Claim(ClaimTypes.Role, r));

        var jwtSection = _cfg.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key");
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");
        var expires = jwtSection.GetValue<int>("ExpireMinutes");

        var keyBytes = Encoding.UTF8.GetBytes(key ?? "");
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expires > 0 ? expires : 60),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
        );
        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { access_token = tokenStr, expires_in = (int)TimeSpan.FromMinutes(expires > 0 ? expires : 60).TotalSeconds });
    }

    [Authorize(AuthenticationSchemes = "Bearer,Identity.Application")]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new { username = user.UserName, email = user.Email, roles = roles });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Username, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        await _userManager.AddToRoleAsync(user, "Customer");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // Send email (optional): consumer can call /api/auth/confirm

        return Ok();
    }
}

public record LoginDto(string Username, string Password, bool Remember = false);
public record RegisterDto(string Username, string Email, string Password);