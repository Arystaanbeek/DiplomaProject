using DiplomaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web;


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
        // Проверяем, существует ли уже пользователь с таким же Email.
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            // Если пользователь существует, возвращаем ошибку.
            return StatusCode(StatusCodes.Status409Conflict, new { Status = "Error", Message = "User already exists!" });
        }

        // Создаем нового пользователя.
        ApplicationUser user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.Email
        };

        // Регистрируем пользователя с паролем.
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            // Если не получилось зарегистрировать пользователя, возвращаем ошибку.
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }

        return Ok(new { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        // Ищем пользователя по Email.
        var user = await _userManager.FindByEmailAsync(model.Email);
        // Проверяем пароль пользователя.
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            // Если пароль верный, генерируем JWT токен.
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, Message = "Logged in successfully" });
        }

        // Если пароль не верный, возвращаем ошибку.
        return Unauthorized(new { Status = "Error", Message = "Invalid credentials" });
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // Не раскрываем, что пользователь не существует
            return BadRequest(new { Status = "Error", Message = "Пользователь с таким email не найден." });
        }

        // Генерация токена сброса пароля
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Кодирование токена для URL
        var encodedToken = HttpUtility.UrlEncode(resetToken);

        // Здесь укажите URL вашего frontend-приложения для сброса пароля
        var frontendResetUrl = $"https://diplomawebapi.azurewebsites.net/ResetPassword?userId={user.Id}&token={encodedToken}";

        // Отправка email с ссылкой на страницу сброса пароля на frontend
        await SendEmailAsync(model.Email, "Сброс пароля",
            $"Пожалуйста, перейдите по этой ссылке для сброса вашего пароля: " +
            $"{frontendResetUrl}");

        return Ok(new { Status = "Success", Message = "Ссылка для сброса пароля отправлена на ваш email." });
    }

    private async Task SendEmailAsync(string email, string subject, string message)
    {
        using (var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
        {
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("arystaanbeek@gmail.com", "ewjy ockh tqql uhyf");

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress("arystaanbeek@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Обработка исключения или повторная попытка отправки письма
            }
        }
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return BadRequest(new { Status = "Error", Message = "Неверный запрос на сброс пароля." });
        }

        // Декодирование токена из URL
        var decodedToken = HttpUtility.UrlDecode(model.Token);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { Status = "Error", Message = "Ошибка при сбросе пароля.", Errors = result.Errors });
        }

        return Ok(new { Status = "Success", Message = "Пароль успешно сброшен." });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        // Обработчик для создания JWT токена.
        var tokenHandler = new JwtSecurityTokenHandler();
        // Получаем секретный ключ из конфигурации.
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        // Описываем как будет выглядеть наш токен.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        // Создаем токен.
        var token = tokenHandler.CreateToken(tokenDescriptor);
        // Возвращаем сериализованный в строку токен.
        return tokenHandler.WriteToken(token);
    }
}
