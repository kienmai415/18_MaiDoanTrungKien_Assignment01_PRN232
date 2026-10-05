using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using _18_MaiDoanTrungKien_FrontEnd.Models;

namespace _18_MaiDoanTrungKien_FrontEnd.Services;

public interface IApiService
{
    Task<(bool Success, LoginResultDto? User, string Error)> LoginAsync(string email, string password);
    Task<AccountViewModel?> GetProfileAsync(short accountId);

    // Accounts
    Task<List<AccountViewModel>> GetAccountsAsync(string? keyword = null);
    Task<AccountViewModel?> GetAccountByIdAsync(short id);
    Task<(bool Success, string Message)> CreateAccountAsync(AccountViewModel account);
    Task<(bool Success, string Message)> UpdateAccountAsync(AccountViewModel account);
    Task<(bool Success, string Message)> DeleteAccountAsync(short id);

    // Categories
    Task<List<CategoryViewModel>> GetCategoriesAsync(string? keyword = null);
    Task<List<CategoryViewModel>> GetActiveCategoriesAsync();
    Task<CategoryViewModel?> GetCategoryByIdAsync(short id);
    Task<(bool Success, string Message)> CreateCategoryAsync(CategoryViewModel category);
    Task<(bool Success, string Message)> UpdateCategoryAsync(CategoryViewModel category);
    Task<(bool Success, string Message)> DeleteCategoryAsync(short id);

    // NewsArticles
    Task<List<NewsArticleViewModel>> GetNewsArticlesAsync(string? keyword = null);
    Task<List<NewsArticleViewModel>> GetActiveNewsArticlesAsync(string? keyword = null);
    Task<NewsArticleViewModel?> GetNewsArticleByIdAsync(string id);
    Task<List<NewsArticleViewModel>> GetMyNewsHistoryAsync(short staffId);
    Task<List<NewsArticleViewModel>> GetReportStatisticsAsync(DateTime startDate, DateTime endDate);
    Task<(bool Success, string Message)> CreateNewsArticleAsync(NewsArticleViewModel article, List<int> tagIds);
    Task<(bool Success, string Message)> UpdateNewsArticleAsync(NewsArticleViewModel article, List<int> tagIds);
    Task<(bool Success, string Message)> DeleteNewsArticleAsync(string id);

