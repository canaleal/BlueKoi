using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Payment
{
    /// <summary>
    /// A class used to send an email with receipt dataset using MailKit
    /// </summary>
    public class EmailData
    {
        /// <summary>
        /// A method used to send the email with receipt information
        /// </summary>
        /// <param name="emailInfo">The information regarding the users email and payment</param>
        public void SendMail(string emailInfo)
        {

            var mailMessage = new MimeMessage();
            mailMessage.From.Add(MailboxAddress.Parse("vinVaultApp@gmail.com"));
            mailMessage.To.Add(MailboxAddress.Parse(emailInfo));
            mailMessage.Subject = "Art";

            var builder = new BodyBuilder();
            builder.TextBody = @"Hey there,
                This is the Blue Koi Team, thank you so much for using our application.
                You can find your image in the orders page.
                
                -- Alex from Blue KOI
                ";

            
            mailMessage.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 465, true);
            smtp.Authenticate("vinVaultApp@gmail.com", "vinVaultApp007");
            smtp.Send(mailMessage);
            smtp.Disconnect(true);

        }
    }
}
