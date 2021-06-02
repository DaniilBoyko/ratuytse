using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ratuytse.Domain
{
    public interface ISubscriptionsQuery
    {
        Task<ICollection<Subscription>> Execute(SubscriptionType subscriptionType);
    }
}
