using System.Threading.Tasks;

namespace Web_BanHang.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
    }
}