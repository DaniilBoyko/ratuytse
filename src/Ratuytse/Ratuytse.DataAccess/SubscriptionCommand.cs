using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Ratuytse.Domain;

namespace Ratuytse.DataAccess
{
    public class SubscriptionCommand : ISubscriptionCommand
    {
        private string _connectionString;

        private const string SubscribeSql = @"
IF NOT EXISTS (
                SELECT 1 FROM subscription 
                WHERE conversation_id = @conversationId
                    AND [type_id] = @typeId
                    AND [subscription_name] = '@subscriptionName'
              )
BEGIN 
    BEGIN TRAN
        DECLARE @subscriptionId INT;

        insert into subscription 
        values (@conversationId, @typeId, 1, @conversationBody, @subscriptionName);

        SET @subscriptionId = SCOPE_IDENTITY();

        INSERT INTO scheduled_subscription
        VALUES (@subscriptionId, @cron, GETUTCDATE(), @message)
    COMMIT TRAN
END
";

        private const string UpdateSql = @"
UPDATE scheduled_subscription
SET last_run_dt = @lastRunDate
WHERE subscription_id = @subscriptionId
";

        private const string UnsubscribeSql = @"
DECLARE @ids as TABLE ([id] INT)

INSERT INTO @ids 
SELECT subscription_id 
FROM subscription
WHERE conversation_id = @conversationId
    AND [type_id] = @typeId

DELETE FROM scheduled_subscription
WHERE subscription_id IN (SELECT id FROM @ids)

DELETE FROM subscription
WHERE subscription_id IN (SELECT id FROM @ids)
";

        public SubscriptionCommand(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainDb");
        }

        public async Task<bool> Subscribe(string conversationId,
            string subscriptionName,
            SubscriptionType subscriptionType,
            string conversationBody,
            string cron, 
            string message)
        {
            await using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(SubscribeSql, new
            {
                conversationId, 
                typeId = (int)subscriptionType, 
                conversationBody,
                cron,
                message,
                subscriptionName,
            });

            return result > 0;
        }

        public async Task Update(int subscriptionId, DateTime lastRunDate)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(UpdateSql, new { subscriptionId, lastRunDate });
        }

        public async Task Unsubscribe(string conversationId, SubscriptionType subscriptionType)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(UnsubscribeSql, new { conversationId, typeId = (int)subscriptionType });
        }
    }
}
