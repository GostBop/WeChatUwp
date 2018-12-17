using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;

namespace WeChat.weixinContact
{

    public partial class Contact
    {
        [JsonProperty("BaseResponse")]
        public BaseResponse BaseResponse { get; set; }

        [JsonProperty("MemberCount")]
        public long MemberCount { get; set; }

        [JsonProperty("MemberList")]
        public MemberList[] MemberList { get; set; }

        [JsonProperty("Seq")]
        public long Seq { get; set; }
        
    }

    public partial class BaseResponse
    {
        [JsonProperty("Ret")]
        public long Ret { get; set; }

        [JsonProperty("ErrMsg")]
        public string ErrMsg { get; set; }
    }

    public partial class MemberList
    {
        [JsonProperty("Uin")]
        public long Uin { get; set; }

        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("NickName")]
        public string NickName { get; set; }

        [JsonProperty("HeadImgUrl")]
        public string HeadImgUrl { get; set; }

        [JsonProperty("ContactFlag")]
        public long ContactFlag { get; set; }

        [JsonProperty("MemberCount")]
        public long MemberCount { get; set; }

        [JsonProperty("MemberList")]
        public object[] MemberListMemberList { get; set; }

        [JsonProperty("RemarkName")]
        public string RemarkName { get; set; }

        [JsonProperty("HideInputBarFlag")]
        public long HideInputBarFlag { get; set; }

        [JsonProperty("Sex")]
        public long Sex { get; set; }

        [JsonProperty("Signature")]
        public string Signature { get; set; }

        [JsonProperty("VerifyFlag")]
        public long VerifyFlag { get; set; }

        [JsonProperty("OwnerUin")]
        public long OwnerUin { get; set; }

        [JsonProperty("PYInitial")]
        public string PyInitial { get; set; }

        [JsonProperty("PYQuanPin")]
        public string PyQuanPin { get; set; }

        [JsonProperty("RemarkPYInitial")]
        public string RemarkPyInitial { get; set; }

        [JsonProperty("RemarkPYQuanPin")]
        public string RemarkPyQuanPin { get; set; }

        [JsonProperty("StarFriend")]
        public long StarFriend { get; set; }

        [JsonProperty("AppAccountFlag")]
        public long AppAccountFlag { get; set; }

        [JsonProperty("Statues")]
        public long Statues { get; set; }

        [JsonProperty("AttrStatus")]
        public long AttrStatus { get; set; }

        [JsonProperty("Province")]
        public string Province { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Alias")]
        public string Alias { get; set; }

        [JsonProperty("SnsFlag")]
        public long SnsFlag { get; set; }

        [JsonProperty("UniFriend")]
        public long UniFriend { get; set; }

        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        [JsonProperty("ChatRoomId")]
        public long ChatRoomId { get; set; }

        [JsonProperty("KeyWord")]
        public string KeyWord { get; set; }

        [JsonProperty("EncryChatRoomId")]
        public string EncryChatRoomId { get; set; }
    }


    public partial class Contact
    {
        public static Contact FromJson(string json) => JsonConvert.DeserializeObject<Contact>(json, WeChat.weixinContact.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Contact self) => JsonConvert.SerializeObject(self, WeChat.weixinContact.Converter.Settings);
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
