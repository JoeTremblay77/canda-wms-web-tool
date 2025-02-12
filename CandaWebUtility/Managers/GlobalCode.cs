using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CandaWebUtility.Data;

namespace CandaWebUtility
{
    internal class GlobalCode
    {
        internal static string ApplicationTimeZoneId(EFEntities db)
        {
            string result = Defaults.ApplicationTimeZoneId;
            var x = db.TimeZoneSetting.AsNoTracking().FirstOrDefault();
            if (x != null)
            {
                result = x.TimeZoneId;
            }
            return result;
        }

        internal static DateTime ConvertToApplicationTime(EFEntities db, DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(GlobalCode.ApplicationTimeZoneId(db)));
        }

        internal static string FixNullString(string s)
        {
            if (s == null)
            {
                return string.Empty;
            }

            return s;
        }



        internal static List<string> GetTimeZoneList()
        {
            List<string> result = new List<string>();
            ReadOnlyCollection<TimeZoneInfo> tzCollection = TimeZoneInfo.GetSystemTimeZones();
            foreach (var item in tzCollection)
            {
                result.Add(item.Id);
            }
            return result;
        }

        internal static string GetUserIdFromToken(string token, EFEntities db)
        {
            string id = string.Empty;

            var ui = db.UserInfo.AsNoTracking().Where(b => b.Token == token).FirstOrDefault();
            if (ui != null)
            {
                id = ui.UserId;
            }

            return id;
        }

        internal static string GetUserToken(string userID, EFEntities db)
        {
            string token = string.Empty;

            var ui = db.UserInfo.AsNoTracking().Where(b => b.UserId == userID).FirstOrDefault();
            if (ui != null)
            {
                token = ui.Token;
            }

            return token;
        }


    }
}

public class PublicGlobalCode
{
    public static string FixNullDateToDatePicker(DateTime? date)
    {
        string result = string.Empty;
        if (date != null)
        {
            result = date.Value.ToString(Defaults.DatePickerToStringFormat);
        }

        return result;
    }
}