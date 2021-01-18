using PanelDePon_WPF.Services.Interfaces;

namespace PanelDePon_WPF.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
