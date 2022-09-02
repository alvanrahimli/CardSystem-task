using System.Globalization;
using System.Security.Claims;
using CardSystem.Api.Messages;
using CardSystem.Api.Options;
using CardSystem.DataAccess.Abstract;
using CardSystem.Domain.Models;
using CardSystem.Services;
using Microsoft.Extensions.Options;

namespace CardSystem.Api.Services;

public class AuthService
{
    private readonly IAsyncEntityRepository<AppUser, int> _userRepository;
    private readonly IOptions<AuthOptions> _authOptions;

    public AuthService(IAsyncEntityRepository<AppUser, int> userRepository,
        IOptions<AuthOptions> authOptions)
    {
        _userRepository = userRepository;
        _authOptions = authOptions;
    }

    public async Task<AppUser?> RegisterUser(RegisterMessage message)
    {
        var user = new AppUser
        {
            FirstName = message.FirstName,
            LastName = message.LastName,
            Username = message.Username,
            PasswordHash = CryptoHelpers.GeneratePwdHash(message.Password)
        };

        var addedUser = await _userRepository.AddAsync(user);
        return addedUser;
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
    
}