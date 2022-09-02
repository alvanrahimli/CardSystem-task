﻿using AutoMapper;
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
public class TransactionsController : ControllerBase
{
    private readonly IAsyncEntityRepository<Transaction, int> _transactionsRepo;
    private readonly ILogger<TransactionsController> _logger;
    private readonly IMapper _mapper;

    public TransactionsController(IAsyncEntityRepository<Transaction, int> transactionsRepo,
        ILogger<TransactionsController> logger, IMapper mapper)
    {
        _transactionsRepo = transactionsRepo;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionMessage>>> GetTransactions()
    {
        var userId = User.GetUserId();
        var transactions = await _transactionsRepo.GetAllAsync(x => x.UserId == userId);
        return Ok(_mapper.Map<List<TransactionMessage>>(transactions));
    }
}