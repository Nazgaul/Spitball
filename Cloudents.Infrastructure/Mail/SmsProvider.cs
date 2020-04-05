using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Mail
{
    /// <summary>
    /// https://www.hadavar.co.il/wp-content/uploads/2016/07/SMS_API-v3-1.pdf
    /// </summary>
    public sealed class SmsProvider : ISmsProvider
    {
        private readonly HttpClient _httpClient;

        public SmsProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> SendSmsAsync(string message, string phoneNumber, CancellationToken token)
        {
            var inforu = new Inforu(message, phoneNumber);
            var xml = Serialize(inforu);

            var content = new StringContent($"InforuXML={xml}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var result = await _httpClient.PostAsync("http://smsapi.hadavar.co.il/SendMessageXml.ashx", content, token);

            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStringAsync();

        }

        //public void Dispose()
        //{
        //    _httpClient?.Dispose();
        //}

        private static string Serialize<T>(T value)
        {
            if (value == null) return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(T), "");

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = false });
            xmlSerializer.Serialize(xmlWriter, value, ns);
            var text = stringWriter.ToString();
            return text.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "")
                .Replace("</xml>", "").Trim();
        }
    }


    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Inforu
    {
        public Inforu(string message, string phoneNumber)
        {
            Content = new InforuContent(message);
            Recipients = new InforuRecipients(phoneNumber);
            Settings = new InforuSettings();
            User = new InforuUser();
        }

        protected Inforu()
        {

        }


        /// <remarks/>
        public InforuUser User { get; set; }


        /// <remarks/>
        public InforuContent Content { get; set; }


        /// <remarks/>
        public InforuRecipients Recipients { get; set; }


        /// <remarks/>
        public InforuSettings Settings { get; set; }


    }

    /// <remarks/>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class InforuUser
    {
        public InforuUser()
        {
            Username = "jaron@spitball.co";
            Password = "Kcw9TfWC";
        }
        /// <remarks/>
        public string Username { get; set; }
        /// <remarks/>
        public string Password { get; set; }

    }

    /// <remarks/>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class InforuContent
    {
        public InforuContent(string message)
        {
            Message = message;
            Type = "sms";
        }
        protected InforuContent()
        {

        }

        /// <remarks/>
        public string Message { get; set; }


        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Type { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class InforuRecipients
    {

        public InforuRecipients(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
        protected InforuRecipients()
        {

        }

        /// <remarks/>
        public string PhoneNumber { get; set; }

    }

    /// <remarks/>
    [SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class InforuSettings
    {

        public InforuSettings()
        {
            Sender = "Spitball";
        }

        /// <remarks/>
        public string Sender { get; set; }
    }
}
