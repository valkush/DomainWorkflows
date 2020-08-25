using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiCall.Events
{
    public class CallPickedUp
    {
        public int CallId { get; set; }
        public int DriverId { get; set; }
    }
}
