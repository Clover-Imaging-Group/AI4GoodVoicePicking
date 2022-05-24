using AI4Good.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AI4Good.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DemoPage : ContentPage
    {
        public DemoPage()
        {
            InitializeComponent();
        }

        private void ConversationList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ConversationList.ItemsSource == null)
                return;
            var last = ConversationList.ItemsSource.Cast<object>().LastOrDefault();
            ConversationList.ScrollTo(last, ScrollToPosition.MakeVisible, true);
        }
        private void ConversationList1_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            //if (ItemsListToPickFrom.ItemsSource == null)
            //    return;
            //var last = ItemsListToPickFrom.ItemsSource.Cast<object>().LastOrDefault();
            //ItemsListToPickFrom.ScrollTo(last, ScrollToPosition.MakeVisible, true);
        }
    }
}