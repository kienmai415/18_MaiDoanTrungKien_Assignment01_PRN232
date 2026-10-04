using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.DAOs;

public class SystemAccountDAO
{
    private static SystemAccountDAO? _instance;
    private static readonly object _instanceLock = new object();

    private SystemAccountDAO() { }

    public static SystemAccountDAO Instance
    {
        get
        {
            lock (_instanceLock)
            {
                _instance ??= new SystemAccountDAO();
                return _instance;
            }
        }
    }

    public List<SystemAccount> GetAccounts()
    {
        using var context = new FunewsManagementContext();
        return context.SystemAccounts.AsNoTracking().ToList();
    }

    public SystemAccount? GetAccountById(short accountId)
    {
        using var context = new FunewsManagementContext();
        return context.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountId == accountId);
    }

    public SystemAccount? GetAccountByEmail(string email)
    {
        using var context = new FunewsManagementContext();
        return context.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountEmail.ToLower() == email.Trim().ToLower());
    }

    public SystemAccount? Authenticate(string email, string password)
    {
        using var context = new FunewsManagementContext();
        return context.SystemAccounts.AsNoTracking()
            .FirstOrDefault(a => a.AccountEmail.ToLower() == email.Trim().ToLower() && a.AccountPassword == password);
    }

    public bool HasCreatedNews(short accountId)
    {
        using var context = new FunewsManagementContext();
        return context.NewsArticles.Any(n => n.CreatedById == accountId);
    }

    public bool CreateAccount(SystemAccount account)
    {
        using var context = new FunewsManagementContext();
        var exists = context.SystemAccounts.Any(a => a.AccountEmail.ToLower() == account.AccountEmail.Trim().ToLower());
        if (exists)
        {
            throw new InvalidOperationException("An account with this email already exists.");
        }

        context.SystemAccounts.Add(account);
        return context.SaveChanges() > 0;
    }

    public bool UpdateAccount(SystemAccount account)
    {
        using var context = new FunewsManagementContext();
        var existing = context.SystemAccounts.Find(account.AccountId);
        if (existing == null)
        {
            return false;
        }

        var emailConflict = context.SystemAccounts.Any(a => a.AccountId != account.AccountId && a.AccountEmail.ToLower() == account.AccountEmail.Trim().ToLower());
        if (emailConflict)
        {
            throw new InvalidOperationException("An account with this email already exists.");
        }

        existing.AccountName = account.AccountName;
        existing.AccountEmail = account.AccountEmail;
        if (!string.IsNullOrEmpty(account.AccountPassword))
        {
            existing.AccountPassword = account.AccountPassword;
        }
        if (account.AccountRole.HasValue)
        {
            existing.AccountRole = account.AccountRole;
        }

        return context.SaveChanges() > 0;
    }

    public bool DeleteAccount(short accountId)
    {
        using var context = new FunewsManagementContext();
        var existing = context.SystemAccounts.Find(accountId);
        if (existing == null)
        {
            return false;
        }

        if (HasCreatedNews(accountId))
        {
            throw new InvalidOperationException("Cannot delete this account because it has created news articles.");
        }

        context.SystemAccounts.Remove(existing);
        return context.SaveChanges() > 0;
    }
}
