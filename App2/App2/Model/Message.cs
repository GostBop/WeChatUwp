using WeChat.weixinInit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.weixinMessage
{
    public partial class Message
    {
        [JsonProperty("BaseResponse")]
        public BaseResponse BaseResponse { get; set; }

        [JsonProperty("AddMsgCount")]
        public long AddMsgCount { get; set; }

        [JsonProperty("AddMsgList")]
        public AddMsgList[] AddMsgList { get; set; }

        [JsonProperty("ModContactCount")]
        public long ModContactCount { get; set; }

        [JsonProperty("ModContactList")]
        public object[] ModContactList { get; set; }

        [JsonProperty("DelContactCount")]
        public long DelContactCount { get; set; }

        [JsonProperty("DelContactList")]
        public object[] DelContactList { get; set; }

        [JsonProperty("ModChatRoomMemberCount")]
        public long ModChatRoomMemberCount { get; set; }

        [JsonProperty("ModChatRoomMemberList")]
        public object[] ModChatRoomMemberList { get; set; }

        [JsonProperty("Profile")]
        public Profile Profile { get; set; }

        [JsonProperty("ContinueFlag")]
        public long ContinueFlag { get; set; }

        [JsonProperty("SyncKey")]
        public SyncKey SyncKey { get; set; }

        [JsonProperty("SKey")]
        public string SKey { get; set; }

        [JsonProperty("SyncCheckKey")]
        public SyncKey SyncCheckKey { get; set; }
    }

    public partial class AddMsgList
    {
        [JsonProperty("MsgId")]
        public string MsgId { get; set; }

        [JsonProperty("FromUserName")]
        public string FromUserName { get; set; }

        [JsonProperty("ToUserName")]
        public string ToUserName { get; set; }

        [JsonProperty("MsgType")]
        public long MsgType { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }

        [JsonProperty("Status")]
        public long Status { get; set; }

        [JsonProperty("ImgStatus")]
        public long ImgStatus { get; set; }

        [JsonProperty("CreateTime")]
        public long CreateTime { get; set; }

        [JsonProperty("VoiceLength")]
        public long VoiceLength { get; set; }

        [JsonProperty("PlayLength")]
        public long PlayLength { get; set; }

        [JsonProperty("FileName")]
        public string FileName { get; set; }

        [JsonProperty("FileSize")]
        public string FileSize { get; set; }

        [JsonProperty("MediaId")]
        public string MediaId { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("AppMsgType")]
        public long AppMsgType { get; set; }

        [JsonProperty("StatusNotifyCode")]
        public long StatusNotifyCode { get; set; }

        [JsonProperty("StatusNotifyUserName")]
        public string StatusNotifyUserName { get; set; }

        [JsonProperty("RecommendInfo")]
        public RecommendInfo RecommendInfo { get; set; }

        [JsonProperty("ForwardFlag")]
        public long ForwardFlag { get; set; }

        [JsonProperty("AppInfo")]
        public AppInfo AppInfo { get; set; }

        [JsonProperty("HasProductId")]
        public long HasProductId { get; set; }

        [JsonProperty("Ticket")]
        public string Ticket { get; set; }

        [JsonProperty("ImgHeight")]
        public long ImgHeight { get; set; }

        [JsonProperty("ImgWidth")]
        public long ImgWidth { get; set; }

        [JsonProperty("SubMsgType")]
        public long SubMsgType { get; set; }

        [JsonProperty("NewMsgId")]
        public long NewMsgId { get; set; }

        [JsonProperty("OriContent")]
        public string OriContent { get; set; }

        [JsonProperty("EncryFileName")]
        public string EncryFileName { get; set; }
    }

    public partial class AppInfo
    {
        [JsonProperty("AppID")]
        public string AppId { get; set; }

        [JsonProperty("Type")]
        public long Type { get; set; }
    }

    public partial class RecommendInfo
    {
        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("NickName")]
        public string NickName { get; set; }

        [JsonProperty("QQNum")]
        public long QqNum { get; set; }

        [JsonProperty("Province")]
        public string Province { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Content")]
        public string Content { get; set; }

        [JsonProperty("Signature")]
        public string Signature { get; set; }

        [JsonProperty("Alias")]
        public string Alias { get; set; }

        [JsonProperty("Scene")]
        public long Scene { get; set; }

        [JsonProperty("VerifyFlag")]
        public long VerifyFlag { get; set; }

        [JsonProperty("AttrStatus")]
        public long AttrStatus { get; set; }

        [JsonProperty("Sex")]
        public long Sex { get; set; }

        [JsonProperty("Ticket")]
        public string Ticket { get; set; }

        [JsonProperty("OpCode")]
        public long OpCode { get; set; }
    }

    public partial class BaseResponse
    {
        [JsonProperty("Ret")]
        public long Ret { get; set; }

        [JsonProperty("ErrMsg")]
        public string ErrMsg { get; set; }
    }

    public partial class Profile
    {
        [JsonProperty("BitFlag")]
        public long BitFlag { get; set; }

        [JsonProperty("UserName")]
        public BindEmail UserName { get; set; }

        [JsonProperty("NickName")]
        public BindEmail NickName { get; set; }

        [JsonProperty("BindUin")]
        public long BindUin { get; set; }

        [JsonProperty("BindEmail")]
        public BindEmail BindEmail { get; set; }

        [JsonProperty("BindMobile")]
        public BindEmail BindMobile { get; set; }

        [JsonProperty("Status")]
        public long Status { get; set; }

        [JsonProperty("Sex")]
        public long Sex { get; set; }

        [JsonProperty("PersonalCard")]
        public long PersonalCard { get; set; }

        [JsonProperty("Alias")]
        public string Alias { get; set; }

        [JsonProperty("HeadImgUpdateFlag")]
        public long HeadImgUpdateFlag { get; set; }

        [JsonProperty("HeadImgUrl")]
        public string HeadImgUrl { get; set; }

        [JsonProperty("Signature")]
        public string Signature { get; set; }
    }

    public partial class BindEmail
    {
        [JsonProperty("Buff")]
        public string Buff { get; set; }
    }

    public partial class Message
    {
        public static Message FromJson(string json) => JsonConvert.DeserializeObject<Message>(json, weixinMessage.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Message self) => JsonConvert.SerializeObject(self, weixinMessage.Converter.Settings);
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
