using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class bubbleleft : UserControl
    {
        public bubbleleft()
        {
            this.InitializeComponent();
            this.PointerEntered += BubbleLeftControl_PointerEntered;
            this.PointerExited += BubbleLeftControl_PointerExited;
        }

        private void BubbleLeftControl_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 158, 235, 107));
            rect1.Fill = brush;
            poly1.Fill = brush;
            line1.Stroke = brush;
        }

        private void BubbleLeftControl_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 158, 235, 107));
            rect1.Fill = brush;
            poly1.Fill = brush;
            line1.Stroke = brush;
        }
        
        /// <summary>
        /// 根据文字的实际高度和宽度来设置控件的大小
        /// </summary>
        private void SetSize()
        {
            int widthMargin = 36, heightMargin = 18, widthMax = 306, widthMin = 40, heightMin = 40;
            //计算文字的实际高度和宽度
            this.textBlock1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //Size sizeText = this.textBlock1.DesiredSize;

            this.Height = this.textBlock1.ActualHeight + heightMargin; //sizeText.Height + heightMargin;
            this.Width = this.textBlock1.ActualWidth + widthMargin;//sizeText.Width + widthMargin;

            if (this.Width > widthMax)  //最宽306，超过就换行
            {
                this.Width = widthMax;
                this.textBlock1.TextWrapping = TextWrapping.Wrap;
                this.textBlock1.Width = this.Width - widthMargin; //超长设定宽度是关键，不然高度计算出来的是单行的高度，不是实际多行的高度

                this.textBlock1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                //Size sizeText1 = this.textBlock1.DesiredSize;
                this.Height = this.textBlock1.ActualHeight + heightMargin;//sizeText1.Height + heightMargin;
                //this.textBlock1.TextTrimming = TextTrimming.WordEllipsis;
            }
            else if (this.Width < widthMin)
            {
                this.Width = widthMin;
            }

            if (this.Height < heightMin)
            {
                
                this.Height = heightMin;
            }
        }
        
        /// <summary>
        /// 文字内容
        /// </summary>
        public string Text
        {
            
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.textBlock1.Text = "";
                    return;
                }
                this.textBlock1.Text = value;

                SetSize();
            }
            get
            {
                return this.textBlock1.Text;
            }
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public new double FontSize
        {
            set
            {
                this.textBlock1.FontSize = value;

                SetSize();
            }
            get
            {
                return this.textBlock1.FontSize;
            }
        }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public new Brush Background
        {
            set
            {
                this.rect1.Fill = value;
                this.poly1.Fill = value;
            }
            get
            {
                return this.rect1.Fill;
            }
        }

        public new Brush Foreground
        {
            set
            {
                this.textBlock1.Foreground = value;
            }
            get
            {
                return this.textBlock1.Foreground;
            }
        }

        private void Image_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {

        }

        public void setImg(BitmapImage bit)
        {
            img.Source = bit;
        }
        public void setHyberLink(String Content, String url)
        {
            Hyberlink.Content = Content;
            Hyberlink.NavigateUri = new Uri(url);
            
            textBlock1.Visibility = Visibility.Collapsed;
            this.Width = Content.Length * 17;
            this.Height = 40;
        }

        private void SetSize1()
        {
            int widthMargin = 36, heightMargin = 18, widthMax = 306, widthMin = 40, heightMin = 40;
            //计算文字的实际高度和宽度
            //this.Hyberlink.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size sizeText = this.Hyberlink.DesiredSize;

            this.Height = sizeText.Height + heightMargin;
            this.Width = sizeText.Width + widthMargin;

            if (this.Width > widthMax)  //最宽306，超过就换行
            {
                this.Width = widthMax;
                //this.Hyberlink.c = TextWrapping.Wrap;
                this.Hyberlink.Width = this.Width - widthMargin; //超长设定宽度是关键，不然高度计算出来的是单行的高度，不是实际多行的高度

                //this.Hyberlink.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText1 = this.Hyberlink.DesiredSize;
                this.Height = sizeText1.Height + heightMargin;
               // this.Hyberlink.TextTrimming = TextTrimming.WordEllipsis;
            }
            else if (this.Width < widthMin)
            {
                this.Width = widthMin;
            }

            if (this.Height < heightMin)
            {

                this.Height = heightMin;
            }
        }
    }
}
