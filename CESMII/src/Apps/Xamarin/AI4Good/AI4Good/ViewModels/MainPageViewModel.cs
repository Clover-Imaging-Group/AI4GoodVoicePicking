using AI4Good.Services;
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
        WebAPIService webAPIService;
        public Command StartDemoCommand { get; set; }
        public Command LoginCommand { get; set; }
        private Xamarin.Forms.Page RootPage { get => Application.Current.MainPage; }

        #region Properties
        bool _isStartDemoButtonVisible;
        public bool IsStartDemoButtonVisible
        {
            get
            {
                return _isStartDemoButtonVisible;
            }
            set
            {
                SetProperty(ref _isStartDemoButtonVisible, value);
            }
        }

        bool _isLoginLayoutVisible = true;
        public bool IsLoginLayoutVisible
        {
            get
            {
                return _isLoginLayoutVisible;
            }
            set
            {
                SetProperty(ref _isLoginLayoutVisible, value);
            }
        }

        bool _isLoginFailed = false;
        public bool IsLoginFailed
        {
            get
            {
                return _isLoginFailed;
            }
            set
            {
                SetProperty(ref _isLoginFailed, value);
            }
        }

        string _usernameValue = "AI4GoodUser";
        public string UsernameValue
        {
            get
            {
                return _usernameValue;
            }
            set
            {
                SetProperty(ref _usernameValue, value);
            }
        }

        string _passwordValue = "P@ssword99";
        public string PasswordValue
        {
            get
            {
                return _passwordValue;
            }
            set
            {
                SetProperty(ref _passwordValue, value);
            }
        }
        string _loginResponseText;
        public string LoginResponseText
        {
            get
            {
                return _loginResponseText;
            }
            set
            {
                SetProperty(ref _loginResponseText, value);
            }
        }
        
        #endregion Properties

        public MainPageViewModel()
        {
            StartDemoCommand = new Command(async () => await StartDemoCommandExecuted());
            LoginCommand = new Command(async () => await LoginCommandExecute());
        }
        private async Task StartDemoCommandExecuted()
        {
            //DemoViewModel vm = new DemoViewModel();
            var page = new DemoPage();
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        private async Task LoginCommandExecute()
        {
            webAPIService = new WebAPIService("Values");
            var loggedInUser = await webAPIService.GetUserByIdAsync(new Guid("e08555a0-1faf-4788-bc62-d4f252465854"));
            if(loggedInUser != null)
            {
                IsStartDemoButtonVisible = true;
                IsLoginLayoutVisible = false;
                IsLoginFailed = false;
                LoginResponseText = "Logged in successfully.";
            }
            else
            {
                // show a message that the login was not successful
                IsLoginFailed = true;
                LoginResponseText = "Invalid Credentials. Please try again.";
            }
            
        }
    }
}
