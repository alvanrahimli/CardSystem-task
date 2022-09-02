namespace CardSystem.Api.Messages;

public record AccountMessage(
    int Id,
    decimal Balance,
    string Type);