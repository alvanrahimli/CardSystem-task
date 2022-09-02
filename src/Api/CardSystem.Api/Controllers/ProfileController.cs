using AutoMapper;
using CardSystem.Api.Extensions;
using CardSystem.Api.Messages;
using CardSystem.DataAccess.Abstract;
using CardSystem.Domain.Models;
using CardSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IAsyncEntityRepository<AppUser, int> _userRepository;
    private readonly ILogger<ProfileController> _logger;
    private readonly IMapper _mapper;

    public ProfileController(IAsyncEntityRepository<AppUser, int> userRepository,
        ILogger<ProfileController> logger, IMapper mapper)
    {
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileMessage>> GetProfile()
    {
        var userId = User.GetUserId();
        var user = await _userRepository.GetSingle(x => x.Id == userId);
        if (user is null)
            throw new Exception($"Logged in user not found by Id: {userId}");

        return Ok(_mapper.Map<ProfileMessage>(user));
    }

    [HttpPut]
    public async Task<ActionResult<ProfileMessage>> UpdateProfile(ProfileMessage message)
    {
        var userId = User.GetUserId();
        var user = await _userRepository.GetSingle(x => x.Id == userId);
        if (user is null)
            throw new Exception($"Logged in user not found by Id: {userId}");

        user.LastName = message.LastName;
        user.FirstName = message.FirstName;
        
        user = await _userRepository.UpdateAsync(user);
        if (user is null)
        {
            _logger.LogWarning("Could not save user details");
            return UnprocessableEntity("Could not save user details");
        }

        return Ok(_mapper.Map<ProfileMessage>(user));
    }

    [HttpPut("password")]
    public async Task<ActionResult> ChangePassword(ChangePwdMessage message)
    {
        if (message.New != message.NewConfirmed)
            return UnprocessableEntity("Password and PasswordConfirmation should be identical");
        
        var userId = User.GetUserId();
        var user = await _userRepository.GetSingle(x => x.Id == userId);
        if (user is null)
            throw new Exception($"Logged in user not found by Id: {userId}");

        if (CryptoHelpers.GeneratePwdHash(message.Old) != user.PasswordHash)
        {
            _logger.LogWarning("Failed attempt to change password (incorrect old password)");
            return Unauthorized("Invalid old password entered");
        }

        user.PasswordHash = CryptoHelpers.GeneratePwdHash(message.New);
        user = await _userRepository.UpdateAsync(user);
        if (user is null)
        {
            _logger.LogWarning("Could not save user details");
            return UnprocessableEntity("Could not save user details");
        }

        return Ok();
    }
}