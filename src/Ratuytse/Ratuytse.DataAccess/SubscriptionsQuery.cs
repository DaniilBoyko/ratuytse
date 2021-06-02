using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Ratuytse.Domain;

namespace Ratuytse.DataAccess
{
    public class SubscriptionsQuery : ISubscriptionsQuery
    {
        private readonly string _connectionString;

        private const string Sql = @"
SELECT 
    s.subscription_id AS SubscriptionId
    ,s.conversation_id AS ConversationId
    ,s.subscription_name AS SubscriptionName
    ,s.conversation_body AS ConversationBody
    ,s.type_id AS TypeId
    ,ss.cron_schedule AS CronSchedule
    ,ss.last_run_dt AS LastRunDate
    ,ss.default_message AS DefaultMessage
FROM subscription s
JOIN scheduled_subscription ss ON ss.subscription_id = s.subscription_id AND s.type_id = @typeId
";

        public SubscriptionsQuery(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainDb");
        }

        public async Task<ICollection<Subscription>> Execute(SubscriptionType subscriptionType)
        {
            await using var connection = new SqlConnection(_connectionString);
            var subscriptions = await connection.QueryAsync<SubscriptionDto>(Sql, new {typeId = (int)subscriptionType});

            return subscriptions
                    .Select(s =>
                    {
                        var schedule = new Schedule(s.CronSchedule, s.LastRunDate, s.DefaultMessage);

                        var subscription = new Subscription(s.SubscriptionId, 
                            s.ConversationId, 
                            s.ConversationBody, 
                            (SubscriptionType)s.TypeId,
                            s.SubscriptionName);

                        subscription.SetSchedule(schedule);

                        return subscription;
                    })
                    .ToList();
        }
    }
}