    // Tags
    Task<List<TagViewModel>> GetTagsAsync();
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5041/api/";
        _httpClient.BaseAddress = new Uri(baseUrl);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
    }

    // =========================================================================================
    // [BƯỚC 3 - HTTP CLIENT CONSUMER]: Đóng gói thông tin (Email, Password) thành JSON
    // Gửi HTTP POST request đến endpoint "api/Auth/login" của BackEnd (Port 5041).
    // Phía BackEnd sẽ tiếp nhận tại hàm: AuthController.Login([FromBody] LoginRequest request)
    // =========================================================================================
    public async Task<(bool Success, LoginResultDto? User, string Error)> LoginAsync(string email, string password)
    {
        try
        {
            // Gửi HTTP POST sang BackEnd Web API
            var response = await _httpClient.PostAsJsonAsync("Auth/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                // Deserialize JSON response trả về thành đối tượng LoginResultDto
                var user = await response.Content.ReadFromJsonAsync<LoginResultDto>(_jsonOptions);
                return (true, user, string.Empty);
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var message = errorObj.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : "Invalid credentials.";
            return (false, null, message ?? "Login failed.");
        }
        catch (Exception ex)
        {
            return (false, null, $"Connection error: {ex.Message}");
        }
    }

    public async Task<AccountViewModel?> GetProfileAsync(short accountId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AccountViewModel>($"Auth/profile/{accountId}", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<AccountViewModel>> GetAccountsAsync(string? keyword = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(keyword) ? "Accounts" : $"Accounts/search?keyword={Uri.EscapeDataString(keyword)}";
            return await _httpClient.GetFromJsonAsync<List<AccountViewModel>>(url, _jsonOptions) ?? new List<AccountViewModel>();
        }
        catch
        {
            return new List<AccountViewModel>();
        }
    }

    public async Task<AccountViewModel?> GetAccountByIdAsync(short id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AccountViewModel>($"Accounts/{id}", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message)> CreateAccountAsync(AccountViewModel account)
    {
        try
        {
            var body = new
            {
                account.AccountName,
                account.AccountEmail,
                account.AccountRole,
                account.AccountPassword
            };
            var response = await _httpClient.PostAsJsonAsync("Accounts", body);
            if (response.IsSuccessStatusCode)
            {
                return (true, "Account created successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to create account.";
            return (false, msg ?? "Failed to create account.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> UpdateAccountAsync(AccountViewModel account)
    {
        try
        {
            var body = new
            {
                account.AccountId,
                account.AccountName,
                account.AccountEmail,
                account.AccountRole,
                account.AccountPassword
            };
            var response = await _httpClient.PutAsJsonAsync($"Accounts/{account.AccountId}", body);
            if (response.IsSuccessStatusCode)
            {
                return (true, "Account updated successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to update account.";
            return (false, msg ?? "Failed to update account.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> DeleteAccountAsync(short id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Accounts/{id}");
            if (response.IsSuccessStatusCode)
            {
                return (true, "Account deleted successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to delete account.";
            return (false, msg ?? "Failed to delete account.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<List<CategoryViewModel>> GetCategoriesAsync(string? keyword = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(keyword) ? "Categories" : $"Categories/search?keyword={Uri.EscapeDataString(keyword)}";
            return await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>(url, _jsonOptions) ?? new List<CategoryViewModel>();
        }
        catch
        {
            return new List<CategoryViewModel>();
        }
    }

    public async Task<List<CategoryViewModel>> GetActiveCategoriesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("Categories/active", _jsonOptions) ?? new List<CategoryViewModel>();
        }
        catch
        {
            return new List<CategoryViewModel>();
        }
    }

    public async Task<CategoryViewModel?> GetCategoryByIdAsync(short id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CategoryViewModel>($"Categories/{id}", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message)> CreateCategoryAsync(CategoryViewModel category)
    {
        try
        {
            var body = new
            {
                category.CategoryName,
                category.CategoryDesciption,
                category.ParentCategoryId,
                category.IsActive
            };
            var response = await _httpClient.PostAsJsonAsync("Categories", body);
            if (response.IsSuccessStatusCode)
            {
                return (true, "Category created successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to create category.";
            return (false, msg ?? "Failed to create category.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> UpdateCategoryAsync(CategoryViewModel category)
    {
        try
        {
            var body = new
            {
                category.CategoryId,
                category.CategoryName,
                category.CategoryDesciption,
                category.ParentCategoryId,
                category.IsActive
            };
            var response = await _httpClient.PutAsJsonAsync($"Categories/{category.CategoryId}", body);
            if (response.IsSuccessStatusCode)
            {
                return (true, "Category updated successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to update category.";
            return (false, msg ?? "Failed to update category.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> DeleteCategoryAsync(short id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return (true, "Category deleted successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to delete category.";
            return (false, msg ?? "Failed to delete category.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<List<NewsArticleViewModel>> GetNewsArticlesAsync(string? keyword = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(keyword) ? "NewsArticles" : $"NewsArticles/search?keyword={Uri.EscapeDataString(keyword)}";
            return await _httpClient.GetFromJsonAsync<List<NewsArticleViewModel>>(url, _jsonOptions) ?? new List<NewsArticleViewModel>();
        }
        catch
        {
            return new List<NewsArticleViewModel>();
        }
    }

    public async Task<List<NewsArticleViewModel>> GetActiveNewsArticlesAsync(string? keyword = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(keyword) ? "NewsArticles/active" : $"NewsArticles/search?keyword={Uri.EscapeDataString(keyword)}&onlyActive=true";
            return await _httpClient.GetFromJsonAsync<List<NewsArticleViewModel>>(url, _jsonOptions) ?? new List<NewsArticleViewModel>();
        }
        catch
        {
            return new List<NewsArticleViewModel>();
        }
    }

    public async Task<NewsArticleViewModel?> GetNewsArticleByIdAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<NewsArticleViewModel>($"NewsArticles/{id}", _jsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<NewsArticleViewModel>> GetMyNewsHistoryAsync(short staffId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<NewsArticleViewModel>>($"NewsArticles/my-history/{staffId}", _jsonOptions) ?? new List<NewsArticleViewModel>();
        }
        catch
        {
            return new List<NewsArticleViewModel>();
        }
    }

    public async Task<List<NewsArticleViewModel>> GetReportStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var s = startDate.ToString("yyyy-MM-dd");
            var e = endDate.ToString("yyyy-MM-dd");
            return await _httpClient.GetFromJsonAsync<List<NewsArticleViewModel>>($"NewsArticles/reports?startDate={s}&endDate={e}", _jsonOptions) ?? new List<NewsArticleViewModel>();
        }
        catch
        {
            return new List<NewsArticleViewModel>();
        }
    }

    public async Task<(bool Success, string Message)> CreateNewsArticleAsync(NewsArticleViewModel article, List<int> tagIds)
    {
        try
        {
            var body = new
            {
                article.NewsArticleId,
                article.NewsTitle,
                article.Headline,
                article.NewsContent,
                article.NewsSource,
                article.CategoryId,
                article.NewsStatus,
                article.CreatedById,
                TagIds = tagIds
            };
            var response = await _httpClient.PostAsJsonAsync("NewsArticles", body);
            if (response.IsSuccessStatusCode)
            {
                return (true, "News article created successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to create article.";
            return (false, msg ?? "Failed to create article.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> UpdateNewsArticleAsync(NewsArticleViewModel article, List<int> tagIds)
    {
        try
        {
            var body = new
            {
                article.NewsArticleId,
                article.NewsTitle,
                article.Headline,
                article.NewsContent,
                article.NewsSource,
                article.CategoryId,
                article.NewsStatus,
                article.UpdatedById,
                TagIds = tagIds
            };
            var response = await _httpClient.PutAsJsonAsync($"NewsArticles/{article.NewsArticleId}", body);
            if (response.IsSuccessStatusCode)
            {
                return (true, "News article updated successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to update article.";
            return (false, msg ?? "Failed to update article.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> DeleteNewsArticleAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"NewsArticles/{id}");
            if (response.IsSuccessStatusCode)
            {
                return (true, "News article deleted successfully.");
            }

            var errorObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
            var msg = errorObj.TryGetProperty("message", out var m) ? m.GetString() : "Failed to delete article.";
            return (false, msg ?? "Failed to delete article.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<List<TagViewModel>> GetTagsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<TagViewModel>>("Tags", _jsonOptions) ?? new List<TagViewModel>();
        }
        catch
        {
            return new List<TagViewModel>();
        }
    }
}
