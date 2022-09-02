namespace CardSystem.Api.Messages;

public record CardMessage(
    int Id,
    string CardNumber,
    bool IsValid,
    string State,
    string Type,
    string? Currency);