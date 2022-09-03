using System.Text.Json;

namespace CardSystem.Api.Messages;

public class ErrorMessage
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = "Something went wrong";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}