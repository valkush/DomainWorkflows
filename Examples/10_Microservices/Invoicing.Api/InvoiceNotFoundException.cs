using System;

namespace Invoicing.Api
{
    public class InvoiceNotFoundException : Exception
    {
        public InvoiceNotFoundException() : base("Invoice is not found")
        {
        }
    }
}
