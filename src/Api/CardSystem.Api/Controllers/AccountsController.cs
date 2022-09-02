using AutoMapper;
using CardSystem.Api.Extensions;
using CardSystem.Api.Messages;
using CardSystem.DataAccess.Abstract;
using CardSystem.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAsyncEntityRepository<Account, int> _accountRepository;
    private readonly ILogger<AccountsController> _logger;
    private readonly IMapper _mapper;

    public AccountsController(IAsyncEntityRepository<Account, int> accountRepository,
        ILogger<AccountsController> logger, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountMessage>>> GetAccounts()
    {
        var userId = User.GetUserId();
        var accounts = await _accountRepository.GetAllAsync(a => a.UserId == userId);
        return Ok(_mapper.Map<List<AccountMessage>>(accounts));
    }
}