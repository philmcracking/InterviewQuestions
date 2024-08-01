using Melior.InterviewQuestion.Data;
using Melior.InterviewQuestion.Types;
using System.Configuration;

namespace Melior.InterviewQuestion.Services
{
    public class AccountService : IAccountService
    {
        private string DataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
        private IAccountDataStore _accountDataStore;
        private IBackupAccountDataStore _backupAccountDataStore;

        public AccountService(IAccountDataStore accountDataStore, IBackupAccountDataStore backupAccountDataStore)
        {
            _accountDataStore = accountDataStore;
            _backupAccountDataStore = backupAccountDataStore;
        }

        public AccountService()
        {
        }

        public Account GetAccount(MakePaymentRequest request)
        {
            if (DataStoreType == "Backup")
                return _accountDataStore.GetAccount(request.DebtorAccountNumber);
            else
                return _backupAccountDataStore.GetAccount(request.DebtorAccountNumber);
        }

        public void DeductAmount(Account account, MakePaymentRequest request)
        {
            account.Balance -= request.Amount;

            if (DataStoreType == "Backup")
                _accountDataStore.UpdateAccount(account);
            else
                _backupAccountDataStore.UpdateAccount(account);
        }

        public bool IsPaymentValid(Account account, MakePaymentRequest request)
        {
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                        return true;
                    break;

                case PaymentScheme.FasterPayments:
                    if (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                        return true;
                    else if (account.Balance > request.Amount)
                        return true;
                    break;

                case PaymentScheme.Chaps:
                    if (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                        return true;
                    else if (account.Status == AccountStatus.Live)
                        return true;
                    break;
            }

            return false;
        }
    }
}

