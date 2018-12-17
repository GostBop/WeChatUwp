using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using System.Xml;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

namespace WeChat.Tile
{
    class TileManger
    {
        public static void Tile(String FromNickName, String ToNickName)
        {
                Windows.Data.Xml.Dom.XmlDocument xdoc = new Windows.Data.Xml.Dom.XmlDocument();
                xdoc.LoadXml(File.ReadAllText("tile.xml"));
                Windows.Data.Xml.Dom.XmlNodeList TileList = xdoc.GetElementsByTagName("text");
                TileList[0].InnerText = FromNickName;
                TileList[1].InnerText = "To";
                TileList[2].InnerText = ToNickName;
                TileList[3].InnerText = FromNickName;
                TileList[4].InnerText = "To";
                TileList[5].InnerText = ToNickName;
                TileList[6].InnerText = FromNickName;
                TileList[7].InnerText = "To";
                TileList[8].InnerText = ToNickName;
                TileList[9].InnerText = FromNickName;
                TileList[10].InnerText = "To";
            TileList[11].InnerText = ToNickName;

                TileNotification notification = new TileNotification(xdoc);
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.Update(notification);
        }
    }
}
