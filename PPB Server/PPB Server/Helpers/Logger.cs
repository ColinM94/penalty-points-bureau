using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PPB_Server.Helpers
{
    public class Logger
    {
        public void AccountLocked(string username)
        {
            string line = $"Username: {username} on {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.";

            try
            {
                using (StreamWriter file = new StreamWriter($"logs\\Locked Accounts {DateTime.Now.ToString("yyyyMMdd")}.txt", true))
                {
                    file.WriteLine(line);
                }

                // Emails admin details about infraction. 
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("ppbserver@gmail.com", "PPB System");
                mail.To.Add(new MailAddress("penaltypointsbureau@gmail.com", "Admin"));
                mail.Subject = "PPB Acount Locked";
                mail.Body = $"Penalty Points Bureau\n\nA user's account has been locked for repeated failed attempts to login.\n\n{line}\n\nThis is an automated message from the PPB server.";

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential("ppbserver@gmail.com", "ppbpass123");
                client.EnableSsl = true; // runtime encrypt the SMTP communications using SSL

                client.Send(mail);
            }
            catch(Exception ex)
            {
                if(Server.Debug)
                {
                    Console.WriteLine(ex);
                }
            }          
        }

        public void Idle(string username)
        {
            try
            {
                string line = $"Username: {username} 15 minute idle logout on {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.";
                using (StreamWriter file = new StreamWriter($"logs\\idlelog.txt", true))
                {
                    file.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                if (Server.Debug)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
