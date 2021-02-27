using System.Collections.Generic;
using System.Threading.Tasks;

namespace OcraServer.Services
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(List<string> to, object data, long timeToLive);
    }
}
