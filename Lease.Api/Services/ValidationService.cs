using Lease.Api.Models;

namespace Lease.Api.Services
{
    public class ValidationService : IValidationService
    {
        public bool IsValidLeaseRecord(LeaseModel lease)
        {
            //lease is not null
            bool isValidLease = lease != null;

            //name is not null
            isValidLease = isValidLease && !string.IsNullOrWhiteSpace(lease.Name);

            //start date before end date
            isValidLease = isValidLease && lease.StartDate <= lease.EndDate;

            //payment amount is greater than -1B and less than 1B
            isValidLease = isValidLease && lease.PaymentAmount > -1000000000 && lease.PaymentAmount < 1000000000;

            isValidLease = isValidLease && lease.NumberOfPayments > 0;
            //this needs work
            if(isValidLease)
            {
                var monthsInLease = lease.EndDate.Subtract(lease.StartDate);
            }

            //interest rate is greater than 0 and less that 9.9999 (>0% < 999.99%)
            isValidLease = isValidLease && lease.InterestRate > 0 && lease.InterestRate <= 9.999; 

            return isValidLease;
        }

        public bool IsValidUploadRecord(string[] record)
        {
            return true;
        }
    }
}
