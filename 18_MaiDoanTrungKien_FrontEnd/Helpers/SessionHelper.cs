using Microsoft.AspNetCore.Http;
using System.Text.Json;
using _18_MaiDoanTrungKien_FrontEnd.Models;

namespace _18_MaiDoanTrungKien_FrontEnd.Helpers;

public static class SessionHelper
{
    private const string UserSessionKey = "LOGGED_IN_USER";

    public static void SetUser(ISession session, LoginResultDto user)
    {
        session.SetString(UserSessionKey, JsonSerializer.Serialize(user));
    }

    public static LoginResultDto? GetUser(ISession session)
    {
        var str = session.GetString(UserSessionKey);
        if (string.IsNullOrEmpty(str)) return null;
        try
        {
            return JsonSerializer.Deserialize<LoginResultDto>(str);
        }
        catch
        {
            return null;
        }
    }

    public static void ClearUser(ISession session)
    {
        session.Remove(UserSessionKey);
    }

    public static bool IsAdmin(ISession session)
    {
        var user = GetUser(session);
        return user != null && user.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsStaff(ISession session)
    {
        var user = GetUser(session);
        return user != null && (user.RoleName.Equals("Staff", StringComparison.OrdinalIgnoreCase) || user.AccountRole == 1);
    }
}
