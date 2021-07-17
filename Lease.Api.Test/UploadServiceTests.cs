using Lease.Api.DataAccess.Repositories;
using Lease.Api.Models;
using Lease.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lease.Api.Test
{
    [TestClass]
    public class UploadServiceTests
    {
        private Mock<IValidationService> _validationServiceMock;
        private Mock<ILeaseRepository> _leaseRepositoryMock;
        string[] _exampleRecordEntries;

        [TestInitialize]
        public void Setup()
        {
            _validationServiceMock = new Mock<IValidationService>();
            _leaseRepositoryMock = new Mock<ILeaseRepository>();
            _exampleRecordEntries = new string[]
            {
                "Lease 001,1/1/2020,12/31/2020,580,12,0.06",
                "Lease 002,8/15/2019,8/15/2025,1385,50,0.045",
                "Lease 003,1/1/2000,9/1/2019,950,108,0.11",
                "Lease 004,11/18/2020,10/1/2021,5832,10,0.0865",
                "Lease 005,3/7/2025,5/20/2025,200,3,0.007"
            };
        }

        [TestMethod]
        public async Task ReadUploadFile_Success()
        {

            var expected = new UploadStatus { Status = Status.SUCCESS };
            
            var uploadService = new UploadService(_validationServiceMock.Object, _leaseRepositoryMock.Object);
            var bytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, _exampleRecordEntries));
            
            using var memoryStream = new MemoryStream(bytes);
            
            var file = new FormFile(
                baseStream: memoryStream,
                baseStreamOffset: 0,
                length: bytes.Length,
                name: "Data",
                fileName: "dummy.csv"
            );

            _validationServiceMock
                .Setup(validation => validation.ValidateRecord(It.IsAny<string[]>()))
                .Returns(new UploadRecordStatus { Status = Status.VALID, ErrorMessage = string.Empty, Lease = new LeaseModel() });

            var result = await uploadService.ReadUploadFile(file);

            Assert.AreEqual(expected.Status, result.Status);
        }
    }
}
