namespace ZapMe.Services.Interfaces;

public interface IMailGunService
{
    Task<bool> SendMailAsync(string senderName, string senderExt, string recepients, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
