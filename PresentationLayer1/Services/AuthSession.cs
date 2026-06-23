using PresentationLayer1.Models;

namespace PresentationLayer1.Services;

public interface IAuthSession
{
    UserInfo? CurrentUser { get; }
    string? Token { get; }
    bool IsSignedIn { get; }
    bool IsStudent { get; }
    bool IsOrganizer { get; }
    void SignIn(LoginResponse login);
    void SignOut();
}

public sealed class AuthSession(IHttpContextAccessor httpContextAccessor) : IAuthSession
{
    private const string UserIdKey = "user:id";
    private const string UserEmailKey = "user:email";
    private const string UserRoleKey = "user:role";
    private const string UserDisplayNameKey = "user:name";
    private const string TokenKey = "auth:token";

    private ISession? Session => httpContextAccessor.HttpContext?.Session;

    public UserInfo? CurrentUser
    {
        get
        {
            var session = Session;
            var id = session?.GetString(UserIdKey);
            var email = session?.GetString(UserEmailKey);
            var role = session?.GetString(UserRoleKey);
            var name = session?.GetString(UserDisplayNameKey);

            return string.IsNullOrWhiteSpace(id) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(role) ||
                string.IsNullOrWhiteSpace(name)
                    ? null
                    : new UserInfo(id, email, role, name);
        }
    }

    public string? Token => Session?.GetString(TokenKey);
    public bool IsSignedIn => CurrentUser is not null;
    public bool IsStudent => string.Equals(CurrentUser?.Role, "STUDENT", StringComparison.OrdinalIgnoreCase);
    public bool IsOrganizer => string.Equals(CurrentUser?.Role, "ORGANIZER", StringComparison.OrdinalIgnoreCase);


    public void SignIn(LoginResponse login)
    {
        var session = Session ?? throw new InvalidOperationException("Session is not available.");
        session.SetString(UserIdKey, login.User.Id);
        session.SetString(UserEmailKey, login.User.Email);
        session.SetString(UserRoleKey, login.User.Role);
        session.SetString(UserDisplayNameKey, login.User.DisplayName);
        session.SetString(TokenKey, login.Token);
    }

    public void SignOut()
    {
        Session?.Clear();
    }
}

