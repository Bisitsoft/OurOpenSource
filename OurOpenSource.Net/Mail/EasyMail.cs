using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace OurOpenSource.Net.Mail
{
    public class EasyMail
    {
        //https://www.cnblogs.com/ZxtIsCnblogs/p/8301819.html
        public static void Send(string accountAddress, string password, string smtpServerAddress, int port, string reciverAddress, string subject, string content)
        {
            MailMessage message = new MailMessage();
            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            message.From = new MailAddress(accountAddress);
            //设置收件人,可添加多个,添加方法与下面的一样
            message.To.Add(reciverAddress);
            //设置抄送人
            //message.CC.Add(ccer);
            //设置邮件标题
            message.Subject = subject;
            //设置邮件内容
            message.Body = content;
            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
            SmtpClient client = new SmtpClient(smtpServerAddress, port);
            //设置发送人的邮箱账号和密码
            client.Credentials = new NetworkCredential(accountAddress, password);
            //启用ssl,也就是安全发送
            client.EnableSsl = true;
            //发送邮件
            client.Send(message);
        }
    }
}
