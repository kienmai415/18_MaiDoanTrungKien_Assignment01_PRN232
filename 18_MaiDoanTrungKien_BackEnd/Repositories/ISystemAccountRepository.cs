using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public interface ISystemAccountRepository
{
    List<SystemAccount> GetAccounts();
    SystemAccount? GetAccountById(short accountId);
    SystemAccount? GetAccountByEmail(string email);
    SystemAccount? Authenticate(string email, string password);
    bool HasCreatedNews(short accountId);
    bool CreateAccount(SystemAccount account);
    bool UpdateAccount(SystemAccount account);
    bool DeleteAccount(short accountId);
}
