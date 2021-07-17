using Lease.Api.Models;

namespace Lease.Api.Services
{
    public interface IValidationService
    {
        UploadRecordStatus ValidateRecord(string[] record);
    }
}
