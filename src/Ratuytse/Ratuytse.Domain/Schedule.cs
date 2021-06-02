using System;
using Cronos;

namespace Ratuytse.Domain
{
    public class Schedule
    {
        private CronExpression _expression;

        public Schedule(string cron, string message)
        {
            _expression = CronExpression.Parse(cron);

            Message = message;
        }

        public Schedule(string cron, DateTime lastRunDate, string message) 
            : this(cron, message)
        {
            LastRunDate = DateTime.SpecifyKind(lastRunDate, DateTimeKind.Utc);
        }

        public DateTime LastRunDate { get; private set; }

        public string Message { get; }

        public bool TryRun(DateTime asOf)
        {
            var next = _expression.GetNextOccurrence(DateTime.SpecifyKind(LastRunDate, DateTimeKind.Utc));
            if (next.HasValue && next.Value <= asOf)
            {
                LastRunDate = next.Value;

                return true;
            }

            return false;
        }
    }
}