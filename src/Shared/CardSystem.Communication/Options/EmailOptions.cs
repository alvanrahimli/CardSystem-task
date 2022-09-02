namespace CardSystem.Communication.Options;

public class EmailOptions
{
    public const string ConfigSection = "EmailOptions";

    public string FromAddress { get; set; } = string.Empty;
}