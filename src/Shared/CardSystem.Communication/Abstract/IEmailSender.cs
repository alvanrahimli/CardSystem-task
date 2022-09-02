namespace CardSystem.Communication.Abstract;

public interface IEmailSender
{
    Task<bool> SendEmail(string to, string title, string body);
}