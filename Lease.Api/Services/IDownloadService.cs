using System;
using System.IO;
using System.Threading.Tasks;

namespace Lease.Api.Services
{
    public interface IDownloadService
    {
        Task<Stream> DownloadMonthlyPayments(DateTime startDate, DateTime endDate);
    }
}
