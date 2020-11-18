using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim.Domain
{
    public interface IThrottlingCounter
    {
        string Id { get; }
        string Name { get; }
        int Burst { get; }
        int RestoreCount { get; }
        int RestoreWaitingTimeInSeconds { get; }
        bool HasHourlyRestoring { get; }
        int HourlyRestoreCount { get; }
        DateTime? ResetOn { get; }

        /// <summary>
        /// Apply throttling counter
        /// </summary>
        /// <exception cref="RequstThrottledException">Represent the request was throttled</exception>
        void Apply();
    }
}
