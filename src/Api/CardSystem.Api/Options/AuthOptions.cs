namespace CardSystem.Api.Options;

public class AuthOptions
{
    public const string ConfigSection = "AuthOptions";

    public string Secret { get; set; } = default!;
    public int ValidForMinutes { get; set; } = 1;
}