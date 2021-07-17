using Lease.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Lease.Api.Controllers.V1
{
    [Route("api/v1/lease/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadService _downloadService;
        public DownloadController(IDownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPayments(DateTime startDate, DateTime endDate)
        {
            if(startDate > endDate)
            {
                return BadRequest("End date cannot be before start date.");
            }

            var monthlyLeasePayment = await _downloadService.DownloadMonthlyPayments(startDate, endDate);

            string filename = $"PaymentSchedule_{startDate.ToString("MM-dd-yyyy")}_{endDate.ToString("MM-dd-yyyy")}.csv";

            return File(monthlyLeasePayment, "text/csv", filename);
        }
    }
}
