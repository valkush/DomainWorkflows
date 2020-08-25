using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiCall.Events
{
    public class CallRequested
    {
        public int CustomerId { get; set; }

        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
    }
}
