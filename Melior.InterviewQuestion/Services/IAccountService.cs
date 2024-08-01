using Melior.InterviewQuestion.Types;

namespace Melior.InterviewQuestion.Services
{
    public interface IAccountService
    {
        Account GetAccount(MakePaymentRequest request);
        void DeductAmount(Account account, MakePaymentRequest request);
        bool IsPaymentValid(Account account, MakePaymentRequest request);
    }
}
