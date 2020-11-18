using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mws_sim
{
    public class RequstThrottledException : Exception
    {
        public RequstThrottledException(string message) : base(message)
        {
        }
        public override string ToString()
        {
            return this.Message;
        }
    }
}
