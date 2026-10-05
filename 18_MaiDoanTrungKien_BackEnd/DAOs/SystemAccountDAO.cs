using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.DAOs;

// =========================================================================================
// [BƯỚC 6 - TẦNG DATA ACCESS OBJECT (SINGLETON PATTERN)]:
// Lớp này trực tiếp khởi tạo FunewsManagementContext (Entity Framework Core)
// để thực thi các câu lệnh LINQ và truy vấn xuống SQL Server Database.
// Áp dụng Singleton Pattern (private constructor + Instance static property + lock)
// đúng theo yêu cầu thiết kế hệ thống ở Trang 2 của đề bài.
// =========================================================================================
public class SystemAccountDAO
{
    private static SystemAccountDAO? _instance;
    private static readonly object _instanceLock = new object();

    // Private constructor ngăn việc khởi tạo từ bên ngoài
    private SystemAccountDAO() { }

    // Thuộc tính Instance cung cấp điểm truy cập duy nhất (Singleton Pattern)
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

    // =========================================================================================
    // [BƯỚC 7 - TRUY VẤN SQL SERVER]: Kiểm tra Email & Password trong bảng SystemAccount.
    // Nếu tìm thấy -> Trả về entity SystemAccount.
    // Kết quả sẽ đi ngược lại: SystemAccountDAO -> SystemAccountRepository -> AuthController
    // -> ApiService -> LoginModel -> Cập nhật Session & chuyển hướng trang.
    // =========================================================================================
    public SystemAccount? Authenticate(string email, string password)
    {
        using var context = new FunewsManagementContext();
        return context.SystemAccounts.AsNoTracking()
            .FirstOrDefault(a => a.AccountEmail.ToLower() == email.Trim().ToLower() && a.AccountPassword == password);
    }

    // Kiểm tra ràng buộc nghiệp vụ: Tài khoản đã từng tạo bài viết chưa?
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

    // =========================================================================================
    // [RÀNG BUỘC NGHIỆP VỤ XÓA TÀI KHOẢN (TRANG 4 ĐỀ BÀI)]:
    // "the delete action will delete an account in the case this account does not belong to any news articles (created),
    // if the account is already created any news article, cannot delete."
    // Luồng:
    // 1. Kiểm tra HasCreatedNews(accountId).
    // 2. Nếu đã có bài viết -> Ném InvalidOperationException (Controller sẽ trả về 400 Bad Request).
    // 3. Nếu chưa viết bài -> Thực hiện Hard Delete (Remove khỏi DbSet và SaveChanges).
    // =========================================================================================
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
