using Lease.Api.DataAccess.Repositories;
using Lease.Api.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lease.Api.Services
{
    public class DownloadService : IDownloadService
    {
        private ILeaseRepository _repo;
        private IMonthlyPaymentService _paymentService;
        public DownloadService(ILeaseRepository repo, IMonthlyPaymentService paymentService)
        {
            _repo = repo;
            _paymentService = paymentService;
        }

        public async Task<Stream> DownloadMonthlyPayments(DateTime startDate, DateTime endDate)
        {
            //var leasesInRange = await _repo.GetLeasesInDateRange(startDate, endDate);
            //var monthlyPaymentExample = _paymentService.MonthlyPaymentScheduleForLease(leasesInRange.First());
            //var monthlyPayments = await _paymentService.MonthlyPaymentScheduleInDateRange(startDate, endDate);
            var monthlyPaymentsComplicated = await _paymentService
                .MonthlyPaymentScheduleInDateRangeByMonthAndYearDictionaryAggregated(startDate, endDate);
            //var somethingElse = await _paymentService.MonthlyPaymentScheduleInDateRange(startDate, endDate);
            //var payments = generateMonthlyPayments(leasesInRange);

            //var orderedPayments = monthlyPayments(payments, startDate, endDate);

            var paymentStream = new MemoryStream();

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);

            await writer.WriteLineAsync("Year,Month,Lease Payment,Interest Payment,Total Payment");

            monthlyPaymentsComplicated
                .Select(p => $"{p.Year},{p.FullMonth},{p.PaymentAmount},{p.InterestPaymentAmount},{p.TotalMonthlyPayment}")
                .ToList()
                .ForEach(entry => writer.WriteLineAsync(entry));
            
            await writer.FlushAsync();
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(paymentStream);

            paymentStream.Position = 0;
            return paymentStream;
        }

        private IEnumerable<MonthlyPaymentModel> monthlyPayments(IEnumerable<MonthlyPaymentModel> monthlyPayments, DateTime startRange, DateTime endRange)
        {
            Dictionary<int, Dictionary<int, MonthlyPaymentModel>>  resultSet = new Dictionary<int, Dictionary<int, MonthlyPaymentModel>>();

            int year = startRange.Year;
            int month = startRange.Month;
            while (year <= endRange.Year)
            {
                resultSet.Add(year, new Dictionary<int, MonthlyPaymentModel>());
                

                while ( (month <= 12 && year != endRange.Year) || (month <= endRange.Month) )
                {
                    var monthString = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
                    //can potentially go through the leases here
                    //presuming that the payments left are payments - (current month-start month)

                    resultSet[year].Add(month, new MonthlyPaymentModel { Year = year, Month = month, FullMonth = monthString, InterestPaymentAmount = 0, PaymentAmount = 0, TotalMonthlyPayment = 0 });
                    month++;
                }
                month = 1;
                year++;
            }

            foreach(var monthlyPayment in monthlyPayments )
            {
                if(resultSet.ContainsKey(monthlyPayment.Year) && resultSet[monthlyPayment.Year].ContainsKey(monthlyPayment.Month))
                {
                    var aggregatedPayment = resultSet[monthlyPayment.Year][monthlyPayment.Month];
                    aggregatedPayment.PaymentAmount += monthlyPayment.PaymentAmount;
                    aggregatedPayment.InterestPaymentAmount += monthlyPayment.InterestPaymentAmount;
                    aggregatedPayment.TotalMonthlyPayment += monthlyPayment.TotalMonthlyPayment;
                }
            }

            var allPaymentsInRange = new List<MonthlyPaymentModel>();
            foreach(var value in resultSet.Values)
            {
                allPaymentsInRange.AddRange(value.Values);
            }

            return allPaymentsInRange.OrderBy(pay => pay.Year).ThenBy(pay => pay.Month);
        }

        private IEnumerable<MonthlyPaymentModel> generateMonthlyPayments(IEnumerable<LeaseModel> leases)
        {
            var monthlyPayments = new List<MonthlyPaymentModel>();

            foreach(var lease in leases)
            {
                var year = lease.StartDate.Year;
                var month = lease.StartDate.Month;

                for(int i = lease.NumberOfPayments; i > 0; i--)
                {
                    var interestPayment = i * lease.PaymentAmount * lease.InterestRate;

                    var monthlyPayment = new MonthlyPaymentModel
                    {
                        Year = year,
                        Month = month % 13,
                        FullMonth = DateTimeFormatInfo.CurrentInfo.GetMonthName(month % 13),
                        PaymentAmount = lease.PaymentAmount,
                        InterestPaymentAmount = interestPayment,
                        TotalMonthlyPayment = interestPayment + lease.PaymentAmount
                    };

                    monthlyPayments.Add(monthlyPayment);
                    
                    month++;
                    if (month == 13)
                    {
                        month = 1;
                        year++;
                    }
                }
            }

            return monthlyPayments;
        }
    }
}
