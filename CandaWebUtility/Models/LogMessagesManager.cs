using CandaWebUtility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace CandaWebUtility
{
    public enum LogType
    {
        Login,
        Split,
        ExpiryDate
    }

    /// <summary>
    /// JT: Logs messages into the CandaWebUtility.dbo.Logging table. The other LogManager that exists is for the error table and was existant from before.
    /// </summary>
    public static class LogMessagesManager {

        private static readonly object _lockObj = new object();

        public static async Task Log(LogType logType, string user, string message = "", string fromValue = "", string toValue = "")
        {
            try
            {
                using (EFEntities db = new EFEntities())
                {
                    db.Logging.Add(new Logging
                    {
                        ROWID = Guid.NewGuid(),
                        DateTimeStamp = DateTime.Now,
                        Action = logType.ToString(),
                        User = user,
                        FromValue = fromValue,
                        ToValue = toValue,
                        Message = message
                    });
                    await db.SaveChangesAsync();
                    
                }
            }
            catch (Exception ex)
            {
                LogManager.LogUIException(user, ex.InnerException == null ? ex : ex.InnerException, "Severe");
            }
        }

    }
}
