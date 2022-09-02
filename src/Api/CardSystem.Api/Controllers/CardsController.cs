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
public class CardsController : ControllerBase
{
    private readonly IAsyncEntityRepository<Card, int> _cardRepository;
    private readonly ILogger<CardsController> _logger;
    private readonly IMapper _mapper;

    public CardsController(IAsyncEntityRepository<Card, int> cardRepository,
        ILogger<CardsController> logger, IMapper mapper)
    {
        _cardRepository = cardRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<CardMessage>>> GetAccounts()
    {
        var userId = User.GetUserId();
        var cards = await _cardRepository.GetAllAsync(c => c.UserId == userId);
        return Ok(_mapper.Map<List<CardMessage>>(cards));
    }
}