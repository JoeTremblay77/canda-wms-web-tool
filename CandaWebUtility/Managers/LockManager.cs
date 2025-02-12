using System;
using System.Threading;

public class LockManager
{
    private static object TimeLock = new object();

    private LockManager()
    {
    }

    public static DateTime GetTimeStamp()
    {
        DateTime result = DateTime.Now;

        if (Monitor.TryEnter(TimeLock, new TimeSpan(0, 0, 1)))
        {
            try
            {
                Thread.Sleep(3);
                result = DateTime.Now;
            }
            finally
            {
                Monitor.Exit(TimeLock);
            }
        }

        return result;
    }
}