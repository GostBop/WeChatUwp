using WeChat.weixinInit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeChat.weixinCookie
{
    class Cookie
    {
        public static Error Get_Cookie(string xml)
        {
            XmlSerializer xmlSearializer = new XmlSerializer(typeof(Error));
            byte[] ResultByte = Encoding.UTF8.GetBytes(xml);

            var ms = new MemoryStream(ResultByte);
            Error data = (Error)xmlSearializer.Deserialize(ms);

            return data;
        }
    }

    [Serializable, XmlRoot("error")]
    public class Error
    {
        public string ret { get; set; }
        public string message { get; set; }
        public string skey { get; set; }
        public string wxsid { get; set; }
        public string wxuin { get; set; }
        public string pass_ticket { get; set; }
        public string isgrayscale { get; set; }
        public string webwx_data_ticket { get; set; }
        public SyncKey syncKey { get; set; }
    }

    public class BaseRequest
    {
        public string Uin { get; set; }
        public string Sid { get; set; }
        public string Skey { get; set; }
        public string DeviceID { get; set; }

        public BaseRequest(string Uin, string Sid, string Skey)
        {
            this.Uin = Uin;
            this.Skey = Skey;
            this.Sid = Sid;
            this.DeviceID = "e123456789012345";
        }
    }

    public class SendMsg
    {
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
        public long ClientMsgId { get; set; }
        public long LocalID { get; set; }
    }
}
