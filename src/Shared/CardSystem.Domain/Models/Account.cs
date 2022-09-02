namespace CardSystem.Domain.Models;

public class Account : EntityBase<int>
{
    public decimal Balance { get; set; }
    public AccountType Type { get; set; }
    
    public int UserId { get; set; }
    public virtual AppUser User { get; set; } = default!;
}