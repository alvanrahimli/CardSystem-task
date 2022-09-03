namespace CardSystem.Api.Messages;

public record TransactionMessage(
    int Id,
    DateTime? Timestamp,
    decimal Amount,
    string? Type,
    string CardNumber,
    VendorMessage Vendor);