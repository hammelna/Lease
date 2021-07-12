using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lease.Api.Models
{
    public class UploadStatus
    {
        public string Status { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
