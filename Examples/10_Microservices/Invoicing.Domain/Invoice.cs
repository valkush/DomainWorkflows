using System;

namespace Invoicing.Domain
{
    public class Invoice
    {
        public const int NewId = 0;        

        public Invoice(int id, decimal total, DateTimeOffset dueDate, string customerId)
        {
            this.Id = id;
            this.Total = total;            
            this.DueDate = dueDate;
            this.CustomerId = customerId;
            this.Status = InvoiceStatus.Pending;
        }

        public int Id { get; set; }

        public decimal Total { get; private set; }
        public DateTimeOffset DueDate { get; private set; }

        public string CustomerId { get; private set; }

        public InvoiceStatus Status { get; private set; }


        public void Update(decimal total)
        {
            if (this.Status != InvoiceStatus.Pending)
                throw new InvalidOperationException($"{this.Status} invoice cannot be updated.");

            this.Total = total;
        }

        public void Overdue()
        {
            if (this.Status != InvoiceStatus.Pending)
                throw new InvalidOperationException($"{this.Status} invoice cannot be overdued.");

            this.Status = InvoiceStatus.Overdue;
        }

        public void SetPaid()
        {
            if (this.Status != InvoiceStatus.Pending)
                throw new InvalidOperationException($"{this.Status} invoice cannot be paid.");

            this.Status = InvoiceStatus.Paid;
        }

        public void SetFaulted()
        {
            if (this.Status != InvoiceStatus.Pending)
                throw new InvalidOperationException($"{this.Status} invoice cannot be faulted.");

            this.Status = InvoiceStatus.Faulted;
        }
    }
}
