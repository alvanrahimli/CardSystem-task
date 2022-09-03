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

    [HttpPost]
    public async Task<ActionResult<AccountMessage>> CreateAccount(AccountMessage message)
    {
        var couldParse = Enum.TryParse(message.Type, out AccountType type);
        if (!couldParse) return BadRequest("Invalid AccountType entered. Can be one of: Deposit, Credit, Currency");

        var account = new Account
        {
            UserId = User.GetUserId(),
            Balance = message.Balance,
            Type = type
        };
        account = await _accountRepository.AddAsync(account);
        await _accountRepository.SaveChangesAsync();

        if (account is null)
            return UnprocessableEntity("Could not create account");
        
        return Ok(_mapper.Map<AccountMessage>(account));
    }

    [HttpPost("{accountId}/cash-in")]
    public async Task<ActionResult<AccountMessage>> CashIn(int accountId, CashInMessage message)
    {
        var account = await _accountRepository.GetSingle(a => a.Id == accountId);
        if (account is null) return NotFound("Account not found");

        if (account.UserId != User.GetUserId())
            return NotFound();

        account.Balance += message.Amount;
        account = await _accountRepository.UpdateAsync(account);
        if (account is null)
            return UnprocessableEntity("Could not increase balance of account");

        return Ok(_mapper.Map<AccountMessage>(account));
    }
}