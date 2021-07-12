using Lease.Api.DataAccess.Repositories;
using Lease.Api.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleToAttribute("Lease.Api.Test")]
namespace Lease.Api.Services
{
    public class UploadService : IUploadService
    {
        private const double MaxPaymentAmount = 1000000000;
        private const double MinPaymentAmount = -1000000000;
        private ILeaseRepository _repo;
        public UploadService(ILeaseRepository repo)
        {
            _repo = repo;
        }

        public async Task<UploadStatus> ReadUploadFile(IFormFile upload)
        {
            var uploadStatus = new UploadStatus
            {
                Status = Status.SUCCESS,
                ErrorMessages = new List<string>()
            };

            var leases = new List<LeaseModel>();

            using var streamReader = new StreamReader(upload.OpenReadStream());

            var headerRecord = await streamReader.ReadLineAsync();

            while(!streamReader.EndOfStream)
            {
                string line = await streamReader.ReadLineAsync();
                var recordStatus = parseLeaseEntry(line.Split(','));

                if(recordStatus.Status.Equals(Status.INVALID))
                {
                    uploadStatus.ErrorMessages.Add(recordStatus.ErrorMessage);
                    uploadStatus.Status = Status.FAILED;
                    break;
                }

                leases.Add(recordStatus.Lease);
            }

            await _repo.CreateLeaess(leases);

            return uploadStatus;
        }

        internal UploadRecordStatus parseLeaseEntry(string[] entry)
        {
            var recordStatus = new UploadRecordStatus
            {
                Status = Status.INVALID,
                ErrorMessage = "",
                Lease = null
            };

            if(entry == null || entry.Length != 6)
            {
                recordStatus.ErrorMessage = $"Expected 6 entries, found {entry.Length}";
                return recordStatus;
            }

            if(string.IsNullOrWhiteSpace(entry[0]))
            {
                recordStatus.ErrorMessage = $"Name Of Lease Is Required";
                return recordStatus;
            }

            recordStatus.Lease = new LeaseModel { Name = entry[0] };

            var isDateParsed = DateTime.TryParseExact(entry[1], "d", null, System.Globalization.DateTimeStyles.None, out var startDate);
            if (isDateParsed)
            {
                recordStatus.Lease.StartDate = startDate;              
            } else
            {
                recordStatus.ErrorMessage = $"Start date required in format MM/dd/yyyy";
                return recordStatus;
            }

            isDateParsed = DateTime.TryParseExact(entry[2], "d", null, System.Globalization.DateTimeStyles.None, out var endDate);
            if(!isDateParsed)
            {
                recordStatus.ErrorMessage = $"End date required in format MM/dd/yyyy";
                return recordStatus;
            }

            if(startDate > endDate)
            {
                recordStatus.ErrorMessage = "End date must be before start date";
                return recordStatus;
            }

            recordStatus.Lease.EndDate = endDate;

            var isPaymentAmountNumeric = double.TryParse(entry[3], out var paymentAmount);

            if(!isPaymentAmountNumeric || paymentAmount < MinPaymentAmount || paymentAmount > MaxPaymentAmount)
            {
                recordStatus.ErrorMessage = $"Payment amount is required and must be between {MinPaymentAmount} and {MaxPaymentAmount}";
                return recordStatus;
            }

            recordStatus.Lease.PaymentAmount = paymentAmount;


            //here we need to calculate number of full months in payment timeframe
            var isNumberOfPaymentsNumeric = int.TryParse(entry[4], out var numberOfPayments);
            if(!isNumberOfPaymentsNumeric || numberOfPayments <= 0 || false)
            {
                recordStatus.ErrorMessage = "Number of Payments must be greater than 0";
                return recordStatus;
            }

            if(numberOfPayments > fullMonthsInLease(startDate, endDate))
            {
                recordStatus.ErrorMessage = "Number of Payments must be less than total number of months in lease";
                return recordStatus;
            }

            recordStatus.Lease.NumberOfPayments = numberOfPayments;

            var isInterstRateNumeric = double.TryParse(entry[5], out var interestRate);
            if(!isInterstRateNumeric && interestRate > 0 && interestRate <= 9.9999)
            {
                recordStatus.ErrorMessage = "Interest Rate must be greater than 0 and less than 999.99%, formatted as the decimal equivalent (1% = 0.01)";
                return recordStatus;
            }

            recordStatus.Lease.InterestRate = interestRate;

            recordStatus.Status = Status.VALID;

            return recordStatus;
        }

        internal int fullMonthsInLease(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year) * 12 +
                (endDate.Month - startDate.Month) + 1;
        }
    }
}
