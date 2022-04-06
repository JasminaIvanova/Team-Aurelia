using Aurelia.App.Models;

namespace Aurelia.App.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
