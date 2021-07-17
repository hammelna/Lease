using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Lease.Api.Models;
using System.Collections.Generic;
using Lease.Api.DataAccess.Repositories;

namespace Service.Lease.Api.Controllers.V1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LeaseController : ControllerBase
    {
        private ILeaseRepository _repo;
        public LeaseController(ILeaseRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaseModel>>> GetLeases()
        {
            var leases = await _repo.GetLeases();

            return Ok(leases);
        }
    }
}
