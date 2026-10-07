using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
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

    // =========================================================================================
    // [BƯỚC 4 - BACKEND CONTROLLER]: Tiếp nhận HTTP POST "api/Auth/login" từ ApiService
    // Luồng rẽ thành 2 nhánh:
    //   - Nhánh 1: Nếu là Admin -> Kiểm tra từ appsettings.json (không vào DB).
    //   - Nhánh 2: Nếu là Staff/Lecturer -> Gọi sang tầng Repository: ISystemAccountRepository.Authenticate(...)
    // =========================================================================================
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var normalizedEmail = request.Email.Trim().ToLower();

        // 1. [NHÁNH 1 - ADMIN]: Kiểm tra Admin Account trực tiếp từ file appsettings.json
        var adminEmail = _configuration["AdminAccount:Email"]?.Trim().ToLower();
        var adminPassword = _configuration["AdminAccount:Password"];

        if (!string.IsNullOrEmpty(adminEmail) && normalizedEmail == adminEmail && request.Password == adminPassword)
        {
            return Ok(new LoginResponse
            {
                AccountId = 0,
                AccountName = "Administrator",
                AccountEmail = _configuration["AdminAccount:Email"]!,
                AccountRole = 0, // 0 chỉ định quyền Admin
                RoleName = "Admin"
            });
        }

        // 2. [NHÁNH 2 - STAFF / LECTURER]: Gọi sang SystemAccountRepository.Authenticate(...)
        // Tiếp theo: SystemAccountRepository sẽ gọi SystemAccountDAO.Instance.Authenticate(...)
        var user = _accountRepository.Authenticate(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Ánh xạ mã Role trong DB: 1 -> Staff, 2 -> Lecturer
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

    // =========================================================================================
    // [TRUY VẤN ODATA CHO AUTH / HỆ THỐNG]:
    // Endpoint: GET api/Auth/users
    // Trả về danh sách tài khoản đã xác thực trong hệ thống (gồm Admin trong appsettings.json và Staff/Lecturer trong DB)
    // Hỗ trợ đầy đủ OData: $filter, $select, $orderby, $top, $skip
    // =========================================================================================
    [HttpGet("users")]
    [EnableQuery]
    public ActionResult<IEnumerable<LoginResponse>> GetAuthUsers()
    {
        var users = new List<LoginResponse>();

        // 1. Lấy tài khoản Admin từ appsettings.json
        var adminEmail = _configuration["AdminAccount:Email"];
        if (!string.IsNullOrEmpty(adminEmail))
        {
            users.Add(new LoginResponse
            {
                AccountId = 0,
                AccountName = "Administrator",
                AccountEmail = adminEmail,
                AccountRole = 0,
                RoleName = "Admin"
            });
        }

        // 2. Lấy danh sách tài khoản trong cơ sở dữ liệu (Staff / Lecturer)
        var dbAccounts = _accountRepository.GetAccounts();
        users.AddRange(dbAccounts.Select(a => new LoginResponse
        {
            AccountId = a.AccountId,
            AccountName = a.AccountName ?? string.Empty,
            AccountEmail = a.AccountEmail,
            AccountRole = a.AccountRole ?? 1,
            RoleName = a.AccountRole switch
            {
                1 => "Staff",
                2 => "Lecturer",
                _ => "User"
            }
        }));

        return Ok(users);
    }
}
