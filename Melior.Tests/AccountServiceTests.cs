using Melior.InterviewQuestion.Data;
using Melior.InterviewQuestion.Services;
using Melior.InterviewQuestion.Types;
using Moq;
using System.Configuration;

namespace Melior.Tests
{
    public class AccountServiceTests
    {
        private Mock<IAccountDataStore> _accountDataStore;
        private Mock<IBackupAccountDataStore> _backupAccountDataStore;
        private Account _account;
        private AccountService _accountService;
        private MakePaymentRequest _request;
        [SetUp]
        public void Setup()
        {
            _accountDataStore = new Mock<IAccountDataStore>();
            _backupAccountDataStore = new Mock<IBackupAccountDataStore>();
            _accountService = new AccountService(_accountDataStore.Object, _backupAccountDataStore.Object);
            _account = new Account();
            _request = new MakePaymentRequest();
        }


        [Test]
        public void GetAccountTest_DataTypeBackup()
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "Backup";
            _accountService.GetAccount(new MakePaymentRequest() { DebtorAccountNumber = "DebtorAccountNumber" });
            _backupAccountDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Once);
            _accountDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Never);
        }
        [Test]
        public void GetAccountTest_DataTypeNotBackup()
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "Not Backup";
            _accountService.GetAccount(new MakePaymentRequest() { DebtorAccountNumber = "DebtorAccountNumber" });
            _backupAccountDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Never);
            _accountDataStore.Verify(x => x.GetAccount(It.IsAny<string>()), Times.Once);
        }


        [Test]
        public void IsPaymentValid_Bacs_HasBacsFlag()
        {
            _request.PaymentScheme = PaymentScheme.Bacs;
            _account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.True(result);
        }
        [Test]
        public void IsPaymentValid_Bacs_DoesntHaveBacsFlag()
        {
            _request.PaymentScheme = PaymentScheme.Bacs;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.False(result);
        }
        [Test]
        public void IsPaymentValid_FasterPayments_HasFasterPaymentsFlag()
        {
            _request.PaymentScheme = PaymentScheme.FasterPayments;
            _account.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.True(result);
        }
        [Test]
        public void IsPaymentValid_FasterPayments_DoesntHaveFasterPaymentsFlag_DoesntHaveEnoughBalance()
        {
            _request.PaymentScheme = PaymentScheme.FasterPayments;
            _account.Balance = 1.00m;
            _request.Amount = 10.00m;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.False(result);
        }
        [Test]
        public void IsPaymentValid_FasterPayments_DoesntHaveFasterPaymentsFlag_HasEnoughBalance()
        {
            _request.PaymentScheme = PaymentScheme.FasterPayments;
            _account.Balance = 10.00m;
            _request.Amount = 1.00m;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.True(result);
        }
        [Test]
        public void IsPaymentValid_Chaps_HasChapsFlag()
        {
            _request.PaymentScheme = PaymentScheme.Chaps;
            _account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.True(result);
        }
        [Test]
        public void IsPaymentValid_Chaps_DoesntHaveChapsFlag_StatusIsLive()
        {
            _request.PaymentScheme = PaymentScheme.Chaps;
            _account.Status = AccountStatus.Live;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.True(result);
        }
        [Test]
        public void IsPaymentValid_Chaps_DoesntHaveChapsFlag_StatusIsntLive()
        {
            _request.PaymentScheme = PaymentScheme.Chaps;
            _account.Status = AccountStatus.Disabled;
            var result = _accountService.IsPaymentValid(_account, _request);
            Assert.False(result);
        }

        [Test]
        public void DeductAmountTest_BalanceGreaterThanAmount()
        {
            var balance = 2.00m;
            var amount = 1.00m;
            DeductTest(balance, amount);
            Assert.True(_account.Balance == 1.00m);
        }
        [Test]
        public void DeductAmountTest_BalanceLessThanAmount()
        {
            var balance = 0.50m;
            var amount = 1.00m;
            DeductTest(balance, amount);
            Assert.True(_account.Balance == -0.50m);
        }
        [Test]
        public void DeductAmountTest_BalanceEqualToThanAmount()
        {
            var balance = 1.00m;
            var amount = 1.00m;
            DeductTest(balance, amount);
            Assert.True(_account.Balance == 0);
        }

        private void DeductTest(decimal accountBalance, decimal deductionAmount)
        {
            _account.Balance = accountBalance;
            _request.Amount = deductionAmount;
            _accountService.DeductAmount(_account, _request);
        }
    }
}