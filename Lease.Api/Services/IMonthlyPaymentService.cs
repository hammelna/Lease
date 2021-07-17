using Lease.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lease.Api.Services
{
    public interface IMonthlyPaymentService
    {
        IEnumerable<MonthlyPaymentModel> MonthlyPaymentScheduleForLease(LeaseModel lease);
        Task<IEnumerable<MonthlyPaymentModel>> MonthlyPaymentScheduleInDateRange(DateTime start, DateTime end);
    }
}
