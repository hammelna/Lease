using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Lease.Api.Models
{
    public class LeaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PaymentAmount { get; set; }
        public int NumberOfPayments { get; set; }
        public double InterestRate { get; set; }

        //public static Task<List<LeaseModel>> GenerateMockData()
        //{
        //    return Task.FromResult(new List<LeaseModel>
        //    {
        //        new LeaseModel { Name = "Lease One", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1), PaymentAmount=500.00, NumberOfPayments=12, InterestRate=0.05 },
        //        new LeaseModel { Name = "Lease Two", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(2), PaymentAmount=600.00, NumberOfPayments=24, InterestRate=0.045 },
        //        new LeaseModel { Name = "Lease Three", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(3), PaymentAmount=700.00, NumberOfPayments=36, InterestRate=0.04 },
        //        new LeaseModel { Name = "Lease Four", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(4), PaymentAmount=800.00, NumberOfPayments=48, InterestRate=0.03 },
        //        new LeaseModel { Name = "Lease Five", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(5), PaymentAmount=1000.00, NumberOfPayments=60, InterestRate=0.025 }
        //    });
        //}
    }
}
