using System;

namespace Lease.Api.Models
{
    public class LeaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PaymentAmount { get; set; }
        public int NumberOfPayments { get; set; }
        public double InterestRate { get; set; }
    }
}
