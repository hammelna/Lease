using Lease.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Lease.Api.DataAccess.Repositories
{
    public class LeaseRepository : ILeaseRepository
    {
        private readonly LeaseContext _context;
        public LeaseRepository(LeaseContext context)
        {
            _context = context;
        }

        public async Task<LeaseModel> CreateLease(LeaseModel lease)
        {
            var result = await _context.Leases.AddAsync(lease);

            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public async Task CreateLeases(IEnumerable<LeaseModel> leases)
        {
            await _context.AddRangeAsync(leases);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LeaseModel>> GetLeases()
        {
            var leases = await _context.Leases.OrderBy(lease => lease.StartDate).ToListAsync();
            return leases;
        }

        public async Task<LeaseModel> GetLease(int id)
        {
            var lease = await _context.Leases.FindAsync(id);
            return lease;
        }

        // Consider making this into its own table with MontlyPayment records
        public async Task<IEnumerable<LeaseModel>> GetLeasesInDateRange(DateTime start, DateTime end)
        {
            var query = _context.Leases
                .Where(lease =>
                    (lease.StartDate <= start && start <= lease.EndDate) ||
                    (lease.StartDate <= end && end <= lease.EndDate) ||
                    (start <= lease.StartDate && lease.EndDate <= end));
            
            var results = await query.ToListAsync();

            return results;
        }
    }
}
