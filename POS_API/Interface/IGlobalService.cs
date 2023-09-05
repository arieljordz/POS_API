using POS_API.Models;

namespace POS_API.Interface
{
    public interface IGlobalService
    {
        void SaveUserLogs(User user, bool isLogin);

        void SaveTransactionLogs(TransactionLogs Tlogs);
    }
}
