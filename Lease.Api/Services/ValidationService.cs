using Lease.Api.Models;
using System;

namespace Lease.Api.Services
{
    public class ValidationService : IValidationService
    {
        private const double MaxPaymentAmount = 1000000000;
        private const double MinPaymentAmount = -1000000000;

        public UploadRecordStatus ValidateRecord(string[] record)
        {
            var recordStatus = new UploadRecordStatus
            {
                Status = Status.INVALID,
                ErrorMessage = "",
                Lease = null
            };

            if(record == null)
            {
                recordStatus.ErrorMessage = "Record was empty, expecting 6 entries";
                return recordStatus;
            }

            if (record.Length != 6)
            {
                recordStatus.ErrorMessage = $"Expected 6 entries, found {record.Length}";
                return recordStatus;
            }

            if (string.IsNullOrWhiteSpace(record[0]))
            {
                recordStatus.ErrorMessage = "Name Of Lease Is Required";
                return recordStatus;
            }

            recordStatus.Lease = new LeaseModel { Name = record[0] };

            var isDateParsed = DateTime.TryParseExact(record[1], "d", null, System.Globalization.DateTimeStyles.None, out var startDate);
            if (isDateParsed)
            {
                recordStatus.Lease.StartDate = startDate;
            }
            else
            {
                recordStatus.ErrorMessage = "Start date required in format MM/dd/yyyy";
                return recordStatus;
            }

            isDateParsed = DateTime.TryParseExact(record[2], "d", null, System.Globalization.DateTimeStyles.None, out var endDate);
            if (!isDateParsed)
            {
                recordStatus.ErrorMessage = "End date required in format MM/dd/yyyy";
                return recordStatus;
            }

            if (startDate > endDate)
            {
                recordStatus.ErrorMessage = "End date must be before start date";
                return recordStatus;
            }

            recordStatus.Lease.EndDate = endDate;

            var isPaymentAmountNumeric = double.TryParse(record[3], out var paymentAmount);

            if (!isPaymentAmountNumeric || paymentAmount <= MinPaymentAmount || paymentAmount >= MaxPaymentAmount)
            {
                recordStatus.ErrorMessage = $"Payment amount is required and must be between {MinPaymentAmount} and {MaxPaymentAmount}";
                return recordStatus;
            }

            recordStatus.Lease.PaymentAmount = paymentAmount;

            var isNumberOfPaymentsNumeric = int.TryParse(record[4], out var numberOfPayments);
            if (!isNumberOfPaymentsNumeric || numberOfPayments <= 0 || false)
            {
                recordStatus.ErrorMessage = "Number of Payments must be greater than 0";
                return recordStatus;
            }

            if (numberOfPayments > fullMonthsInLease(startDate, endDate))
            {
                recordStatus.ErrorMessage = "Number of Payments must be less than total number of months in lease";
                return recordStatus;
            }

            recordStatus.Lease.NumberOfPayments = numberOfPayments;

            var isInterstRateNumeric = double.TryParse(record[5], out var interestRate);
            if (!isInterstRateNumeric || interestRate < 0 || interestRate > 9.9999)
            {
                recordStatus.ErrorMessage = "Interest Rate must be greater than 0 and less than 999.99%, formatted as the decimal equivalent (1% = 0.01)";
                return recordStatus;
            }

            recordStatus.Lease.InterestRate = interestRate;

            recordStatus.Status = Status.VALID;

            return recordStatus;
        }

        private int fullMonthsInLease(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year) * 12 +
                (endDate.Month - startDate.Month) + 1;
        }
    }
}
