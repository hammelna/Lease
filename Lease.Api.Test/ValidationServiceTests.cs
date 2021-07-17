using Lease.Api.Models;
using Lease.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Lease.Api.Test
{
    [TestClass]
    public class ValidationServiceTests
    {
        IValidationService _validationService;
        string[] _exampleRecordEntries;

        [TestInitialize()]
        public void Setup()
        {
            _validationService = new ValidationService();
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
        public void ValidLease()
        {
            var expected = new UploadRecordStatus { Status = Status.VALID };
            
            var recordStatus = _validationService.ValidateRecord(_exampleRecordEntries.First().Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
        }

        [TestMethod]
        public void InvalidLease_LeaseIsNull()
        {
            var expected = new UploadRecordStatus { Status = Status.INVALID, ErrorMessage = "Record was empty, expecting 6 entries" };

            var recordStatus = _validationService.ValidateRecord(null);

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_LessThan6Entries()
        {
            var expected = new UploadRecordStatus { Status = Status.INVALID, ErrorMessage = "Expected 6 entries, found 5" };

            var record = "Lease 001,1/1/2020,12/31/2020,580,12";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_NameIsEmpty()
        {
            var expected = new UploadRecordStatus { Status = Status.INVALID, ErrorMessage = "Name Of Lease Is Required" };

            var record = ",1/1/2020,12/31/2020,580,12,0.06";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_InvalidStartDate()
        {
            var expected = new UploadRecordStatus { Status = Status.INVALID, ErrorMessage = "Start date required in format MM/dd/yyyy" };

            var record = "Lease,1-1-2020,12/31/2020,580,12,0.06";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_InvalidEndDate()
        {
            var expected = new UploadRecordStatus { Status = Status.INVALID, ErrorMessage = "End date required in format MM/dd/yyyy" };

            var record = "Lease,1/1/2020,12-31-2020,580,12,0.06";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_EndDateBeforeStartDate()
        {
            var expected = new UploadRecordStatus { Status = Status.INVALID, ErrorMessage = "End date must be before start date"};

            var record = "Lease,1/1/2021,12/31/2020,580,12,0.06";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_PaymentAmountGreaterThan1B()
        {
            double MaxPaymentAmount = 1000000000;
            double MinPaymentAmount = -1000000000;

            var expected = new UploadRecordStatus { 
                Status = Status.INVALID, 
                ErrorMessage = $"Payment amount is required and must be between {MinPaymentAmount} and {MaxPaymentAmount}"
            };

            var record = $"Lease,1/1/2020,12/31/2020,{MaxPaymentAmount},12,0.06";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_PaymentAmountLessThan1B()
        {
            double MaxPaymentAmount = 1000000000;
            double MinPaymentAmount = -1000000000;

            var expected = new UploadRecordStatus
            {
                Status = Status.INVALID,
                ErrorMessage = $"Payment amount is required and must be between {MinPaymentAmount} and {MaxPaymentAmount}"
            };

            var record = $"Lease,1/1/2020,12/31/2020,{MinPaymentAmount},12,0.06";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_NegativeNumberOfPayments()
        {
            var expected = new UploadRecordStatus
            {
                Status = Status.INVALID,
                ErrorMessage = "Number of Payments must be greater than 0"
            };

            var record = $"Lease,1/1/2020,12/31/2020,6000,-1,0.06";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_InterestRateLessThanZero()
        {
            var expected = new UploadRecordStatus
            {
                Status = Status.INVALID,
                ErrorMessage = "Interest Rate must be greater than 0 and less than 999.99%, formatted as the decimal equivalent (1% = 0.01)"
            };

            var record = $"Lease,1/1/2020,12/31/2020,6000,12,-0.01";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_InterestRateGreaterThan999Percent()
        {
            var expected = new UploadRecordStatus
            {
                Status = Status.INVALID,
                ErrorMessage = "Interest Rate must be greater than 0 and less than 999.99%, formatted as the decimal equivalent (1% = 0.01)"
            };

            var record = $"Lease,1/1/2020,12/31/2020,6000,12,10";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }

        [TestMethod]
        public void InvalidLease_NumberOfPaymentsGreaterThanNumberOfMonths()
        {
            var expected = new UploadRecordStatus
            {
                Status = Status.INVALID,
                ErrorMessage = "Number of Payments must be less than total number of months in lease"
            };

            var record = $"Lease,1/1/2020,12/31/2020,6000,13,0.01";
            var recordStatus = _validationService.ValidateRecord(record.Split(','));

            Assert.AreEqual(expected.Status, recordStatus.Status);
            Assert.AreEqual(expected.ErrorMessage, recordStatus.ErrorMessage);
        }
    }
}
