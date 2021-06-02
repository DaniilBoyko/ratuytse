namespace Ratuytse.Domain
{
    public class Subscription
    {
        public Subscription(string conversationId, string conversationBody, SubscriptionType type, string subscriptionName)
        {
            ConversationId = conversationId;
            ConversationBody = conversationBody;
            Type = type;
            SubscriptionName = subscriptionName;
        }

        public Subscription(int subscriptionId,
            string conversationId, 
            string conversationBody,
            SubscriptionType type,
            string subscriptionName) 
            : this(conversationId, conversationBody, type, subscriptionName)
        {
            SubscriptionId = subscriptionId;
        }

        public int SubscriptionId { get; set; }

        public string ConversationId { get; }

        public string SubscriptionName { get; }

        public Schedule Schedule { get; private set; }

        public string ConversationBody { get; }

        public SubscriptionType Type { get; }

        public void SetSchedule(Schedule schedule)
        {
            Schedule = schedule;
        }
    }
}
