namespace CardSystem.Api.Messages;

public record VendorMessage(
    int Id,
    string Name,
    List<string> Addresses,
    List<string> Contacts);