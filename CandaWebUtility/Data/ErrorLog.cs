//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CandaWebUtility.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ErrorLog
    {
        public System.Guid ErrorLogId { get; set; } = Guid.NewGuid();
        public System.DateTime DateCreated { get; set; } = DateTime.Now;
        public string UserName { get; set; }
        public string Severity { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public bool Sent { get; set; }
    }
}
