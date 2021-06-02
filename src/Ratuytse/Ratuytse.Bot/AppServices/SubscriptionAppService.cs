using System;
using System.Linq;
using System.Threading.Tasks;
using Ratuytse.Domain;

namespace Ratuytse.Bot.AppServices
{
    public interface ISubscriptionAppService
    {
        Task Process(string messageText,
            string conversationId,
            string conversationBody,
            Action<string> respondWith);
    }

    public class SubscriptionAppService : ISubscriptionAppService
    {
        private readonly ISubscriptionCommand _subscriptionCommand;

        public SubscriptionAppService(ISubscriptionCommand subscriptionCommand)
        {
            _subscriptionCommand = subscriptionCommand;
        }

        public async Task Process(string messageText,
            string conversationId,
            string conversationBody,
            Action<string> respondWith)
        {
            var terms = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (terms.Any(t => string.Equals(t, "subscribe", StringComparison.InvariantCultureIgnoreCase)) 
                    && terms.Any(t => string.Equals(t, SubscriptionType.Report.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                var success = await _subscriptionCommand.Subscribe(conversationId,
                    terms.Length > 3 ? terms[3] : SubscriptionType.Report.ToString(),
                    SubscriptionType.Report,
                    conversationBody,
                    "10 13 * * MON", 
                    "default message");

                respondWith(success ? "subscribed successfully!" : "already subscribed!");
            }
            else if (terms.Any(t => string.Equals(t, "unsubscribe", StringComparison.InvariantCultureIgnoreCase))
                     && terms.Any(t => string.Equals(t, SubscriptionType.Report.ToString(), StringComparison.InvariantCultureIgnoreCase)))
            {
                await _subscriptionCommand.Unsubscribe(conversationId,
                    SubscriptionType.Report);

                respondWith("unsubscribed successfully!");
            }
        }
    }
}
