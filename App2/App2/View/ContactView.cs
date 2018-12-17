using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WeChat
{
    public class ContactView
    {
        private ObservableCollection<FriendList> allItems = new ObservableCollection<FriendList>();
        public ObservableCollection<FriendList> AllItems { get { return this.allItems; } }
    }

    public partial class FriendList
    {
        public string UserName { get; set; }

        public string RemarkName { get; set; }

        public string NickName { get; set; }

        public List<bubble> dialog { get; set; }

        public BitmapImage bitmap { get; set; }
    }
}
