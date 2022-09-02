namespace CardSystem.Domain.Models;

public class AppUser : EntityBase<int>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    
    public DateTime LastLoginTime { get; set; }
    public DateTime LastPwdChangeTime { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}