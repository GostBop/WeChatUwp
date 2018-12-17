using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class bubble : UserControl
    {
        public bubble()
        {
            this.InitializeComponent();
        }

        public string Text
        {
            set
            {
                left.Text = value;
            }
            get
            {
                return left.Text;
            }
        }

        public void setImg(BitmapImage bit)
        {
            img.Source = bit;
        }

        public void setImg_msg(BitmapImage bit)
        {
            img_meg.Visibility = Visibility.Visible;
            media.Visibility = Visibility.Collapsed;
            left.Visibility = Visibility.Collapsed;
            img_meg.Source = bit;
        }

        public void setHyberLink(String Content, String url)
        {
            left.setHyberLink(Content, url);
        }

        public void setNickName(String NickName_)
        {
            NickName.Text = NickName_;
        }

        public void setLength(int width)
        {
            left.Width = width;
        }

        public void setMedia(String uri)
        {
            img_meg.Visibility = Visibility.Collapsed;
            media.Visibility = Visibility.Visible;
            left.Visibility = Visibility.Collapsed;
            media.setMedia(uri);
        }

        public void setName(String name)
        {
            media.setName(name);
        }
    }
}
