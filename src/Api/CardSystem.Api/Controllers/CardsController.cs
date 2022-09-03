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
    public async Task<ActionResult<List<CardMessage>>> GetCards()
    {
        var userId = User.GetUserId();
        var cards = await _cardRepository.GetAllAsync(c => c.UserId == userId);
        return Ok(_mapper.Map<List<CardMessage>>(cards));
    }

    [HttpPost]
    public async Task<ActionResult<CardMessage>> CreateCard(CardMessage message)
    {
        var duplicateNumber = await _cardRepository.GetSingle(c => c.CardNumber == message.CardNumber);
        if (duplicateNumber is not null) return StatusCode(409, "Duplicate card number");

        if (message is not { CardNumber.Length: 16, Type: { } })
        {
            return BadRequest("Invalid model entered");
        }
        
        var parsedType = Enum.Parse<CardType>(message.Type);
        var card = new Card
        {
            UserId = User.GetUserId(),
            CardNumber = message.CardNumber,
            IsValid = true,
            State = CardState.Active,
            Type = parsedType,
            Currency = parsedType is CardType.Currency
                ? Enum.Parse<Currency>(message.Currency!)
                : null
        };
        card = await _cardRepository.AddAsync(card);
        await _cardRepository.SaveChangesAsync();
        if (card is null)
        {
            return UnprocessableEntity("Could not create card");
        }

        return Ok(_mapper.Map<CardMessage>(card));
    }

    [HttpPut]
    public async Task<ActionResult<CardMessage>> UpdateCard(CardMessage message)
    {
        var card = await _cardRepository.GetSingle(c => c.Id == message.Id);
        if (card is null) return NotFound($"Card not found with Id: {message.Id}");
        if (card.UserId != User.GetUserId()) return NotFound();

        if (message.State != null) card.State = Enum.Parse<CardState>(message.State);
        if (message.Type != null) card.Type = Enum.Parse<CardType>(message.Type);
        if (message.IsValid != null) card.IsValid = (bool)message.IsValid;
        if (message.Currency != null && card.Type == CardType.Currency)
            card.Currency = Enum.Parse<Currency>(message.Currency); 
        
        card = await _cardRepository.UpdateAsync(card);
        if (card is null) return UnprocessableEntity("Could not update card");

        return Ok(_mapper.Map<CardMessage>(card));
    }
}