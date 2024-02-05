using Castle.DynamicProxy;
using Core.Entities.Concrete;
using Core.Utilities.Interception;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Utilities.IOC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Performance
{
    public class PerformanceAspect :MethodInterception
    {
        private int _internal;
        //aradaki süreyi tespit etemsi için
        private Stopwatch _stopwatch;

        public PerformanceAspect(int interval)
        {
            _internal = interval;
            _stopwatch = ServiceTool.ServiceProvider.GetService<Stopwatch>();
        }
        protected override void OnBefore(IInvocation invocation)
        {
            //tüm işlemlerden önce _stopwatch başlicak 
            _stopwatch.Start();
        }

       
        protected override void OnAfter(IInvocation invocation)
        {
            //işlemden sonra durducak
            if(_stopwatch.Elapsed.TotalSeconds > _internal)
            {
                //MAİL ATICAK
                string stringbody = $"Performance : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}";
                SendConfirmEmail(stringbody);

            }
            _stopwatch.Reset();
        }

        void SendConfirmEmail(string stringbody)
        {
            string subject = "Performans Maili";
            string body = "";
            SendMailDto sendMailDto = new SendMailDto()
            {
                Email = "cevikburak1@zohomail.eu",
                Password = "burakcevik1",
                Port = 587,
                SMTP = "smtp.zoho.eu",
                SSL = true,
                email = "cevikburak1@zohomail.eu",
                Subject = subject,
                Body = stringbody,

            };
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(sendMailDto.Email);
                mailMessage.To.Add(sendMailDto.Email);
                mailMessage.Subject = sendMailDto.Subject;
                mailMessage.Body = sendMailDto.Body;
                mailMessage.IsBodyHtml = true;
                //mailMessage.Attachments.Add();
                using (SmtpClient smtp = new SmtpClient(sendMailDto.SMTP))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(sendMailDto.Email, sendMailDto.Password);
                    smtp.EnableSsl = sendMailDto.SSL;
                    smtp.Port = sendMailDto.Port;
                    smtp.Send(mailMessage);
                };
            };
        }
    }
}
