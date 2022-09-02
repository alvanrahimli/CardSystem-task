using CardSystem.Communication.Abstract;
using CardSystem.Communication.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CardSystem.Communication.Concrete;

public class MockEmailSender : IEmailSender
{
    private readonly IOptions<EmailOptions> _emailOptions;
    private readonly ILogger<MockEmailSender> _logger;

    public MockEmailSender(IOptions<EmailOptions> emailOptions,
        ILogger<MockEmailSender> logger)
    {
        if (emailOptions.Value is null) 
            throw new InvalidOperationException("Could not resolve IOptions<EmailOptions>");
        
        _emailOptions = emailOptions;
        _logger = logger;
    }
    
    public async Task<bool> SendEmail(string to, string title, string body)
    {
        _logger.LogInformation("Sent email from '{From}' to '{To}': {Title}: {Body}",
            _emailOptions.Value.FromAddress, to, title, body);
        return await Task.FromResult(true);
    }
}