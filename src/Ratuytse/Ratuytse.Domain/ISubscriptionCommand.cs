using System;
using System.Threading.Tasks;

namespace Ratuytse.Domain
{
    public interface ISubscriptionCommand
    {
        Task<bool> Subscribe(string conversationId, 
            string subscriptionName,
            SubscriptionType subscriptionType, 
            string conversationBody,
            string cron,
            string message);

        Task Update(int subscriptionId, DateTime lastRunDate);

        Task Unsubscribe(string conversationId, SubscriptionType subscriptionType);
    }
}
