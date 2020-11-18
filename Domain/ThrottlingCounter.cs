using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim.Domain
{
    internal class ThrottlingCounter : IThrottlingCounter
    {
        private readonly ThrottlingValueObject _tvo;
        private DateTime? _lastInvokedTime;

        public string Id => _tvo.Operation + _tvo.ApiTypeId.ToString() + (_tvo.ReportType ?? string.Empty);

        public string Name { get; }

        public int Burst => _burst;
        private int _burst;

        public int RestoreCount => _restoreCount;
        private int _restoreCount;

        public int RestoreWaitingTimeInSeconds => _restoreWaitingTimeInSeconds;
        private int _restoreWaitingTimeInSeconds;

        public bool HasHourlyRestoring { get; }

        public int HourlyRestoreCount => _hourlyRestoreCount;
        private int _hourlyRestoreCount;

        public DateTime? ResetOn => _resetOn;
        private DateTime? _resetOn = default;

        ThrottlingCounter(ThrottlingValueObject tvo)
        {
            _tvo = tvo;
            Name = tvo.Operation + " " + tvo.ReportType ?? "";
            _burst = tvo.MaximumRequestQuota;
            _restoreCount = tvo.RestoreAmount;
            _restoreWaitingTimeInSeconds = tvo.RestoreWaitingTimeInSeconds;
            HasHourlyRestoring = (tvo.HourlyRequestQuota != default);
            _hourlyRestoreCount = tvo.HourlyRequestQuota;
        }

        public static explicit operator ThrottlingCounter(ThrottlingValueObject tvo)
        {
            return new ThrottlingCounter(tvo);
        }

        public void Apply()
        {
            var now = DateTime.Now;

            // Check hourly
            if (_resetOn != default && (now - (DateTime)_resetOn).TotalSeconds > 3600)
            {
                _hourlyRestoreCount = _tvo.HourlyRequestQuota;
                _resetOn = default;
            }

            // Get recovery
            if (_lastInvokedTime != default)
            {
                var diff =
                    (int)
                    (now - (DateTime)_lastInvokedTime).TotalSeconds / _restoreWaitingTimeInSeconds;
                _burst = _burst + diff * _restoreCount;
                if (_burst > _tvo.MaximumRequestQuota)
                    _burst = _tvo.MaximumRequestQuota;
            }

            if (_burst == 0 || _hourlyRestoreCount == 0)
                throw new RequstThrottledException("The request was throttled.");

            // Burst
            _burst -= 1;
            _hourlyRestoreCount -= 1;

            // Set timer
            _lastInvokedTime = DateTime.Now;
            if (_resetOn == default)
                _resetOn = _lastInvokedTime;
        }
    }
}
