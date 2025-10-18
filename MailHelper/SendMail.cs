using WebQuanLySanPhamKhoHang.Helper;
using System.Net;
using System.Net.Mail;

namespace WebQuanLySanPhamKhoHang.Helper
{
    public class SendMail
    {
        public static bool SendEmail(string to, string subject, string body, string attachFile)
        {
            try
            {
                MailMessage message = new MailMessage(ConstantHelper.emailSender, to, subject, body);
                using (var client = new SmtpClient(ConstantHelper.hostEmail, ConstantHelper.portEmail))
                {
                    client.EnableSsl = true;
                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        Attachment attachment = new Attachment(attachFile);
                        message.Attachments.Add(attachment);
                    }
                    NetworkCredential credential = new NetworkCredential(ConstantHelper.emailSender, ConstantHelper.passSender);
                    client.UseDefaultCredentials = false;
                    client.Credentials = credential;
                    client.Send(message);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}