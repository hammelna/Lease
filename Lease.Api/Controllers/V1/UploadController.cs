using Lease.Api.Models;
using Lease.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lease.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private IUploadService _uploadService;
        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost]
        public async Task<ActionResult<UploadStatus>> UploadLeaseCsv(IFormFile upload)
        {
            var uploadStatus = await _uploadService.ReadUploadFile(upload);

            return uploadStatus.Status.Equals(Status.SUCCESS) ?
                Ok(uploadStatus) :
                BadRequest(uploadStatus);
        }
    }
}
