using Microsoft.Extensions.Configuration;
using mws_sim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim
{
    internal class ThrottlingService : IThrottlingService
    {
        private readonly IDictionary<string, IThrottlingCounter> _dict;

        public ThrottlingService(IRepository<ThrottlingValueObject> thRepo)
        {
            var _coll =
                thRepo.List()
                .Select(r => (IThrottlingCounter)(ThrottlingCounter)r);
            _dict =
                _coll
                .ToDictionary(r => r.Id);
        }

        public IEnumerable<IThrottlingCounter> GetCounterList()
        {
            return _dict.Values;
        }

        public void HandleInvoke(string throttlingId)
        {
            var c = _dict[throttlingId];
            c.Apply();
        }
    }
}
