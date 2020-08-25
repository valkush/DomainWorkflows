using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiCall.Domain
{
    public class Call
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? DriverId { get; set; }

        public CallInfo Info { get; set; }

        public CallStatus Status { get; set; }
    }
}
