namespace CardSystem.Api.Messages;

public record ProfileMessage(
    int? Id,
    string FirstName,
    string LastName,
    string? Username);