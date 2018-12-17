using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WeChat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Login : Page
    {
        string code;
        string uuid;
        string redirect_uri;
        public Login()
        {
            this.InitializeComponent();
        }

        private void Get_QRCode()
        {
            Uri uri = new Uri("http://login.weixin.qq.com/qrcode/" + uuid);
            qrcode.Source = new BitmapImage(uri);
        }


        private async Task Get_redirect_uri()
        {
            String uri = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?uuid="+ uuid + "&tip=1&_=" + Time.Now();

            var response = await Get.Get_Response_Str(uri, "");
            string result = await response.Content.ReadAsStringAsync();

            code = result.Split(new char[] { '=', ';' })[1];
            if(code.Equals("200"))
            {
                redirect_uri = result.Split('"')[1];
            }
            
            Debug.WriteLine(redirect_uri);
        }

        private async Task Get_uuid()
        {
            String uri = "https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&fun=new&lang=zh_CN&_=" + Time.Now();
            StringBuilder s = new StringBuilder();
            var response = await Get.Get_Response_Str(uri, "");
            string result = await response.Content.ReadAsStringAsync();

            uuid = result.Split('"')[1];

            Debug.WriteLine(uuid);
        }

        private async void QRCode(object sender, RoutedEventArgs e)
        {
            await Get_uuid();
            Get_QRCode();
            await Get_redirect_uri();
            while (!code.Equals("200"))
            {
                await Task.Delay(2000);
                await Get_redirect_uri();
                switch(code)
                {
                    case "408":
                        break;
                    case "200":
                        
                        Frame.Navigate(typeof(MainPage), redirect_uri);
                        break;
                }
            }

        }
    }
}
