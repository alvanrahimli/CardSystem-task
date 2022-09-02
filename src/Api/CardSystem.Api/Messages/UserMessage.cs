namespace CardSystem.Api.Messages;

public record UserMessage(
    string Username,
    string LastName, string FirstName);