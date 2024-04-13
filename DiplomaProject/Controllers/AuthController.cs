using DiplomaProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        // Проверяем, существует ли уже пользователь с таким Email
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Status = "Error", Message = "User already exists!" });
        }

        // Создаём нового пользователя
        ApplicationUser user = new ApplicationUser()
        {
            Email = model.Email,
            UserName = model.Email
        };

        // Регистрируем пользователя с паролем
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }

        return Ok(new { Status = "Success", Message = "User created successfully!" });
    }

    /*[HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        // Пытаемся найти пользователя по Email
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            // Генерируем JWT токен для пользователя
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, Message = "Logged in successfully" });
        }

        return Unauthorized(new { Status = "Error", Message = "Invalid credentials" });
    }*/

    // ИЗМЕНИТЕ МЕТОД С POST НА GET
    [HttpGet("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Пытаемся найти пользователя по Email
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            // Генерируем JWT токен для пользователя
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, Message = "Logged in successfully" });
        }

        return Unauthorized(new { Status = "Error", Message = "Invalid credentials" });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
                
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [HttpGet("admin-data")]
    [Authorize(Policy = "RequireAdministratorRole")]
    public IActionResult GetAdminData()
    {
        // Вернуть данные администратора
        return Ok("Секретные данные администратора");
    }

    [HttpGet("user-data")]
    [Authorize]
    public IActionResult GetUserData()
    {
        // Вернуть данные пользователя
        return Ok("Данные пользователя");
    }
}
