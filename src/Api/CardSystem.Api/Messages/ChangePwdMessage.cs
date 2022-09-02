namespace CardSystem.Api.Messages;

public record ChangePwdMessage(
    string Old,
    string New,
    string NewConfirmed);