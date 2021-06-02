using System;
using System.Threading;
using System.Threading.Tasks;
using EasyExtensions.BackgroundServiceExtensions;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ratuytse.Domain;

namespace Ratuytse.Bot.Jobs
{
    public class FillReportReminder : ScheduledServiceBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public FillReportReminder(ScheduledServiceOptions<FillReportReminder> options, 
            IServiceProvider serviceProvider,
            IConfiguration configuration) : base(options.Expression)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public override async Task ExecuteJobAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var subscriptionsQuery = (ISubscriptionsQuery)scope.ServiceProvider.GetService(typeof(ISubscriptionsQuery));
            var subscriptionsCommand = (ISubscriptionCommand)scope.ServiceProvider.GetService(typeof(ISubscriptionCommand));
            var subscriptions = await subscriptionsQuery.Execute(SubscriptionType.Report);

            foreach (var subscription in subscriptions)
            {
                try
                {
                    if (subscription.Schedule.TryRun(DateTime.UtcNow))
                    {
                        var conversationReference = JsonConvert.DeserializeObject<ConversationReference>(subscription.ConversationBody);

                        BotAdapter adapter =
                            (BotAdapter)scope.ServiceProvider.GetService(typeof(IBotFrameworkHttpAdapter));

                        await adapter.ContinueConversationAsync(_configuration["MicrosoftAppId"],
                            conversationReference, (t, c) => BotCallback(t, c, subscription.Schedule.Message), cancellationToken);

                        await subscriptionsCommand.Update(subscription.SubscriptionId, DateTime.UtcNow);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken, string message)
        {
            var reply = MessageFactory.Text(message);
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
