using Melior.InterviewQuestion.Types;

namespace Melior.InterviewQuestion.Data
{
    public interface IBackupAccountDataStore
    {
        public Account GetAccount(string accountNumber);

        public void UpdateAccount(Account account);
    }
}
