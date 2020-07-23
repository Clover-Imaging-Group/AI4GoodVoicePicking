using AI4Good.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AI4Good.ViewModels
{
    public class MainPageViewModel:BaseViewModel
    {
        public Command StartDemoCommand { get; set; }
        private Xamarin.Forms.Page RootPage { get => Application.Current.MainPage; }
        public MainPageViewModel()
        {
            StartDemoCommand = new Command(async () => await StartDemoCommandExecuted());
        }
        private async Task StartDemoCommandExecuted()
        {
            //DemoViewModel vm = new DemoViewModel();
            var page = new DemoPage();
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

    }
}
