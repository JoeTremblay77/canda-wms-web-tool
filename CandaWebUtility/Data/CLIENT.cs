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
    
    public partial class CLIENT
    {
        public System.Guid ROWID { get; set; }
        public string CLIENTNAME { get; set; }
        public string TENANTID { get; set; }
        public string DN_PATH_PH { get; set; }
        public string DN_FILE_PH { get; set; }
        public string DN_EXTN_PH { get; set; }
        public string MINCHARGE { get; set; }
        public Nullable<int> RENEWALMETHOD { get; set; }
        public Nullable<int> INVOICETYPE { get; set; }
        public Nullable<System.DateTime> START_BUSINESS_DATE { get; set; }
        public Nullable<System.DateTime> END_BUSINESS_DATE { get; set; }
        public string UPDATE_FLAG { get; set; }
        public string BILL_TO_CLIENT { get; set; }
        public string BACK_ORDER { get; set; }
        public string EXTERNAL_ACCESS { get; set; }
    }
}
