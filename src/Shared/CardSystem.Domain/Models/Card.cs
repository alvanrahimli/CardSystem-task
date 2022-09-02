namespace CardSystem.Domain.Models;

public class Card : EntityBase<int>
{
    public string CardNumber { get; set; } = default!;
    public bool IsValid { get; set; }
    public CardState State { get; set; }
    public CardType Type { get; set; }
    
    /// <summary>
    /// Is assigned only if <see cref="Type"/> is <see cref="CardType.Currency"/>
    /// </summary>
    public Currency? Currency { get; set; }

    public int UserId { get; set; }
    public virtual AppUser User { get; set; } = default!;
}