using CandaWebUtility.Data;
using System;
using System.Globalization;
using System.IO;
using System.Text;

public class LogManager
{
    private const string dirNameDateFormat = "yyyy.MM.dd";

    private const string fileNameDateFormat = "yyyy.MM.dd.HH.mm.ss.ffff";

    private const string fileNameExtension = ".txt";

    private LogManager()
    {
    }

    private static void writeToDb(string user, Exception ex, string severity)
    {
        using (EFEntities db = new EFEntities())
        {
            ErrorLog log = new ErrorLog();
            log.DateCreated = DateTime.UtcNow;
            log.ErrorLogId = Guid.NewGuid();
            log.ErrorMessage = ex.Message;

            if (ex.InnerException != null)
            {
                log.ErrorMessage += " " + ex.InnerException.Message;
            }
            log.Severity = severity;
            log.Sent = false;
            log.StackTrace = string.Empty;

            if (ex.StackTrace != null)
            {
                log.StackTrace = ex.StackTrace;
            }

            if (ex.InnerException != null)
            {
                if (ex.InnerException.StackTrace != null)
                {
                    log.StackTrace += " InnerException : " + ex.InnerException.StackTrace;
                }
            }

            log.UserName = user;

            db.ErrorLog.Add(log);
            db.SaveChanges();
        }
    }

    private static void writeToDisk(string user, Exception ex, string severity)
    {
        writeToDisk(user, ex.ToString(), severity);
    }

    private static void writeToDisk(string user, string ex, string severity)
    {
        string root = System.Web.Hosting.HostingEnvironment.MapPath("~/Log");
        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }

        string errorsPath = Path.Combine(root, "Errors");
        if (!Directory.Exists(errorsPath))
        {
            Directory.CreateDirectory(errorsPath);
        }

        DateTime now = DateTime.UtcNow;

        string dirPath = Path.Combine(errorsPath, now.ToString(LogManager.dirNameDateFormat, CultureInfo.InvariantCulture));
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        StringBuilder s = new StringBuilder();
        s.AppendLine(now.ToString(LogManager.fileNameDateFormat, CultureInfo.InvariantCulture));
        s.AppendLine("Severity: " + severity);
        s.AppendLine("User: " + user);
        s.AppendLine(ex.ToString());

        string filePath = Path.Combine(dirPath, now.ToString(LogManager.fileNameDateFormat, CultureInfo.InvariantCulture) + LogManager.fileNameExtension);

        File.AppendAllText(filePath, s.ToString());
    }

    internal static System.Web.Mvc.HandleErrorInfo HandleUIException(string user, System.Exception ex, string severity)
    {
        LogManager.LogUIException(user, ex, severity);
        System.Web.Mvc.HandleErrorInfo model = new System.Web.Mvc.HandleErrorInfo(ex, "Home", "Index");
        return model;
    }

    internal static void LogUIException(string user, System.Exception ex, string severity)
    {
        try
        {
            //  save to db
            LogManager.writeToDb(user, ex, severity);
        }
        catch (Exception x)
        {
            try
            {
                //  write the caught error to db,
                LogManager.writeToDisk(user, x, severity);
            }
            catch { } // do nothing because if writing to disk isn't working then....
        }
    }
}