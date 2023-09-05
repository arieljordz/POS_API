using POS_API.Interface;
using POS_API.Models;

namespace POS_API.Service
{
    public class GlobalService : IGlobalService
    {
        private readonly POSDBContext db;
        public GlobalService(POSDBContext _db)
        {
            db = _db;
        }
        public void SaveUserLogs(User user, bool isLogin)
        {
            if (isLogin)
            {
                UserLogs userLogs = new UserLogs();
                userLogs.UserId = user.Id;
                userLogs.TimeInDate = DateTime.Now;
                userLogs.TimeOutDate = null;
                db.UserLogs.Add(userLogs);
                db.SaveChanges();
            }
            else
            {
                var uLogs = db.UserLogs.Where(x => x.UserId == user.Id && x.TimeOutDate == null).FirstOrDefault();
                if (uLogs != null)
                {
                    uLogs.TimeOutDate = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }

        public void SaveTransactionLogs(TransactionLogs Tlogs)
        {
            db.TransactionLogs.Add(Tlogs);
            db.SaveChanges();
        }
    }
}
