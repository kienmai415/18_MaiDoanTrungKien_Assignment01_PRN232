using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using _18_MaiDoanTrungKien_BackEnd.DTOs;
using _18_MaiDoanTrungKien_BackEnd.Models;
using _18_MaiDoanTrungKien_BackEnd.Repositories;

namespace _18_MaiDoanTrungKien_BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ISystemAccountRepository _accountRepository;

    public AccountsController(ISystemAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpGet]
    [EnableQuery]
    public ActionResult<IEnumerable<SystemAccount>> GetAccounts()
    {
        var accounts = _accountRepository.GetAccounts();
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public ActionResult<SystemAccount> GetAccount(short id)
    {
        var account = _accountRepository.GetAccountById(id);
        if (account == null)
        {
            return NotFound(new { message = $"Account with ID {id} not found." });
        }
        return Ok(account);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<SystemAccount>> Search([FromQuery] string? keyword)
    {
        var accounts = _accountRepository.GetAccounts().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            accounts = accounts.Where(a =>
                (a.AccountName != null && a.AccountName.ToLower().Contains(term)) ||
                (a.AccountEmail != null && a.AccountEmail.ToLower().Contains(term)));
        }
        return Ok(accounts.ToList());
    }

    [HttpPost]
    public IActionResult CreateAccount([FromBody] AccountCreateDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.AccountEmail) || string.IsNullOrWhiteSpace(dto.AccountPassword))
        {
            return BadRequest(new { message = "Email and Password are required." });
        }

        try
        {
            var newAccount = new SystemAccount
            {
                AccountName = dto.AccountName,
                AccountEmail = dto.AccountEmail.Trim(),
                AccountRole = dto.AccountRole,
                AccountPassword = dto.AccountPassword
            };

            var success = _accountRepository.CreateAccount(newAccount);
            if (success)
            {
                return CreatedAtAction(nameof(GetAccount), new { id = newAccount.AccountId }, newAccount);
            }

            return BadRequest(new { message = "Could not create account." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateAccount(short id, [FromBody] AccountUpdateDto dto)
    {
        if (dto == null || id != dto.AccountId)
        {
            return BadRequest(new { message = "ID in URL does not match request body." });
        }

        var existing = _accountRepository.GetAccountById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Account with ID {id} not found." });
        }

        try
        {
            var accountToUpdate = new SystemAccount
            {
                AccountId = id,
                AccountName = dto.AccountName,
                AccountEmail = dto.AccountEmail.Trim(),
                AccountRole = dto.AccountRole,
                AccountPassword = !string.IsNullOrWhiteSpace(dto.AccountPassword) ? dto.AccountPassword : existing.AccountPassword
            };

            var success = _accountRepository.UpdateAccount(accountToUpdate);
            if (success)
            {
                return Ok(new { message = "Account updated successfully." });
            }

            return BadRequest(new { message = "Could not update account." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteAccount(short id)
    {
        var existing = _accountRepository.GetAccountById(id);
        if (existing == null)
        {
            return NotFound(new { message = $"Account with ID {id} not found." });
        }

        try
        {
            var success = _accountRepository.DeleteAccount(id);
            if (success)
            {
                return Ok(new { message = "Account deleted successfully." });
            }

            return BadRequest(new { message = "Could not delete account." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
