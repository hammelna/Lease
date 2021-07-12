using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lease.Api.Models
{
    public class UploadRecordStatus
    {
        public string Status { get; set; }
        public LeaseModel Lease { get; set; }
        public string ErrorMessage { get; set; }
    }
}
