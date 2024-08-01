using Melior.InterviewQuestion.Types;

namespace Melior.InterviewQuestion.Services
{
    public class PaymentService : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var accountService = new AccountService();
            Account account = accountService.GetAccount(request);

            if (account == null)
                return new MakePaymentResult() { Success = false };

            if(accountService.IsPaymentValid(account, request))
            {
                accountService.DeductAmount(account, request);
                return new MakePaymentResult() { Success = true };
            }

            return new MakePaymentResult() { Success = false };
        }
    }
}
