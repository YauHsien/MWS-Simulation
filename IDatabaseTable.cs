using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim
{
    public interface IDatabaseTable
    {
        string Schema { get; }
        string TableName { get; }
        IEnumerable<string> PivotFields { get; }
    }
}
