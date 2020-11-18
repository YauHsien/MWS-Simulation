using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim
{
    public class ThrottlingValueObject : IDatabaseTable
    {
        public byte ApiTypeId { get; set; }
        public string Operation { get; set; }
        public string ReportType { get; set; }
        public int MaximumRequestQuota { get; set; }
        public int RestoreAmount { get; set; }
        public int RestoreWaitingTimeInSeconds { get; set; }
        public int HourlyRequestQuota { get; set; }

        string IDatabaseTable.Schema => "dbo";

        string IDatabaseTable.TableName => "Throttling";

        IEnumerable<string> IDatabaseTable.PivotFields => new List<string>();
    }
}
