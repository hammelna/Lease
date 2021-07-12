using Lease.Api.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lease.Api.Services
{
    public interface IUploadService
    {
        Task<UploadStatus> ReadUploadFile(IFormFile upload);
    }
}
