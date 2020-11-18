using mws_sim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim
{
    public interface IThrottlingService
    {
        IEnumerable<IThrottlingCounter> GetCounterList();
        /// <summary>
        /// Handle invocation to trigger throttling.
        /// </summary>
        /// <param name="throttlingId">Identifier of some ThrottlingCounter</param>
        /// <exception cref="RequstThrottledException">Represent the request was throttled</exception>
        void HandleInvoke(string throttlingId);
    }
}
