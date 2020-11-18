using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using mws_sim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IThrottlingService _serv;

        public IEnumerable<ThrottlingInfo> ThList { get; private set; } = new List<ThrottlingInfo>();

        [BindProperty(Name = "m.Id")]
        public string ThrottlingId { get; set; }

        public bool IsThrottled { get; set; } = false;

        public RequstThrottledException? ThrottledException { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IThrottlingService serv)
        {
            _logger = logger;
            _serv = serv;
        }

        public void OnGet()
        {
            ThList = _serv.GetCounterList().Select(r => ThrottlingInfo.GetThrottlingInfo(r));
        }

        public void OnPost()
        {
            try
            {
                _serv.HandleInvoke(ThrottlingId);
            }
            catch (RequstThrottledException ex)
            {
                IsThrottled = true;
                ThrottledException = ex;
            }
            finally
            {
                ThList = _serv.GetCounterList().Select(r => ThrottlingInfo.GetThrottlingInfo(r));
            }
        }

        /**
         * Definition
         */
        public class ThrottlingInfo
        {
            public string Id { get; }
            public string Name { get; }
            public int Burst { get; }
            public int RsCount { get; }
            public int RsWait { get; }
            public bool HasHRestrict { get; }
            public int Hourly { get; }
            public DateTime? ResetOn { get; }
            ThrottlingInfo(IThrottlingCounter c)
            {
                Id = c.Id;
                Name = c.Name;
                Burst = c.Burst;
                RsCount = c.RestoreCount;
                RsWait = c.RestoreWaitingTimeInSeconds;
                HasHRestrict = c.HasHourlyRestoring;
                Hourly = c.HourlyRestoreCount;
                ResetOn = c.ResetOn;
            }
            public static ThrottlingInfo GetThrottlingInfo(IThrottlingCounter c)
            {
                return new ThrottlingInfo(c);
            }
        }
    }
}
