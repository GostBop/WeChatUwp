using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeChat.weixinMusic
{
    class Music
    {
        public static msg GetMusic(String result)
        {


            XmlSerializer xmlSearializer = new XmlSerializer(typeof(msg));
            byte[] ResultByte = Encoding.UTF8.GetBytes(result);

            var ms = new MemoryStream(ResultByte);
            msg data = (msg)xmlSearializer.Deserialize(ms);

            return data;
        }
    }

    public class Streamvideo
    {
        public string streamvideototaltime { get; set; }
    }

    public class CanvasPageItem
    {
    }

    public class Appattach
    {
        public string cdnthumburl { get; set; }
        public string cdnthumbmd5 { get; set; }
        public string cdnthumblength { get; set; }
        public string cdnthumbheight { get; set; }
        public string cdnthumbwidth { get; set; }
        public string cdnthumbaeskey { get; set; }
        public string aeskey { get; set; }
        public string encryver { get; set; }
        public string islargefilemsg { get; set; }
    }

    public class Emoticongift
    {
        public string packageflag { get; set; }
    }

    public class Emoticonshared
    {
        public string packageflag { get; set; }
    }

    public class Designershared
    {
        public string designeruin { get; set; }
        public string designername { get; set; }
        public string designerrediretcturl { get; set; }
    }

    public class Emotionpageshared
    {
        public string tid { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string iconUrl { get; set; }
        public string secondUrl { get; set; }
        public string pageType { get; set; }
    }

    public class Webviewshared
    {
    }

    public class Weappinfo
    {
        public string appservicetype { get; set; }
    }

    public class Websearch
    {
        public string rec_category { get; set; }
    }

    public class Appmsg
    {
        public string title { get; set; }
        public string des { get; set; }
        public string action { get; set; }
        public string type { get; set; }
        public string showtype { get; set; }
        public string url { get; set; }
        public string lowurl { get; set; }
        public string dataurl { get; set; }
        public string lowdataurl { get; set; }
        public string contentattr { get; set; }
        public Streamvideo streamvideo { get; set; }
        public CanvasPageItem canvasPageItem { get; set; }
        public Appattach appattach { get; set; }
        public string androidsource { get; set; }
        public Emoticongift emoticongift { get; set; }
        public Emoticonshared emoticonshared { get; set; }
        public Designershared designershared { get; set; }
        public Emotionpageshared emotionpageshared { get; set; }
        public Webviewshared webviewshared { get; set; }
        public string md5 { get; set; }
        public Weappinfo weappinfo { get; set; }
        public string statextstr { get; set; }
        public Websearch websearch { get; set; }
    }

    public class Appinfo
    {
        public string version { get; set; }
        public string appname { get; set; }
    }

    public class msg
    {
        public Appmsg appmsg { get; set; }
        public string scene { get; set; }
        public Appinfo appinfo { get; set; }
    }

}
