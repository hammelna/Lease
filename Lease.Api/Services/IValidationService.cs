using Lease.Api.Models;

namespace Lease.Api.Services
{
    public interface IValidationService
    {
        bool IsValidLeaseRecord(LeaseModel lease);

        bool IsValidUploadRecord(string[] record);
    }
}
