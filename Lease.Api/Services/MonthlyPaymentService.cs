using Lease.Api.DataAccess.Repositories;
using Lease.Api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Lease.Api.Services
{
    public class MonthlyPaymentService : IMonthlyPaymentService
    {
        private ILeaseRepository _repo;
        public MonthlyPaymentService(ILeaseRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<MonthlyPaymentModel>> MonthlyPaymentScheduleInDateRange(
            DateTime start, DateTime end)
        {
            var aggregatedMonthlyPaymentsPerYear = (await _repo.GetLeasesInDateRange(start, end))
                    .SelectMany(lease => MonthlyPaymentScheduleForLease(lease))
                    .GroupBy(key => key.Year, value => value, (key, values) =>
                    {
                        var results = values.GroupBy(month => month.Month, monthValue => monthValue, (month, monthValues) =>
                            monthValues.Aggregate((p1, p2) => new MonthlyPaymentModel
                            {
                                Year = p1.Year,
                                Month = p1.Month,
                                FullMonth = p1.FullMonth,
                                PaymentAmount = p1.PaymentAmount + p2.PaymentAmount,
                                InterestPaymentAmount = p1.InterestPaymentAmount + p2.InterestPaymentAmount,
                                TotalMonthlyPayment = p1.TotalMonthlyPayment + p2.TotalMonthlyPayment
                            })
                        );

                        return new KeyValuePair<int, IEnumerable<MonthlyPaymentModel>>(key, results);
                    }).ToDictionary(k => k.Key, v => v.Value);

            var resultSet = new List<MonthlyPaymentModel>();
            foreach (var year in aggregatedMonthlyPaymentsPerYear.Keys.Where(year => year >= start.Year && year <= end.Year))
            {
                var startMonth = year == start.Year ? start.Month : 1;
                var endMonth = year == end.Year ? end.Month : 12;
                var validMonths = aggregatedMonthlyPaymentsPerYear[year].Where(mp => mp.Month >= startMonth && mp.Month <= endMonth);
                resultSet.AddRange(validMonths);
            }

            return resultSet.OrderBy(mp => mp.Year).ThenBy(mp => mp.Month);
        }

        public IEnumerable<MonthlyPaymentModel> MonthlyPaymentScheduleForLease(LeaseModel lease)
        {
            var monthlyPayments = new List<MonthlyPaymentModel>();
            var year = lease.StartDate.Year;
            var month = lease.StartDate.Month;

            for (int i = lease.NumberOfPayments; i > 0; i--)
            {
                var interestPayment = i * lease.PaymentAmount * lease.InterestRate;

                var monthlyPayment = new MonthlyPaymentModel
                {
                    Year = year,
                    Month = month,
                    FullMonth = DateTimeFormatInfo.CurrentInfo.GetMonthName(month),
                    PaymentAmount = lease.PaymentAmount,
                    InterestPaymentAmount = interestPayment,
                    TotalMonthlyPayment = interestPayment + lease.PaymentAmount
                };

                monthlyPayments.Add(monthlyPayment);

                year = month == 12 ? year + 1 : year;
                month = month == 12 ? 1 : month + 1;
            }

            return monthlyPayments;
        }
    }
}
