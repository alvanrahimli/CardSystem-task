namespace CardSystem.Domain.Models;

public class Vendor : EntityBase<int>
{
    public string Name { get; set; } = default!;
    public List<string> Addresses { get; set; } = new(); // Will be mapped to postgres's text[] data type
    public List<string> Contacts { get; set; } = new();  // Will be mapped to postgres's text[] data type
}