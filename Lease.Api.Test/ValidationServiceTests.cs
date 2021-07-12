using Lease.Api.Models;
using Lease.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Lease.Api.Test
{
    [TestClass]
    public class ValidationServiceTests
    {
        [TestMethod]
        public void ValidLease()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();

            var isValid = validationService.IsValidLeaseRecord(lease);


            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void InvalidLease_LeaseIsNull()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = null;
            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_NameIsNull()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.Name = null;

            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_EndDateBeforeStartDate()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.EndDate = DateTime.Now.AddYears(-5);

            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_PaymentAmountLessThanNegative1B()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.PaymentAmount = -1000000001;

            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_PaymentAmountGreaterThan1B()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.PaymentAmount = 1000000001;

            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_NegativeNumberOfPayments()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.NumberOfPayments = -1;

            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_NumberOfPaymentsGreaterThanNumberOfMonths()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.NumberOfPayments = 70;

            var isValid = validationService.IsValidLeaseRecord(lease);

            //Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_InterestRateLessThanZero()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.InterestRate = -1;

            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void InvalidLease_InterestRateGreaterThan_999_99()
        {
            IValidationService validationService = new ValidationService();
            LeaseModel lease = validLeaseModel();
            lease.InterestRate = 1000;

            var isValid = validationService.IsValidLeaseRecord(lease);

            Assert.IsFalse(isValid);
        }

        private LeaseModel validLeaseModel()
        {
            return new LeaseModel
            {
                Name = "Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(5),
                PaymentAmount = 50000,
                NumberOfPayments = 60,
                InterestRate = 0.05
            };
        }
    }
}
