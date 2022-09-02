namespace CardSystem.Api.Messages;

public record struct RegisterMessage(
    string FirstName, string LastName,
    string Username, string Password);