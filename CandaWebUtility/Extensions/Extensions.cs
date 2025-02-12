using System;
using System.Collections.Generic;

namespace CandaWebUtility.Data
{
    public partial class AccountEmailSMTP
    {
        internal void SaveChanges(string modifyUserName, AccountEmailSMTP model)
        {
            this.DateModified = DateTime.UtcNow;
            this.FromEmailAddress = model.FromEmailAddress.Trim().ToLower();
            this.Host = model.Host;
            this.Password = SecurityManager.Hide(model.Password);
            this.Port = model.Port;
            this.UserModified = modifyUserName;
            this.UserName = model.UserName;
            this.UseSSL = model.UseSSL;
        }

        internal void Show()
        {
            this.Password = SecurityManager.Show(this.Password);
        }
    }

    public partial class TimeZoneSetting
    {
        public List<string> TimeZones { get; set; }
    }
}