using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;


public class Notifier
{
    public void SendEmails(AdressBook book)
    {
        try
        {
            SmtpClient Smtp = new SmtpClient("smtp.gmail.com", 587);
            Smtp.UseDefaultCredentials = false;
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.EnableSsl = true;
            Smtp.Credentials = new NetworkCredential("grigorenkovlad1993@gmail.com", "13101993");
            var birthdayEmail = book.BirthdayUser().Select(x => x.Email);
            foreach(var item in birthdayEmail){
            MailMessage email = new MailMessage();
                email.From = new MailAddress("grigorenkovlad1993@gmail.com");
                email.Subject = "Happy Birhday";
                email.Body = "Best wishes";
                email.To.Add(item);
                Smtp.Send(email);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

}