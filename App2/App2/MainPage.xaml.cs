using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WeChat.weixinCookie;
using WeChat.weixinInit;
using WeChat.weixinContact;
using Newtonsoft.Json.Linq;
using WeChat.weixinMessage;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using WeChat.weixinMusic;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace WeChat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ContactView ContactView = new ContactView();
        ContactView AllContactView = new ContactView();
        ContactView AllPublicView = new ContactView();
        ContactView SearchView = new ContactView();
        string redirect_uri;
        Error cookie;
        String cookie_str;
        Init wxInit;
        Contact contact;

        public MainPage()
        {
            this.InitializeComponent();
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataRequested;
            //button.Click += Send_Message;
        }

        private async void PageLoad(object sender, RoutedEventArgs e)
        {
            bool flag = await Get_Cookie();
            if (flag)
            {
                return;
            }
            await Init();
            await Get_Contact();
            await Listen();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            redirect_uri = (string)e.Parameter;
            Debug.WriteLine(redirect_uri);
        }

        private async Task<bool> Get_Cookie()
        {
                String uri = redirect_uri + "&fun=new";

                HttpClientHandler myHandler = new HttpClientHandler();
                myHandler.AllowAutoRedirect = false;
                HttpClient myClient = new HttpClient(myHandler);
                var myRequest = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await myClient.SendAsync(myRequest);
                string result = await response.Content.ReadAsStringAsync();

                cookie = Cookie.Get_Cookie(result);
                String s = "+";
            if (cookie.wxsid.Contains(s))
            {
                Debug.WriteLine("哈哈哈");
                Debug.WriteLine(cookie.wxsid);
                Frame.Navigate(typeof(Login));
                return true;

            }
                string[] temp = myHandler.CookieContainer.GetCookieHeader(new Uri(uri)).Split(new char[] { ',', ';' });
                foreach (string c in temp)
                {
                    if (c.Contains("webwx_data_ticket"))
                    {
                        cookie.webwx_data_ticket = c.Split('=')[1];
                        break;
                    }
                }

                cookie_str = "webwx_data_ticket=" + cookie.webwx_data_ticket + "; wxsid=" + cookie.wxsid + "; wxuin=" + cookie.wxuin;

            return false;
            
        }

        private async Task Init()
        {
            BaseRequest baseRequest = new BaseRequest(cookie.wxuin, cookie.wxsid, cookie.skey);
            JObject jsonObj = new JObject();
            jsonObj.Add("BaseRequest", JObject.FromObject(baseRequest));
            String json = jsonObj.ToString().Replace("\r\n", "");

            String uri = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r=" + Time.Now() + "&lang=ch_ZN&pass_ticket=" + cookie.pass_ticket;

            string result = await Post.Get_Response_Str(uri, json);

            //Debug.WriteLine(result);

            wxInit = weixinInit.Init.FromJson(result);
            cookie.syncKey = wxInit.SyncKey;

            foreach (var member in wxInit.ContactList)
            {
                if (!member.UserName[1].Equals('@') && !member.NickName.Equals("文件传输助手"))
                {
                    FriendList friend = new FriendList();
                    friend.UserName = member.UserName;
                    friend.NickName = member.NickName;
                    friend.dialog = new List<bubble>();

                    uri = "http://wx2.qq.com" + member.HeadImgUrl;
                    
                    var response = await Get.Get_Response_Str(uri, cookie_str);
                    var r = await response.Content.ReadAsByteArrayAsync();
                    friend.bitmap = await ByteArrayToBitmapImage(r);

                    ContactView.AllItems.Add(friend);
                }
            }

        }

        private async Task Get_Contact()
        {
            String uri = "http://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetcontact" +
                "?&r=" + Time.Now();
            var response = await Get.Get_Response_Str(uri, cookie_str);
            string result = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(result);
            contact = Contact.FromJson(result);
            btnContact.Click += btnContact_Click;
            btnPublic.Click += btnPublic_Click;

        }

        private async Task Listen()
        {
            while (true)
            {
                await Task.Delay(500);
                bool flag = await HeartBeat();
                if (flag)
                {
                    break;
                }
            }
        }

        private async Task<bool> HeartBeat()
        {
            String uri = "https://webpush.wx2.qq.com/cgi-bin/mmwebwx-bin/synccheck" +
                "?skey=" + cookie.skey +
                "&r=" + Time.Now() +
                "&sid=" + cookie.wxsid +
                "&uin=" + cookie.wxuin +
                "&deviceid=" + "e123456789012345" +
                "&synckey=" + cookie.syncKey.get_urlstring() +
                "&_=" + Time.Now();
            
            var response = await Get.Get_Response_Str(uri, cookie_str);
            string result = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(result);

            string result_str = result.Split('=')[1];
            string selector = result_str.Split('"')[3];
            string retcode = result_str.Split('"')[1];
            //Debug.WriteLine(selector);
            if (retcode.Equals("1101"))
            {
                Frame.Navigate(typeof(Login));
                return true;
            }

            if (!selector.Equals("0"))
            {
                Get_Message();
            }
            return false;
        }

        private async void Get_Message()
        {
            String uri = "http://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsync" +
                "?pass_ticket=" + cookie.pass_ticket +
                "&r=" + Time.Now();

            BaseRequest baseRequest = new BaseRequest(cookie.wxuin, cookie.wxsid, cookie.skey);
            JObject jsonObj = new JObject();
            jsonObj.Add("BaseRequest", JObject.FromObject(baseRequest));
            jsonObj.Add("SyncKey", JObject.FromObject(cookie.syncKey));
            jsonObj.Add("rr", Time.Now());

            String json = jsonObj.ToString().Replace("\r\n", "");

            string result = await Post.Get_Response_Str(uri, json);
            Debug.WriteLine(result);

            var message = Message.FromJson(result);

            cookie.syncKey = message.SyncKey;

            foreach (var user in message.AddMsgList)
            {

                if (user.Content.Equals("") || (user.MsgType != 1 && user.MsgType != 3 && user.MsgType != 49) || (user.MsgType == 49 && user.AppMsgType != 5 && user.AppMsgType != 3))
                {
                    continue;
                   
                }
                if(user.FromUserName == wxInit.User.UserName)
                {
                    bool flag = false;
                    for (int i = 0; i < ContactView.AllItems.Count; i++)
                    {
                        if(user.ToUserName == ContactView.AllItems[i].UserName)
                        {
                            
                            BitmapImage bitmap = new BitmapImage();
                            bool isImg = false;
                            if (user.MsgType == 3)
                            {
                                String u = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID=" + user.MsgId + "&skey=" + cookie.skey + "&type=slave";
                                var res = await Get.Get_Response_Str(u, cookie_str);
                                byte[] b = await res.Content.ReadAsByteArrayAsync();
                                bitmap = await ByteArrayToBitmapImage(b);
                                isImg = true;
                            }
                            bubble msg = new bubble();
                            msg.Text = user.Content;
                            msg.setNickName(ContactView.AllItems[i].NickName);
                            if (isImg)
                            {
                                msg.setImg_msg(bitmap);
                            }
                            if (user.MsgType == 49 && user.AppMsgType == 5)
                            {
                                msg.setHyberLink(user.FileName, user.Url);
                            }
                            if (user.MsgType == 49 && user.AppMsgType == 3)
                            {
                                String str;
                                str = user.Content.Replace("&lt;", "<");
                                str = str.Replace("&gt;", ">");
                                str = str.Replace("<br/>", "\n");
                                Debug.WriteLine(str);
                                var music = Music.GetMusic(str);
                                Debug.WriteLine(music.appmsg.dataurl);
                                msg.setMedia(music.appmsg.dataurl);
                                msg.setName(music.appmsg.title);
                            }
                            uri = "http://wx2.qq.com" + wxInit.User.HeadImgUrl;
                            var response = await Get.Get_Response_Str(uri, cookie_str);
                            var r = await response.Content.ReadAsByteArrayAsync();
                            var bit = await ByteArrayToBitmapImage(r);
                            msg.setImg(bit);
                            msg.setNickName(wxInit.User.NickName);
                            Tile.TileManger.Tile(wxInit.User.NickName, ContactView.AllItems[i].NickName);
                            ContactView.AllItems[i].dialog.Add(msg);
                            if (i != 0)
                            {
                                var u = ContactView.AllItems[i];
                                ContactView.AllItems.RemoveAt(i);
                                ContactView.AllItems.Insert(0, u);
                            }
                            if (friend != null && friend.UserName == user.ToUserName)
                            {
                                sp1.Children.Add(msg);
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        foreach (var member in contact.MemberList)
                        {
                            if (user.ToUserName == member.UserName && !member.UserName[1].Equals('@'))
                            {
                                FriendList friend = new FriendList();
                                friend.UserName = user.ToUserName;
                                friend.NickName = member.NickName;
                                friend.dialog = new List<bubble>();

                                BitmapImage bitmap = new BitmapImage();
                                bool isImg = false;
                                if (user.MsgType == 3)
                                {
                                    String u = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID=" + user.MsgId + "&skey=" + cookie.skey + "&type=slave";
                                    var res = await Get.Get_Response_Str(u, cookie_str);
                                    byte[] b = await res.Content.ReadAsByteArrayAsync();
                                    bitmap = await ByteArrayToBitmapImage(b);
                                    isImg = true;
                                }

                                uri = "http://wx2.qq.com" + member.HeadImgUrl;
                                var response = await Get.Get_Response_Str(uri, cookie_str);
                                var result_byte = await response.Content.ReadAsByteArrayAsync();
                                friend.bitmap = await ByteArrayToBitmapImage(result_byte);

                                bubble msg = new bubble();
                                // msg.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                                //msg.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                                msg.Text = user.Content;

                                if (isImg)
                                {
                                    msg.setImg_msg(bitmap);
                                }
                                if (user.MsgType == 49 && user.AppMsgType == 5)
                                {
                                    msg.setHyberLink(user.FileName, user.Url);
                                }
                                if (user.MsgType == 49 && user.AppMsgType == 3)
                                {
                                    String str;
                                    str = user.Content.Replace("&lt;", "<");
                                    str = str.Replace("&gt;", ">");
                                    str = str.Replace("<br/>", "\n");
                                    Debug.WriteLine(str);
                                    var music = Music.GetMusic(str);
                                    Debug.WriteLine(music.appmsg.dataurl);
                                    msg.setMedia(music.appmsg.dataurl);
                                    msg.setName(music.appmsg.title);
                                }
                                uri = "http://wx2.qq.com" + wxInit.User.HeadImgUrl;
                                response = await Get.Get_Response_Str(uri, cookie_str);
                                var r = await response.Content.ReadAsByteArrayAsync();
                                var bit = await ByteArrayToBitmapImage(r);
                                msg.setImg(bit);
                                msg.setNickName(wxInit.User.NickName);
                                Tile.TileManger.Tile(wxInit.User.NickName, member.NickName);

                                friend.dialog.Add(msg);
                                ContactView.AllItems.Insert(0, friend);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < ContactView.AllItems.Count; i++)
                    {
                        if (user.FromUserName == ContactView.AllItems[i].UserName)
                        {
                            
                            Debug.WriteLine(user.MsgType);
                            BitmapImage bit = new BitmapImage();
                            bool isImg = false;
                            if (user.MsgType == 3)
                            {
                                String u = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID=" + user.MsgId + "&skey=" + cookie.skey + "&type=slave";
                                var response = await Get.Get_Response_Str(u, cookie_str);
                                byte[] r = await response.Content.ReadAsByteArrayAsync();
                                bit = await ByteArrayToBitmapImage(r);
                                isImg = true;
                            }
                            bubble msg = new bubble();
                            //msg.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                            //msg.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                            msg.Text = user.Content;
                            if (isImg)
                            {
                                msg.setImg_msg(bit);
                            }
                            if(user.MsgType == 49 && user.AppMsgType == 5)
                            {
                                msg.setHyberLink(user.FileName, user.Url);
                            }
                            if (user.MsgType == 49 && user.AppMsgType == 3)
                            {
                                String str;
                                str = user.Content.Replace("&lt;", "<");
                                str = str.Replace("&gt;", ">");
                                str = str.Replace("<br/>", "\n");
                                Debug.WriteLine(str);
                                var music = Music.GetMusic(str);
                                Debug.WriteLine(music.appmsg.dataurl);
                                msg.setMedia(music.appmsg.dataurl);
                                msg.setName(music.appmsg.title);
                            }
                            msg.setImg(ContactView.AllItems[i].bitmap);
                            msg.setNickName(ContactView.AllItems[i].NickName);

                            Tile.TileManger.Tile(ContactView.AllItems[i].NickName, wxInit.User.NickName);

                            ContactView.AllItems[i].dialog.Add(msg);

                            if (i != 0)
                            {
                                var u = ContactView.AllItems[i];
                                ContactView.AllItems.RemoveAt(i);
                                ContactView.AllItems.Insert(0, u);
                            }
                            if (friend != null && friend.UserName == user.FromUserName)
                            {
                                
                                sp1.Children.Add(msg);
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {

                        foreach (var member in contact.MemberList)
                        {
                            if (user.FromUserName == member.UserName && !member.UserName[1].Equals('@'))
                            {
                                FriendList friend = new FriendList();
                                friend.UserName = user.FromUserName;
                                friend.NickName = member.NickName;
                                friend.dialog = new List<bubble>();

                                BitmapImage bitmap = new BitmapImage();
                                bool isImg = false;
                                if (user.MsgType == 3)
                                {
                                    String u = "https://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxgetmsgimg?&MsgID=" + user.MsgId + "&skey=" + cookie.skey + "&type=slave";
                                    var res = await Get.Get_Response_Str(u, cookie_str);
                                    byte[] b = await res.Content.ReadAsByteArrayAsync();
                                    bitmap = await ByteArrayToBitmapImage(b);
                                    isImg = true;
                                }

                                uri = "http://wx2.qq.com" + member.HeadImgUrl;
                                var response = await Get.Get_Response_Str(uri, cookie_str);
                                var result_byte = await response.Content.ReadAsByteArrayAsync();
                                friend.bitmap = await ByteArrayToBitmapImage(result_byte);

                                bubble msg = new bubble();
                                // msg.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                                //msg.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                                msg.Text = user.Content;
                                if (isImg)
                                {
                                    msg.setImg_msg(bitmap);
                                }
                                if (user.MsgType == 49 && user.AppMsgType == 5)
                                {
                                    msg.setHyberLink(user.FileName, user.Url);
                                }
                                if (user.MsgType == 49 && user.AppMsgType == 3)
                                {
                                    String str;
                                    str = user.Content.Replace("&lt;", "<");
                                    str = str.Replace("&gt;", ">");
                                    str = str.Replace("<br/>", "\n");
                                    Debug.WriteLine(str);
                                    var music = Music.GetMusic(str);
                                    Debug.WriteLine(music.appmsg.dataurl);
                                    msg.setMedia(music.appmsg.dataurl);
                                    msg.setName(music.appmsg.title);
                                }
                                uri = "http://wx2.qq.com" + member.HeadImgUrl;
                                response = await Get.Get_Response_Str(uri, cookie_str);
                                var r = await response.Content.ReadAsByteArrayAsync();
                                var bit = await ByteArrayToBitmapImage(r);
                                msg.setImg(bit);
                                msg.setNickName(member.NickName);

                                Tile.TileManger.Tile(member.NickName, wxInit.User.NickName);

                                friend.dialog.Add(msg);
                                ContactView.AllItems.Insert(0, friend);
                                break;
                            }
                        }
                    }
                }
            }

            Debug.WriteLine("读取消息");
            Debug.WriteLine("BaseResponse.Ret:" + message.BaseResponse.Ret);
            Debug.WriteLine("AddMsgCount:" + message.AddMsgCount);
            foreach (var a in message.AddMsgList)
            {
                Debug.WriteLine(a.Content);
            }

            Debug.WriteLine("ModContactCount:" + message.ModContactCount);
            Debug.WriteLine("DelContactCount:" + message.DelContactCount);
            Debug.WriteLine("ModChatRoomMemberCount:" + message.ModChatRoomMemberCount);
        }

        public static async Task<BitmapImage> ByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return null;
            }

            BitmapImage bmp = new BitmapImage();
            try
            {
                MemoryStream mStream = new MemoryStream(byteArray);
                await bmp.SetSourceAsync(mStream.AsRandomAccessStream());
                return bmp;
            }
            catch (Exception ex)
            {
                bmp = null;
            }

            return bmp;
        }
        
        FriendList friend;
        private void Get_Dialog(object sender, ItemClickEventArgs e)
        {
            
            sp1.Children.Clear();
            friend = (FriendList)e.ClickedItem;
            nickname.Text = friend.NickName;
            foreach (var d in friend.dialog)
            {
                sp1.Children.Add(d);
            }
        }
        
        private async void Send_Message(object sender, RoutedEventArgs e)
        {
            if(friend != null)
            {
                String uri = "http://wx2.qq.com/cgi-bin/mmwebwx-bin/webwxsendmsg" +
                    "?sid=" + cookie.wxsid +
                    "&skey=" + cookie.skey +
                    "&pass_ticket=" + cookie.pass_ticket +
                    "&r=" + Time.Now();

                BaseRequest baseRequest = new BaseRequest(cookie.wxuin, cookie.wxsid, cookie.skey);
                JObject jsonObj = new JObject();
                jsonObj.Add("BaseRequest", JObject.FromObject(baseRequest));

                SendMsg msg = new SendMsg();
                msg.FromUserName = wxInit.User.UserName;
                msg.ToUserName = friend.UserName;
                msg.Type = 1;
                msg.Content = send.Text;
                msg.ClientMsgId = Time.Now();
                msg.LocalID = Time.Now();
                jsonObj.Add("Msg", JObject.FromObject(msg));


                String json = jsonObj.ToString().Replace("\r\n", "");

                string result = await Post.Get_Response_Str(uri, json);
                
                for (int i = 0; i < ContactView.AllItems.Count; i++)
                {
                    if (friend.UserName == ContactView.AllItems[i].UserName)
                    {
                        bubble m = new bubble();
                        m.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                        m.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                        m.Text = send.Text;

                        uri = "http://wx2.qq.com" + wxInit.User.HeadImgUrl;
                        var response = await Get.Get_Response_Str(uri, cookie_str);
                        var r = await response.Content.ReadAsByteArrayAsync();
                        var bit = await ByteArrayToBitmapImage(r);
                        m.setImg(bit);
                        m.setNickName(wxInit.User.NickName);

                        Tile.TileManger.Tile(wxInit.User.NickName, ContactView.AllItems[i].NickName);

                        ContactView.AllItems[i].dialog.Add(m);
                        if(i != 0)
                        {
                            var u = ContactView.AllItems[i];
                            ContactView.AllItems.RemoveAt(i);
                            ContactView.AllItems.Insert(0, u);
                        }
                        sp1.Children.Add(m);
                        send.Text = "";
                    }
                }
            }
            
        }

        bool Contact_Flag = true;
        private async void btnContact_Click(object sender, RoutedEventArgs e)
        {
            MyList.Visibility = Visibility.Collapsed;
            MyList2.Visibility = Visibility.Visible;
            MyList3.Visibility = Visibility.Collapsed;
            MyList4.Visibility = Visibility.Collapsed;
            right.Visibility = Visibility.Collapsed;
            if (Contact_Flag)
            {
                Contact_Flag = false;
                foreach (var member in contact.MemberList)
                {
                    if (!member.UserName[1].Equals('@') && !member.NickName.Equals("文件传输助手") && (member.VerifyFlag & 8) == 0)
                    {
                        FriendList friend = new FriendList();
                        friend.UserName = member.UserName;
                        friend.NickName = member.RemarkName;
                        if (member.RemarkName.Equals(""))
                        {
                            friend.NickName = member.NickName;
                        }

                        friend.dialog = new List<bubble>();

                        String uri = "http://wx2.qq.com" + member.HeadImgUrl;

                        var res = await Get.Get_Response_Str(uri, cookie_str);
                        var r = await res.Content.ReadAsByteArrayAsync();
                        friend.bitmap = await ByteArrayToBitmapImage(r);

                        AllContactView.AllItems.Add(friend);
                    }
                }
                
            }
        }
        bool Public_Flag = true;
        private async void btnPublic_Click(object sender, RoutedEventArgs e)
        {
            MyList.Visibility = Visibility.Collapsed;
            MyList2.Visibility = Visibility.Collapsed;
            MyList3.Visibility = Visibility.Visible;
            MyList4.Visibility = Visibility.Collapsed;
            right.Visibility = Visibility.Collapsed;
            if (Public_Flag)
            {
                Public_Flag = false;
                foreach (var member in contact.MemberList)
                {
                    if (!member.UserName[1].Equals('@') && !member.NickName.Equals("文件传输助手") && (member.VerifyFlag & 8) != 0)
                    {
                        FriendList friend = new FriendList();
                        friend.UserName = member.UserName;
                        friend.NickName = member.RemarkName;
                        if (member.RemarkName.Equals(""))
                        {
                            friend.NickName = member.NickName;
                        }

                        friend.dialog = new List<bubble>();

                        String uri = "http://wx2.qq.com" + member.HeadImgUrl;

                        var res = await Get.Get_Response_Str(uri, cookie_str);
                        var r = await res.Content.ReadAsByteArrayAsync();
                        friend.bitmap = await ByteArrayToBitmapImage(r);

                        AllPublicView.AllItems.Add(friend);
                    }
                }

            }
        }

        private void btnChat_Click(object sender, RoutedEventArgs e)
        {
            MyList.Visibility = Visibility.Visible;
            MyList2.Visibility = Visibility.Collapsed;
            MyList3.Visibility = Visibility.Collapsed;
            MyList4.Visibility = Visibility.Collapsed;
            right.Visibility = Visibility.Visible;

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            MyList.Visibility = Visibility.Collapsed;
            MyList2.Visibility = Visibility.Collapsed;
            MyList3.Visibility = Visibility.Collapsed;
            MyList4.Visibility = Visibility.Visible;
            right.Visibility = Visibility.Collapsed;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            sp1.Children.Clear();
            nickname.Text = "";
            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            if (item != null)
            {
                ContactView.AllItems.Remove(item);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList2.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            if(item != null)
            {
                foreach(var member in ContactView.AllItems)
                {
                    if(member.UserName == item.UserName)
                    {
                        return;
                    }
                }
                ContactView.AllItems.Insert(0, item);
            }
        }

        private string sharetitle = "", sharedescription = "";
        private StorageFile shareimg;
        private async void Share_click(object sender, RoutedEventArgs e)
        {

            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            sharetitle = item.NickName;
            sharedescription = "这是我的好朋友";

            shareimg = await Package.Current.InstalledLocation.GetFileAsync("Assets\\weixin256.png");
            DataTransferManager.ShowShareUI();
        }

        private async void Share_click1(object sender, RoutedEventArgs e)
        {

            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList2.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            sharetitle = item.NickName;
            sharedescription = "这是我的好朋友";

            shareimg = await Package.Current.InstalledLocation.GetFileAsync("Assets\\weixin256.png");
            DataTransferManager.ShowShareUI();
        }

        private async void Share_click2(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList3.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            sharetitle = item.NickName;
            sharedescription = "这是我的好朋友";

            shareimg = await Package.Current.InstalledLocation.GetFileAsync("Assets\\weixin256.png");
            DataTransferManager.ShowShareUI();
        }

        private async void Search_Click(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SearchView.AllItems.Clear();
            var name = args.QueryText.Trim();
            if (name == "")
                return;
            foreach (var member in contact.MemberList)
            {
                if (member.RemarkName.Contains(name) || (member.RemarkName.Equals("") && member.NickName.Contains(name)) )
                {
                    FriendList friend = new FriendList();
                    friend.UserName = member.UserName;
                    friend.NickName = member.RemarkName;
                    if (member.RemarkName.Equals(""))
                    {
                        friend.NickName = member.NickName;
                    }

                    friend.dialog = new List<bubble>();

                    String uri = "http://wx2.qq.com" + member.HeadImgUrl;

                    var res = await Get.Get_Response_Str(uri, cookie_str);
                    var r = await res.Content.ReadAsByteArrayAsync();
                    friend.bitmap = await ByteArrayToBitmapImage(r);

                    SearchView.AllItems.Add(friend);
                }
            }
        }

        private async void Share_click4(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList4_.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            sharetitle = item.NickName;
            sharedescription = "这是我的好朋友";

            shareimg = await Package.Current.InstalledLocation.GetFileAsync("Assets\\weixin256.png");
            DataTransferManager.ShowShareUI();
        }

        private void Add_Click3(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList4_.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            if (item != null)
            {
                foreach (var member in ContactView.AllItems)
                {
                    if (member.UserName == item.UserName)
                    {
                        return;
                    }
                }
                ContactView.AllItems.Insert(0, item);
            }
        }

        private void Add_Click2(object sender, RoutedEventArgs e)
        {
            var dc = (sender as FrameworkElement).DataContext;
            var item = (MyList3.ContainerFromItem(dc) as ListViewItem).Content as FriendList;
            if (item != null)
            {
                foreach (var member in ContactView.AllItems)
                {
                    if (member.UserName == item.UserName)
                    {
                        return;
                    }
                }
                ContactView.AllItems.Insert(0, item);
            }
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            Debug.WriteLine(e.Request.ToString());
            DataRequest request = e.Request;
            DataPackage requestData = request.Data;
            requestData.Properties.Title = sharetitle;
            requestData.SetText(sharedescription);

            // Because we are making async calls in the DataRequested event handler,
            //  we need to get the deferral first.
            DataRequestDeferral deferral = request.GetDeferral();

            // Make sure we always call Complete on the deferral.
            try
            {
                requestData.SetBitmap(RandomAccessStreamReference.CreateFromFile(shareimg));
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}

