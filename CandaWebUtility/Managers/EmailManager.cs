using Microsoft.AspNet.Identity;
using CandaWebUtility.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

internal class Email
{
    internal async Task Send(AccountEmailSMTP smtp)
    {
        using (SmtpClient client = new SmtpClient(smtp.Host, smtp.Port))
        {
            client.EnableSsl = smtp.UseSSL;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(smtp.UserName, SecurityManager.Show(smtp.Password));

            using (MailMessage message = new MailMessage())
            {
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                foreach (var x in To)
                {
                    message.To.Add(x);
                }
                foreach (var x in CC)
                {
                    message.CC.Add(x);
                }
                foreach (var x in BCC)
                {
                    message.Bcc.Add(x);
                }

                message.Subject = Subject;
                message.Body = Body;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.From = new MailAddress(From);
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);
            }
        }
    }

    public Email()
    {
        To = new List<string>();
        CC = new List<string>();
        BCC = new List<string>();
    }

    public List<string> BCC { get; set; }

    public string Body { get; set; }

    public List<string> CC { get; set; }

    public string From { get; set; }

    public string Subject { get; set; }

    public List<string> To { get; set; }
}

internal class EmailManager
{
    internal EmailManager()
    { }

    internal async Task SendAccountEmail(IdentityMessage message)
    {
        using (EFEntities db = new EFEntities())
        {
            AccountEmailSMTP smtp = db.AccountEmailSMTP.AsNoTracking().FirstOrDefault();
            if (smtp == null)
            {
                throw new System.Exception("AccountEmailSMTP Does Not Exist");
            }

            Email e = new Email
            {
                Subject = message.Subject,
                Body = message.Body,
                From = smtp.FromEmailAddress
            };
            e.To.Add(message.Destination);

            await e.Send(smtp);
        }
    }
}