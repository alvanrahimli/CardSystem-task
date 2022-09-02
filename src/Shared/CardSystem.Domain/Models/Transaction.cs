namespace CardSystem.Domain.Models;

public class Transaction : EntityBase<int>
{
    public DateTime Timestamp { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string CardNumber { get; set; } = default!;

    public int VendorId { get; set; }
    public virtual Vendor Vendor { get; set; } = default!;
    
    public int UserId { get; set; }
    public virtual AppUser User { get; set; } = default!;
}