using System.Collections.Generic;
using _18_MaiDoanTrungKien_BackEnd.DAOs;
using _18_MaiDoanTrungKien_BackEnd.Models;

namespace _18_MaiDoanTrungKien_BackEnd.Repositories;

// =========================================================================================
// [BƯỚC 5 - TẦNG REPOSITORY (REPOSITORY PATTERN)]:
// Lớp này đóng vai trò trung gian trừu tượng hóa truy cập dữ liệu giữa Controller và DAO.
// Controller KHÔNG được gọi trực tiếp DbContext (theo đúng yêu cầu đề bài trang 5).
// Tất cả các hàm ở đây đều chuyển tiếp đến tầng Data Access Object qua Singleton: SystemAccountDAO.Instance.
// =========================================================================================
public class SystemAccountRepository : ISystemAccountRepository
{
    // Lấy danh sách toàn bộ tài khoản
    public List<SystemAccount> GetAccounts() => SystemAccountDAO.Instance.GetAccounts();

    // Lấy tài khoản theo ID
    public SystemAccount? GetAccountById(short accountId) => SystemAccountDAO.Instance.GetAccountById(accountId);

    // Lấy tài khoản theo Email
    public SystemAccount? GetAccountByEmail(string email) => SystemAccountDAO.Instance.GetAccountByEmail(email);

    // Xác thực tài khoản Staff / Lecturer (BƯỚC 5 trong luồng Login) -> Gọi tiếp sang SystemAccountDAO.Instance.Authenticate(...)
    public SystemAccount? Authenticate(string email, string password) => SystemAccountDAO.Instance.Authenticate(email, password);

    // Kiểm tra tài khoản đã từng tạo bài viết chưa (dùng cho ràng buộc xóa tài khoản)
    public bool HasCreatedNews(short accountId) => SystemAccountDAO.Instance.HasCreatedNews(accountId);

    // Thêm mới tài khoản
    public bool CreateAccount(SystemAccount account) => SystemAccountDAO.Instance.CreateAccount(account);

    // Cập nhật thông tin tài khoản
    public bool UpdateAccount(SystemAccount account) => SystemAccountDAO.Instance.UpdateAccount(account);

    // Xóa tài khoản (có kiểm tra ràng buộc không được xóa nếu đã tạo bài)
    public bool DeleteAccount(short accountId) => SystemAccountDAO.Instance.DeleteAccount(accountId);
}
