using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace xfactor.Models
{
    public class SendEmail
    {

        string email;
        string Subject;
        string body;
        
        public SendEmail(string groupe, string subject, string body)
        {
            email = groupe;
            Subject = subject;
            this.body = body;
        }
        private XFactor_PRODEntities1 db = new XFactor_PRODEntities1();

        public string SendEmailXpert()
        {
            try
            {
                T_CONFIGURATION_EMAIL configemail= db.T_CONFIGURATION_EMAIL.Find(1);

                string email_xfactor = configemail.EMAIL; //"AdminXFactor@poulinagroup.com";
                string password = configemail.MDP;//"azertyui";
                                                    // cleint.Timeout = 100000;
                                                    //   cleint.DeliveryMethod = SmtpDeliveryMethod.Network;
                MailMessage mail = new MailMessage();
                foreach (var item in db.T_EMAIL.Where(p => p.TITRE_GROUPE.Contains(email)))
                {
                    mail.To.Add(db.TS_USER.Find(item.ID_USER).MAIL_USER);
                }
            
                mail.Subject = Subject;
                mail.Body = body;
                mail.From=new MailAddress(email_xfactor);
                mail.IsBodyHtml = true;
          
                // SmtpClient cleint = new SmtpClient("mail23.lwspanel.com");
                 SmtpClient cleint = new SmtpClient();
                cleint.Host = configemail.SMTP; //"192.168.1.4";
                cleint.Port = (int)configemail.PORT; //25;
                cleint.UseDefaultCredentials = true;
                cleint.EnableSsl =(bool)configemail.EnableSsl;
                 cleint.Timeout = 100000;
                cleint.DeliveryMethod = SmtpDeliveryMethod.Network;
                cleint.Credentials = new System.Net.NetworkCredential(email_xfactor, password);
                cleint.Send(mail);

                return "true";
            }
          catch (Exception e)
            {

                return e.Message;
            }


       
        }
    }
}