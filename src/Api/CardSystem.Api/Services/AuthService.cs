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
    private readonly ILogger<AuthService> _logger;

    public AuthService(IAsyncEntityRepository<AppUser, int> userRepository,
        IOptions<AuthOptions> authOptions, IEmailSender emailSender, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _authOptions = authOptions;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<UserMessage?> RegisterUser(RegisterMessage message)
    {
        var sameUsernameUser = await _userRepository.GetAllAsync(x => x.Username == message.Username);
        if (sameUsernameUser.Count > 0)
        {
            _logger.LogWarning("Duplicate username found");
            return null;
        }
        
        var user = new AppUser
        {
            FirstName = message.FirstName,
            LastName = message.LastName,
            Username = message.Username,
            PasswordHash = CryptoHelpers.GeneratePwdHash(message.Password)
        };

        user = await _userRepository.AddAsync(user);
        if (await _userRepository.SaveChangesAsync() && user is not null)
        {
            _logger.LogInformation("Created new user {@User}", user);
            return new UserMessage(user.Username, user.LastName, user.FirstName);
        }

        return null;
    }

    public async Task<TokenMessage?> LogInUser(LoginMessage message)
    {
        var possibleUsers = await _userRepository.GetAllAsync(x => x.Username == message.Username);
        var pwdHash = CryptoHelpers.GeneratePwdHash(message.Password);
        var user = possibleUsers.FirstOrDefault(u => u.PasswordHash == pwdHash);

        if (user is null)
        {
            _logger.LogWarning("User with username: {Username} not found", message.Username);
            return null;
        }

        var jwtContext = new TokenHelpers.JwtContext(_authOptions.Value.Secret,
            DateTime.UtcNow.AddMinutes(_authOptions.Value.ValidForMinutes), new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Expiration,
                    DateTime.UtcNow.AddMinutes(_authOptions.Value.ValidForMinutes)
                        .ToString(CultureInfo.InvariantCulture))
            });
        user.LastLoginTime = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
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
            user.LastPwdChangeTime = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("New password generated for User {@User}", user);
            return true;
        }

        return false;
    }
}