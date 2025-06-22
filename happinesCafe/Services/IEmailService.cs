using System.Threading.Tasks;

namespace happinesCafe.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
