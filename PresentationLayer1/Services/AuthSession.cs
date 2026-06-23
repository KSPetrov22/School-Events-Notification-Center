using PresentationLayer1.Models;

namespace PresentationLayer1.Services;

public interface IAuthSession
{
    MockUser? CurrentUser { get; }
    string? Token { get; }
    bool IsSignedIn { get; }
    bool IsStudent { get; }
    bool IsOrganizer { get; }
    void SignIn(LoginResponse login);
    void SignOut();
}

public sealed class AuthSession(IHttpContextAccessor httpContextAccessor) : IAuthSession
{
    private const string UserIdKey = "mock:user:id";
    private const string UserEmailKey = "mock:user:email";
    private const string UserRoleKey = "mock:user:role";
    private const string UserDisplayNameKey = "mock:user:name";
    private const string TokenKey = "mock:token";

    private ISession? Session => httpContextAccessor.HttpContext?.Session;

    public MockUser? CurrentUser
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
                    : new MockUser(id, email, role, name);
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

