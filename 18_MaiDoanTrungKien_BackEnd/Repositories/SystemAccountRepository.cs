using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.DAOs;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

public class SystemAccountRepository : ISystemAccountRepository
{
    public List<SystemAccount> GetAccounts() => SystemAccountDAO.Instance.GetAccounts();

    public SystemAccount? GetAccountById(short accountId) => SystemAccountDAO.Instance.GetAccountById(accountId);

    public SystemAccount? GetAccountByEmail(string email) => SystemAccountDAO.Instance.GetAccountByEmail(email);

    public SystemAccount? Authenticate(string email, string password) => SystemAccountDAO.Instance.Authenticate(email, password);

    public bool HasCreatedNews(short accountId) => SystemAccountDAO.Instance.HasCreatedNews(accountId);

    public bool CreateAccount(SystemAccount account) => SystemAccountDAO.Instance.CreateAccount(account);

    public bool UpdateAccount(SystemAccount account) => SystemAccountDAO.Instance.UpdateAccount(account);

    public bool DeleteAccount(short accountId) => SystemAccountDAO.Instance.DeleteAccount(accountId);
}
