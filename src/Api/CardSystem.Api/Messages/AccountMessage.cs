namespace CardSystem.Api.Messages;

public record AccountMessage(
    decimal Balance,
    string Type);