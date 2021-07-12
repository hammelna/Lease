using Lease.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lease.Api.Test
{
    [TestClass]
    public class UploadServiceTests
    {
        [TestMethod]
        public void TestMonthCalc()
        {
            UploadService service = new UploadService(null);

            var startDate = new DateTime(2020, 1, 1);
            var endDate = new DateTime(2021, 1, 1);

            var result = service.fullMonthsInLease(startDate, endDate);


            startDate = new DateTime(2020, 1, 1);
            endDate = new DateTime(2020, 12, 31);

            result = service.fullMonthsInLease(startDate, endDate);


            //var startDate = new DateTime(2020, 1, 1);
            //var endDate = new DateTime(2021, 1, 1);

            //service.fullMonthsInLease(startDate, endDate);


            //var startDate = new DateTime(2020, 1, 1);
            //var endDate = new DateTime(2021, 1, 1);

            //service.fullMonthsInLease(startDate, endDate);


            //var startDate = new DateTime(2020, 1, 1);
            //var endDate = new DateTime(2021, 1, 1);

            //service.fullMonthsInLease(startDate, endDate);
        }

        [TestMethod]
        public void ParseLease()
        {
            string entry = "LeaseName,1/2/2020,1/1/2021,720,12,0.05"; 
            var uploadService = new UploadService(null);
            var status = uploadService.parseLeaseEntry(entry.Split(','));

            Assert.AreEqual("Valid", status.Status);
        }
    }
}
