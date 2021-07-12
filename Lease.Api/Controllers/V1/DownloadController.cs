using Lease.Api.Models;
using Lease.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lease.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadService _downloadService;
        public DownloadController(IDownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        //[HttpGet]
        //public async Task<ActionResult<IFormFile>> GetMonthlyPaymentsExport()
        //{
        //    IFormFile download = await _downloadService.DownloadMonthlyPayments();

        //    File file = 

        //    return Ok(download);
        //}

        [HttpGet]
        public async Task<IActionResult> DownloadPayments(string startDate, string endDate)
        {
            bool isValidDate = DateTime.TryParse(startDate, out var start);
            if (!isValidDate)
            {
                return BadRequest("Invalid Start Date");
            }

            isValidDate = DateTime.TryParse(endDate, out var end);
            if (!isValidDate)
            {
                return BadRequest("Invalid End Date");
            }

            if(start > end)
            {
                return BadRequest("End date cannot be before start date.");
            }

            var monthlyLeasePayment = await _downloadService.DownloadMonthlyPayments(start, end);
            return File(monthlyLeasePayment, "text/csv", "DownloadExample.csv");
        }
    }
}
