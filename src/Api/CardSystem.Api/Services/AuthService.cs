using System.Globalization;
using System.Security.Claims;
using CardSystem.Api.Messages;
using CardSystem.Api.Options;
using CardSystem.Communication.Abstract;
using CardSystem.DataAccess.Abstract;
using CardSystem.Domain.Models;
using CardSystem.Services;
using Microsoft.Extensions.Options;

namespace CardSystem.Api.Services;

public class AuthService
{
    private readonly IAsyncEntityRepository<AppUser, int> _userRepository;
    private readonly IOptions<AuthOptions> _authOptions;
    private readonly IEmailSender _emailSender;

    public AuthService(IAsyncEntityRepository<AppUser, int> userRepository,
        IOptions<AuthOptions> authOptions, IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _authOptions = authOptions;
        _emailSender = emailSender;
    }

    public async Task<UserMessage?> RegisterUser(RegisterMessage message)
    {
        var sameUsernameUser = await _userRepository.GetAllAsync(x => x.Username == message.Username);
        if (sameUsernameUser.Count > 0) return null;
        
        var user = new AppUser
        {
            FirstName = message.FirstName,
            LastName = message.LastName,
            Username = message.Username,
            PasswordHash = CryptoHelpers.GeneratePwdHash(message.Password)
        };

        user = await _userRepository.AddAsync(user);
        return user is not null
            ? new UserMessage(user.Username, user.LastName, user.FirstName)
            : null;
    }

    public async Task<TokenMessage?> LogInUser(LoginMessage message)
    {
        var possibleUsers = await _userRepository.GetAllAsync(x => x.Username == message.Username);
        var pwdHash = CryptoHelpers.GeneratePwdHash(message.Password);
        var user = possibleUsers.FirstOrDefault(u => u.PasswordHash == pwdHash);
        
        if (user is null) return null;

        var jwtContext = new TokenHelpers.JwtContext(_authOptions.Value.Secret,
            DateTime.UtcNow.AddMinutes(_authOptions.Value.ValidForMinutes), new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Expiration,
                    DateTime.UtcNow.AddMinutes(_authOptions.Value.ValidForMinutes)
                        .ToString(CultureInfo.InvariantCulture))
            });
        var token = TokenHelpers.GetJwt(jwtContext);
        return new TokenMessage(token);
    }

    public async Task<bool> RequestNewPwd(string username)
    {
        var user = await _userRepository.GetSingle(x => x.Username == username);
        if (user is null)
        {
            return false;
        }

        var randomPwd = RandomHelpers.RandomString(8);
        user.PasswordHash = CryptoHelpers.GeneratePwdHash(randomPwd);
        if (await _userRepository.SaveChangesAsync())
        {
            await _emailSender.SendEmail(user.Username, "Password changed", $"Your new password is {randomPwd}");
            return true;
        }

        return false;
    }
}