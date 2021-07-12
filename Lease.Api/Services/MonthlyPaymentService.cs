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

        public async Task<IEnumerable<MonthlyPaymentModel>> MonthlyPaymentScheduleInDateRange(DateTime start, DateTime end)
        {
            var leases = await _repo.GetLeasesInDateRange(start, end);

            var monthlyPaymentsPerYear = leases
                .SelectMany(lease => MonthlyPaymentScheduleForLease(lease))
                .GroupBy(key => key.Year, value => value, (key, values) =>
                    new KeyValuePair<int, IEnumerable<MonthlyPaymentModel>>(key, values))
                .ToDictionary(k => k.Key, v => v.Value);

            var results = new List<MonthlyPaymentModel>();

            foreach (var key in monthlyPaymentsPerYear.Keys)
            {
                int yearStart = key == start.Year ? start.Month : 1;
                int yearEnd = key == end.Year ? end.Month : 12;
                for (int month = yearStart; month <= yearEnd; month++)
                {
                    var aggregatedPayments = monthlyPaymentsPerYear[key].Where(pay => pay.Month == month)
                        .Aggregate((p1, p2) => new MonthlyPaymentModel
                        {
                            Year = p1.Year,
                            Month = p1.Month,
                            FullMonth = p1.FullMonth,
                            PaymentAmount = p1.PaymentAmount + p2.PaymentAmount,
                            InterestPaymentAmount = p1.InterestPaymentAmount + p2.InterestPaymentAmount,
                            TotalMonthlyPayment = p1.TotalMonthlyPayment + p2.TotalMonthlyPayment
                        });

                    results.Add(aggregatedPayments);
                }
            }

            return results.OrderBy(p => p.Year).ThenBy(p => p.Month);
        }

        //public async Task<IEnumerable<MonthlyPaymentModel>> MonthlyPaymentScheduleInDateRangeByMonthAndYearDictionaryNotAggregated(
        //    DateTime start, DateTime end)
        //{
        //    var monthlyPaymentsPerYearAndPerMonth = (await _repo.GetLeasesInDateRange(start, end))
        //       .SelectMany(lease => MonthlyPaymentScheduleForLease(lease))
        //       .GroupBy(key => key.Year, value => value, (key, values) =>
        //       {
        //           var results = values.GroupBy(month => month.Month, monthValue => monthValue, (month, monthValues) =>
        //               new KeyValuePair<int, IEnumerable<MonthlyPaymentModel>>(key, values))
        //               .ToDictionary(k => k.Key, v => v.Value);

        //           return new KeyValuePair<int, Dictionary<int, IEnumerable<MonthlyPaymentModel>>>(key, results);
        //       })
        //       .ToDictionary(k => k.Key, v => v.Value);
        //}

        public async Task<IEnumerable<MonthlyPaymentModel>> MonthlyPaymentScheduleInDateRangeByMonthAndYearDictionaryAggregated(
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
            foreach(var year in aggregatedMonthlyPaymentsPerYear.Keys.Where(year => year >= start.Year && year <= end.Year))
            {
                var startMonth = year == start.Year ? start.Month : 1;
                var endMonth = year == end.Year ? end.Month : 12;
                var validMonths = aggregatedMonthlyPaymentsPerYear[year].Where(mp => mp.Month >= startMonth && mp.Month <= endMonth);
                resultSet.AddRange(validMonths);
            }

            return resultSet.OrderBy(mp => mp.Year).ThenBy(mp => mp.Month);
        }
    }
}


//    var results = values.GroupBy(
//        month => month.Month, 
//        monthValue => monthValue, 
//        (month, monthValues) =>
//        {
//            var monthResults = monthValues.Aggregate((p1, p2) => new MonthlyPaymentModel
//             {
//                 Year = p1.Year,
//                 Month = p1.Month,
//                 FullMonth = p1.FullMonth,
//                 PaymentAmount = p1.PaymentAmount + p2.PaymentAmount,
//                 InterestPaymentAmount = p1.InterestPaymentAmount + p2.InterestPaymentAmount,
//                 TotalMonthlyPayment = p1.TotalMonthlyPayment + p2.TotalMonthlyPayment
//             });
//        }
//        new KeyValuePair<int, IEnumerable<MonthlyPaymentModel>>(key, values))
//        .ToDictionary(k => k.Key, v => v.Value);

