using Microsoft.AspNetCore.Mvc;
using _18_MaiDoanTrungKien_BackEnd.DTOs;
using _18_MaiDoanTrungKien_BackEnd.Repositories;

namespace _18_MaiDoanTrungKien_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISystemAccountRepository _accountRepository;
    private readonly IConfiguration _configuration;

    public AuthController(ISystemAccountRepository accountRepository, IConfiguration configuration)
    {
        _accountRepository = accountRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var normalizedEmail = request.Email.Trim().ToLower();

        // 1. Check Admin Account from appsettings.json
        var adminEmail = _configuration["AdminAccount:Email"]?.Trim().ToLower();
        var adminPassword = _configuration["AdminAccount:Password"];

        if (!string.IsNullOrEmpty(adminEmail) && normalizedEmail == adminEmail && request.Password == adminPassword)
        {
            return Ok(new LoginResponse
            {
                AccountId = 0,
                AccountName = "Administrator",
                AccountEmail = _configuration["AdminAccount:Email"]!,
                AccountRole = 0, // 0 indicates Admin
                RoleName = "Admin"
            });
        }

        // 2. Check Database System Accounts (Staff / Lecturer)
        var user = _accountRepository.Authenticate(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var roleName = user.AccountRole switch
        {
            1 => "Staff",
            2 => "Lecturer",
            _ => "User"
        };

        return Ok(new LoginResponse
        {
            AccountId = user.AccountId,
            AccountName = user.AccountName ?? string.Empty,
            AccountEmail = user.AccountEmail,
            AccountRole = user.AccountRole ?? 1,
            RoleName = roleName
        });
    }

    [HttpGet("profile/{accountId}")]
    public IActionResult GetProfile(short accountId)
    {
        if (accountId == 0)
        {
            return Ok(new
            {
                AccountId = 0,
                AccountName = "Administrator",
                AccountEmail = _configuration["AdminAccount:Email"],
                RoleName = "Admin"
            });
        }

        var user = _accountRepository.GetAccountById(accountId);
        if (user == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        return Ok(new
        {
            user.AccountId,
            user.AccountName,
            user.AccountEmail,
            user.AccountRole,
            RoleName = user.AccountRole == 1 ? "Staff" : "Lecturer"
        });
    }
}
