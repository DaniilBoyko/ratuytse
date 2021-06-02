using System;

namespace Ratuytse.DataAccess
{
    internal class SubscriptionDto
    {
        public int SubscriptionId { get; set; }

        public string SubscriptionName { get; set; }

        public string ConversationId { get; set; }

        public int TypeId { get; set; }

        public string ConversationBody { get; set; }

        public string CronSchedule { get; set; }

        public DateTime LastRunDate { get; set; }

        public string DefaultMessage { get; set; }
    }
}