//    return new KeyValuePair<int, IEnumerable<MonthlyPaymentModel>>(key, results);
//})
//.ToDictionary(k => k.Key, v => v.Value);



//var allPaymentsForLeasesInRange2 = leases
//    .SelectMany(lease => MonthlyPaymentScheduleForLease(lease)
//        .Where(paymentSchedule => start.Year <= paymentSchedule.Year && paymentSchedule.Year <= end.Year)).ToList();

//var dict = allPaymentsForLeasesInRange2.ToDictionary(k => k.Year, v => v);

//var monthlyPaymentsByYear = allPaymentsForLeasesInRange2.GroupBy(
//    lease => lease.Year, v => v,
//    (key, value) => new KeyValuePair<int, IEnumerable<MonthlyPaymentModel>>(key, value))
//    .ToDictionary(x => x.Key, x => x.Value);

//var results = new List<MonthlyPaymentModel>();
//foreach(var group in monthlyPaymentsByYear)
//{
//    var monthStart = group.Key == start.Year ? start.Month : 1;
//    var monthEnd = group.Key == end.Year ? end.Month : 12;
//    for(int i = monthStart; i <= monthEnd; i++)
//    {
//        var payments = group.va
//    }
//}
//for(int year = start.Year; year <= end.Year; year++)
//{

//    for(int month = start.Month; month <= 12; month++)
//    {
//        var payments = allPaymentsForLeasesInRange2.Where(pay => pay.Year == year && pay.Month == month);
//        var aggregated = new MonthlyPaymentModel
//        {
//            Year = year,
//            Month = month,
//            FullMonth = DateTimeFormatInfo.CurrentInfo.GetMonthName(month),
//            PaymentAmount = payments.Sum(p => p.PaymentAmount),
//            InterestPaymentAmount = payments.Sum(p => p.InterestPaymentAmount),
//            TotalMonthlyPayment = payments.Sum(p => p.TotalMonthlyPayment)
//        };
//        results.Add(aggregated);
//    }
//}



//Dictionary<int, Dictionary<int, MonthlyPaymentModel>> resultSet = new Dictionary<int, Dictionary<int, MonthlyPaymentModel>>();

//int year = startRange.Year;
//int month = startRange.Month;
//while (year <= endRange.Year)
//{
//    resultSet.Add(year, new Dictionary<int, MonthlyPaymentModel>());


//    while ((month <= 12 && year != endRange.Year) || (month <= endRange.Month))
//    {
//        var monthString = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
//        //can potentially go through the leases here
//        //presuming that the payments left are payments - (current month-start month)

//        resultSet[year].Add(month, new MonthlyPaymentModel { Year = year, Month = month, FullMonth = monthString, InterestPaymentAmount = 0, PaymentAmount = 0, TotalMonthlyPayment = 0 });
//        month++;
//    }
//    month = 1;
//    year++;
//}

//foreach (var monthlyPayment in monthlyPayments)
//{
//    if (resultSet.ContainsKey(monthlyPayment.Year) && resultSet[monthlyPayment.Year].ContainsKey(monthlyPayment.Month))
//    {
//        var aggregatedPayment = resultSet[monthlyPayment.Year][monthlyPayment.Month];
//        aggregatedPayment.PaymentAmount += monthlyPayment.PaymentAmount;
//        aggregatedPayment.InterestPaymentAmount += monthlyPayment.InterestPaymentAmount;
//        aggregatedPayment.TotalMonthlyPayment += monthlyPayment.TotalMonthlyPayment;
//    }
//}

//var allPaymentsInRange = new List<MonthlyPaymentModel>();
//foreach (var value in resultSet.Values)
//{
//    allPaymentsInRange.AddRange(value.Values);
//}

//return allPaymentsInRange.OrderBy(pay => pay.Year).ThenBy(pay => pay.Month);
