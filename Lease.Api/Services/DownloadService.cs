using Lease.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lease.Api.Services
{
    public class DownloadService : IDownloadService
    {
        private IMonthlyPaymentService _paymentService;
        public DownloadService(IMonthlyPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<Stream> DownloadMonthlyPayments(DateTime startDate, DateTime endDate)
        {
            var monthlyPayments = await _paymentService
                .MonthlyPaymentScheduleInDateRange(startDate, endDate);
         
            var paymentStream = new MemoryStream();

            using var memoryStream = new MemoryStream();

            using var writer = new StreamWriter(memoryStream);
            
            await writeHeader(writer);
            await writeBody(writer, monthlyPayments);
            await positionAndCopyStream(writer, memoryStream, paymentStream);
            
            return paymentStream;
        }

        private Task writeHeader(StreamWriter writer)
        {
            return writer.WriteLineAsync("Year,Month,Lease Payment,Interest Payment,Total Payment");
        }

        private async Task writeBody(StreamWriter writer, IEnumerable<MonthlyPaymentModel> monthlyPayments)
        {
            foreach(var mp in monthlyPayments)
            {
                var entry = $"{mp.Year},{mp.FullMonth},{mp.PaymentAmount.ToString("F")},{mp.InterestPaymentAmount.ToString("F")},{mp.TotalMonthlyPayment.ToString("F")}";
                await writer.WriteLineAsync(entry);
            }
        }

        private async Task positionAndCopyStream(StreamWriter writer, Stream memoryStream, Stream paymentStream)
        {
            await writer.FlushAsync();
            
            memoryStream.Position = 0;

            await memoryStream.CopyToAsync(paymentStream);

            paymentStream.Position = 0;
        }
    }
}
