namespace Lease.Api.Models
{
    public class MonthlyPaymentModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string FullMonth { get; set; }
        public double PaymentAmount { get; set; }
        public double InterestPaymentAmount { get; set; }
        public double TotalMonthlyPayment { get; set; }
    }
}
