using Lease.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lease.Api.DataAccess.Repositories
{
    public interface ILeaseRepository
    {
        Task<LeaseModel> CreateLease(LeaseModel lease);
        Task CreateLeases(IEnumerable<LeaseModel> leases);
        Task<IEnumerable<LeaseModel>> GetLeases();
        Task<LeaseModel> GetLease(int id);
        Task<IEnumerable<LeaseModel>> GetLeasesInDateRange(DateTime start, DateTime end);
    }
}
