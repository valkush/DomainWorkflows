using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiCall.Domain
{
    public enum CallStatus
    {
        Pending,
        Canceled,
        Closed, 
        Expired
    }
}
