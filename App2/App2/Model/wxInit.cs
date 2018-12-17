using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.weixinInit
{
    // To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
    //
    //    using QuickType;
    //
    //    var weChat = WeChat.FromJson(jsonString);

    public partial class Init
    {
        [JsonProperty("BaseResponse")]
        public BaseResponse BaseResponse { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }

        [JsonProperty("ContactList")]
        public ContactList[] ContactList { get; set; }

        [JsonProperty("SyncKey")]
        public SyncKey SyncKey { get; set; }

        [JsonProperty("User")]
        public User User { get; set; }

        [JsonProperty("ChatSet")]
        public string ChatSet { get; set; }

        [JsonProperty("SKey")]
        public string SKey { get; set; }

        [JsonProperty("ClientVersion")]
        public long ClientVersion { get; set; }

        [JsonProperty("SystemTime")]
        public long SystemTime { get; set; }

        [JsonProperty("GrayScale")]
        public long GrayScale { get; set; }

        [JsonProperty("InviteStartCount")]
        public long InviteStartCount { get; set; }

        [JsonProperty("MPSubscribeMsgCount")]
        public long MpSubscribeMsgCount { get; set; }

        [JsonProperty("MPSubscribeMsgList")]
        public MpSubscribeMsgList[] MpSubscribeMsgList { get; set; }

        [JsonProperty("ClickReportInterval")]
        public long ClickReportInterval { get; set; }
    }

    public partial class BaseResponse
    {
        [JsonProperty("Ret")]
        public long Ret { get; set; }

        [JsonProperty("ErrMsg")]
        public string ErrMsg { get; set; }
    }

    public partial class ContactList
    {
        [JsonProperty("Uin")]
        public long Uin { get; set; }

        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("NickName")]
        public string NickName { get; set; }
        
        [JsonProperty("HeadImgUrl")]
        public string HeadImgUrl { get; set; }
    }

    public partial class MpSubscribeMsgList
    {
    }

    public partial class SyncKey
    {
        [JsonProperty("Count")]
        public long Count { get; set; }

        [JsonProperty("List")]
        public List[] List { get; set; }

        public string get_urlstring()
        {
            string urlstring = "";
            for (int i = 0; i < Count; i++)
            {
                if (i != 0) urlstring += "|";
                urlstring += List[i].Key + "_" + List[i].Val;
            }
            return urlstring;
        }
    }

    public partial class List
    {
        [JsonProperty("Key")]
        public long Key { get; set; }

        [JsonProperty("Val")]
        public long Val { get; set; }
    }

    public partial class User
    {
        [JsonProperty("Uin")]
        public long Uin { get; set; }

        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("NickName")]
        public string NickName { get; set; }

        [JsonProperty("HeadImgUrl")]
        public string HeadImgUrl { get; set; }

        [JsonProperty("RemarkName")]
        public string RemarkName { get; set; }

        [JsonProperty("PYInitial")]
        public string PyInitial { get; set; }

        [JsonProperty("PYQuanPin")]
        public string PyQuanPin { get; set; }

        [JsonProperty("RemarkPYInitial")]
        public string RemarkPyInitial { get; set; }

        [JsonProperty("RemarkPYQuanPin")]
        public string RemarkPyQuanPin { get; set; }

        [JsonProperty("HideInputBarFlag")]
        public long HideInputBarFlag { get; set; }

        [JsonProperty("StarFriend")]
        public long StarFriend { get; set; }

        [JsonProperty("Sex")]
        public long Sex { get; set; }

        [JsonProperty("Signature")]
        public string Signature { get; set; }

        [JsonProperty("AppAccountFlag")]
        public long AppAccountFlag { get; set; }

        [JsonProperty("VerifyFlag")]
        public long VerifyFlag { get; set; }

        [JsonProperty("ContactFlag")]
        public long ContactFlag { get; set; }

        [JsonProperty("WebWxPluginSwitch")]
        public long WebWxPluginSwitch { get; set; }

        [JsonProperty("HeadImgFlag")]
        public long HeadImgFlag { get; set; }

        [JsonProperty("SnsFlag")]
        public long SnsFlag { get; set; }
    }

    public partial class Init
    {
        public static Init FromJson(string json) => JsonConvert.DeserializeObject<Init>(json, WeChat.weixinInit.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Init self) => JsonConvert.SerializeObject(self, WeChat.weixinInit.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

