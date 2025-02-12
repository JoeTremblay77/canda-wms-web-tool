public static class ApplicationRoles
{
    public const string Admin = "Admin";
    public const string All = "Admin, Staff";
    public const string Staff = "Staff";
}

public static class SearchDropDownFilterValues
{
    public const string AllText = "All";
    public const string AllVal = "00000000-0000-0000-0000-000000000000";
}

public static class SortTypes
{
    public const string CustomerName = "CustomerName";
    public const string CustomerName_desc = "CustomerName_desc";

    public const string CustomerPO = "CustomerPO";
    public const string CustomerPO_desc = "CustomerPO_desc";

    public const string InvoicedDate = "InvoicedDate";
    public const string InvoicedDate_desc = "InvoicedDate_desc";

    public const string InvoiceNumber = "InvoiceNumber";
    public const string InvoiceNumber_desc = "InvoiceNumber_desc";

    public const string JobCompleteDate = "JobCompleteDate";
    public const string JobCompleteDate_desc = "JobCompleteDate_desc";
    public const string JobDescription = "JobDescription";
    public const string JobDescription_desc = "JobDescription_desc";
    public const string JobNumber = "JobNumber";
    public const string JobNumber_desc = "JobNumber_desc";
    public const string JobSiteName = "JobSiteName";
    public const string JobSiteName_desc = "JobSiteName_desc";

    public const string JobStartDate = "JobStartDate";
    public const string JobStartDate_desc = "JobStartDate_desc";
    public const string LeadTech = "LeadTech";
    public const string LeadTech_desc = "LeadTech_desc";
    public const string ProjectManagerName = "ProjectManagerName";
    public const string ProjectManagerName_desc = "ProjectManagerName_desc";
    public const string QuoteNumber = "QuoteNumber";
    public const string QuoteNumber_desc = "QuoteNumber_desc";
    public const string ReconciledDate = "ReconciledDate";
    public const string ReconciledDate_desc = "ReconciledDate_desc";
    public const string SignedOffDate = "SignedOffDate";
    public const string SignedOffDate_desc = "SignedOffDate_desc";
    public const string TicketNumber = "TicketNumber";
    public const string TicketNumber_desc = "TicketNumber_desc";
    public const string TotalInvoiced = "TotalInvoiced";
    public const string TotalInvoiced_desc = "TotalInvoiced_desc";
    public const string TotalMargin = "TotalMargin";
    public const string TotalMargin_desc = "TotalMargin_desc";
    public const string TotalPrice = "TotalPrice";
    public const string TotalPrice_desc = "TotalPrice_desc";
}

public static class UserTypes
{
    public const string User = "User";
}

public class Defaults
{
    //public const string AuditInfoDateTimeFormat = "yyyy ddd MMM dd h:mm tt";
    //public const string DateCreatedDateTimeFormat = "ddd MMM dd yyyy";

    internal const int UserTokenLength = 15;
    public const string ApplicationTimeZoneId = "Pacific Standard Time";
    public const string AuditDateFormat = "dd-MM-yyyy h:mm:ss tt";
    public const string AuditDateFormatGrid = "{0:dd-MM-yyyy h:mm:ss tt}";
    public const string DateFormat = "yyyy-MM-dd";
    public const string DatePickerToStringFormat = "ddd MMM d yyyy";
    public const string ErrorLogDateTimeFormatGrid = "{0:ddd MMM dd h:mm:ss tt yyyy}";
    public const int InvoiceResultsPageSize = 10000;
    public const int MinPasswordLength = 9;

    public const string MiscTypeDefaultString = "Default";
    public const string PercentageFormatGrid = "{0:P}";
    public const int ProjectListFromDaysBack = 183;
    public const int ProjectListToDaysAhead = 30;
    public const int ProjectResultsPageSize = 10000;
    public const string UserActivityTimeFormat = "h:mm:ss tt";
    public const string ValueChangedBackground = "#F4F2F0";
}

public class ErrorSeverity
{
    private ErrorSeverity()
    {
    }

    public const string Level1 = "Level1";

    public const string Level2 = "Level2";
}

public class ViewText
{
    private ViewText()
    {
    }

    public const string CannotDeleteDefaultRecord = "The record you are trying to delete is a default and cannot be removed.";
    public const string ChangesSaved = "Changes Saved";
    public const string ConfirmAccount = "Please confirm your account";
    public const string CurrentValue = "Current value: ";
    public const string RecordDeleted = "Unable to save changes. The record was deleted by another user while you were editing it.";
    public const string RecordNotFound = "That record was not found in the database. Most likely you are looking at an out of date list, or someone else just deleted it after your list was generated.";
    public const string ReleaseErrorMessage = "An error occurred while processing your request. Please contact your system adminstrator.";
    public const string TryAgainToAdd = "Unable to create this record. Try again, and if the problem persists, please contact your system administrator.";

    public const string TryAgainToDelete = "Unable to delete that record. Try again, and if the problem persists, please contact your system administrator.";

    public const string TryAgainToEdit = "Unable to edit that record. Try again, and if the problem persists, please contact your system administrator.";

    public const string TryAgainToSave = "Unable to save changes. Try again, and if the problem persists, please contact your system administrator.";

    public const string UnableToSave = "The record you attempted to edit "
                        + "was modified by another user after you got the original values. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.";

    public const string UnableToSaveDueToUpdateProcess = "Try again, the record was locked briefly by an update process.";
}