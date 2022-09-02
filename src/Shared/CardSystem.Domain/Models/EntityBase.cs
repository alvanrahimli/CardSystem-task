namespace CardSystem.Domain.Models;

public class EntityBase<T>
{
    public T Id { get; set; } = default!;

    public override string ToString()
    {
        return $"{GetType().Name}[Id={Id}]";
    }
}