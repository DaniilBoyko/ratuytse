// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.13.1

using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ratuytse.Bot.AppServices;

namespace Ratuytse.Bot.Bots
{
    public class SubscriptionBot : ActivityHandler
    {
        private readonly ISubscriptionAppService _subscriptionAppService;

        public SubscriptionBot(ISubscriptionAppService subscriptionAppService)
        {
            _subscriptionAppService = subscriptionAppService;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken)
        {
            var conversationReference = turnContext.Activity.GetConversationReference();
            await _subscriptionAppService.Process(turnContext.Activity.Text,
                conversationReference.Conversation.Id,
                JsonConvert.SerializeObject(conversationReference),
                async (response) =>
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(response),
                        cancellationToken);
                });
        }
    }
}
