namespace Lease.Api.Models
{
    public class UploadRecordStatus
    {
        public string Status { get; set; }
        public LeaseModel Lease { get; set; }
        public string ErrorMessage { get; set; }
    }
}
