﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace OurOpenSource.Net.Mail
{
    /// <summary>
    /// 易用的邮件操作类。
    /// A class for easy to operation email.
    /// </summary>
    public class EasyMail
    {
        // 参考原文：https://www.cnblogs.com/ZxtIsCnblogs/p/8301819.html {
        /// <summary>
        /// 发送邮件。
        /// Send email.
        /// </summary>
        /// <param name="accountAddress">
        /// 发送邮件用的邮箱账号。
        /// An email address for send email.
        /// </param>
        /// <param name="password">
        /// 发送邮件用的邮箱密码。
        /// The password of account for send email.
        /// </param>
        /// <param name="smtpServerAddress">
        /// SMTP邮箱服务器地址。
        /// SMTP server address.
        /// </param>
        /// <param name="port">
        /// SMTP邮箱服务器端口。
        /// SMTP server port.
        /// </param>
        /// <param name="receiverAddress">
        /// 收信人邮箱地址。
        /// Receiver email address.
        /// </param>
        /// <param name="subject">
        /// 标题（主题）。
        /// Subject.
        /// </param>
        /// <param name="content">
        /// 内容。
        /// Content.
        /// </param>
        public static void Send(string accountAddress, string password, string smtpServerAddress, int port, string receiverAddress, string subject, string content)
        {
            MailMessage message = new MailMessage()
            {
                // 设置发件人，发件人需要与设置的邮件发送服务器的邮箱一致。 Set email sender. It needs to be consistent with the mailbox of the set mail sending server.
                From = new MailAddress(accountAddress),
                // 设置邮件标题。 Set mail subject.
                Subject = subject,
                // 设置邮件内容。 Set mail content.
                Body = content
            };
            // 设置收件人，可添加多个，添加方法与下面的一样。 Set receiver. You can add more than one in the same way as below.
            message.To.Add(receiverAddress);
            // 设置抄送人。 Set CC
            //message.CC.Add(ccer);

            // 设置邮件发送服务器，服务器根据你使用的邮箱而不同，可以到相应的邮箱管理后台查看。 Set the mail sending server. The server varies according to the mailbox you use. You can view it in the corresponding mailbox management background.
            SmtpClient client = new SmtpClient(smtpServerAddress, port)
            {
                // 设置发送人的邮箱账号和密码。 Set the sender's email account and password.
                Credentials = new NetworkCredential(accountAddress, password),
                // 启用SSL。 Enable SSL.
                EnableSsl = true,
            };

            // 发送邮件。 Send email.
            client.Send(message);
        }
        // }
    }
}
