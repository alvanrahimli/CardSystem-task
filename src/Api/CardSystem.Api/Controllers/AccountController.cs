using CardSystem.Api.Messages;
using CardSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AuthService _authService;

    public AccountController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterMessage>> Register(RegisterMessage message)
    {
        var user = await _authService.RegisterUser(message);
        if (user is null)
            return Unauthorized("Could not register user");

        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<RegisterMessage>> Login(LoginMessage message)
    {
        var token = await _authService.LogInUser(message);
        if (token is null)
            return Unauthorized("Username/password combination is invalid");

        return Ok(token);
    }
    
    [HttpPost("new-password")]
    public async Task<ActionResult<RegisterMessage>> NewPassword(EmailMessage message)
    {
        var couldSendPwd = await _authService.RequestNewPwd(message.Email);
        if (!couldSendPwd)
            return Unauthorized("Could not send email");

        return Ok();
    }

    [Authorize]
    [HttpGet("test-auth")]
    public ActionResult TestAuth()
    {
        return Ok();
    }
}