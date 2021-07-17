using Lease.Api.DataAccess.Repositories;
using Lease.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lease.Api.Services
{
    public class UploadService : IUploadService
    {
        private readonly IValidationService _validationService;
        private readonly ILeaseRepository _repo;

        public UploadService(IValidationService validationService, ILeaseRepository repo)
        {
            _validationService = validationService;
            _repo = repo;
        }

        public async Task<UploadStatus> ReadUploadFile(IFormFile upload)
        {
            var leases = new List<LeaseModel>();

            using var streamReader = new StreamReader(upload.OpenReadStream());

            var headerRecord = await streamReader.ReadLineAsync();

            while(!streamReader.EndOfStream)
            {
                var recordStatus = await parseLine(streamReader);

                if(recordStatus.Status.Equals(Status.INVALID))
                {
                    return new UploadStatus { Status = Status.FAILED, ErrorMessage = recordStatus.ErrorMessage };
                }

                leases.Add(recordStatus.Lease);
            }

            await _repo.CreateLeases(leases);

            return new UploadStatus { Status = Status.SUCCESS };
        }

        private async Task<UploadRecordStatus> parseLine(StreamReader reader)
        {
            string line = await reader.ReadLineAsync();
            return _validationService.ValidateRecord(line.Split(','));
        }
    }
}
