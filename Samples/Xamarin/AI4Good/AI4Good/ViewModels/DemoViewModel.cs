using AI4Good.Models;
using AI4Good.Renderers;
using AI4Good.Services;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AI4Good.ViewModels
{
    public class DemoViewModel:BaseViewModel
    {
        #region properties
        WebAPIService webAPIService;
        public ObservableCollection<Conversation> Conversations { get; set; }
        public ObservableCollection<UserRole> UserRoles { get; set; }
        public ObservableCollection<Item> ItemsToPick { get; set; }
        public Item NextItemToPick { get; set; }
        public List<Item> ItemsPicked { get; set; }
        public User User { get; set; }
        public string ScannedItemIds { get; set; }

        string _itemPickingErrorText = "";
        public string ItemPickingErrorText
        {
            get
            {
                return _itemPickingErrorText;
            }
            set
            {
                SetProperty(ref _itemPickingErrorText, value);
            }
        }

        bool _isItemPickingErrorTextVisible;
        public bool IsItemPickingErrorTextVisible
        {
            get
            {
                return _isItemPickingErrorTextVisible;
            }
            set
            {
                SetProperty(ref _isItemPickingErrorTextVisible, value);
            }
        }

        string _apiCallButtonText;
        public string APICallButtonText
        {
            get
            {
                return _apiCallButtonText;
            }
            set
            {
                SetProperty(ref _apiCallButtonText, value);
            }
        }

        string _textColor;
        public string TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                SetProperty(ref _textColor, value);
            }
        }

        string _currentSpeech;
        public string CurrentSpeech
        {
            get
            {
                return _currentSpeech;
            }
            set
            {
                SetProperty(ref _currentSpeech, value);
            }
        }
        string lastAIText;
        string lastTTS;
        string _muteText;
        public string MuteText
        {
            get
            {
                return _muteText;
            }
            set
            {
                SetProperty(ref _muteText, value);
            }
        }
        bool firstLoad = true;
        #endregion
        public DemoViewModel()
        {
            Conversations = new ObservableCollection<Conversation>();
            UserRoles = new ObservableCollection<UserRole>();
            ItemsToPick = new ObservableCollection<Item>();
            ItemsPicked = new List<Item>();
            ScannedItemIds = "";
            MuteText = "MUTE";
            InitializeCommands();
            InitializeAudioServices();
            InitializeHubConnector();
        }
        #region AudioServices
        AudioRecorderService recorder;
        IAudio _audio;
        public IAudio Audio
        {
            get
            {
                return _audio;
            }
            set
            {
                SetProperty(ref _audio, value);
            }
        }
        private void InitializeAudioServices()
        {
            Audio = DependencyService.Get<IAudio>();
            Audio.StateChanged += Audio_StateChanged;
            recorder = new AudioRecorderService
            {
                AudioSilenceTimeout = TimeSpan.FromSeconds(2),
                StopRecordingOnSilence = true, //will stop recording after 2 seconds (default)
                StopRecordingAfterTimeout = true//,  //stop recording after a max timeout (defined below)
                //TotalAudioTimeout = TimeSpan.FromSeconds(15) //audio will stop recording after 15 seconds
            };
            recorder.AudioInputReceived += Recorder_AudioInputReceived;
        }
        private void Audio_StateChanged(object sender, EventArgs e)
        {
            if (MuteText != "MUTE" && !recorder.IsRecording)
            {
                recorder.StartRecording();
            }
        }
        private void Recorder_AudioInputReceived(object sender, string e)
        {
            if (sender.GetType() == typeof(AudioRecorderService))
            {
                //MicrophoneImage = "mic_off.png";
                var arService = (AudioRecorderService)sender;
                var filePath = arService.FilePath;
                //SendAudioForTranslation(filePath);
            }
        }

        private async void SendAudioForTranslation(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                var fileByteArray = ReadBytesFromStream(stream);
                //var selectedLanguage = this.SelectedLanguage;
                //var speechResult = await speechServices.SynthesiseSpeech(fileByteArray, selectedLanguage);
                //if (speechResult != null)
                //{
                //    if (speechResult.DisplayText == null)
                //        return;

                //    if (speechResult.DisplayText.Length <= 30)
                //        GetBotIntent(speechResult.DisplayText);
                //    else
                //        MetalDetectorDemo(speechResult.DisplayText);
                //}
            }
        }
        public byte[] ReadBytesFromStream(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        #endregion
        #region HubConnector
        HubConnector hubConnector;
        private async Task InitializeHubConnector()
        {
            hubConnector = new HubConnector();
            hubConnector.TTSResponseDelegate += HubConnector_TTSResponseDelegate;
            hubConnector.StartAsync();
            await Task.Delay(500);
            APICallButtonText = "Get Items";
            CurrentSpeech = "We invite you to experience the future of AI that will empower your employees with disabilities to pick a warehouse order and inspire your team.";
            Conversations.Add(new Conversation { IsAI = true, Message = CurrentSpeech });

            hubConnector.GetText2Speech("XamarinDemoApp", CurrentSpeech);
            CurrentSpeech = "Are you ready to start picking?";
            lastAIText = CurrentSpeech;
            hubConnector.GetText2Speech("XamarinDemoApp", CurrentSpeech);
        }

        private async void HubConnector_TTSResponseDelegate(string user, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {

                while(Audio.IsPlaying)
                {
                    await Task.Delay(100);
                }
                lastTTS = message;
                Audio.PlayBase64(lastTTS);
                if (firstLoad)
                {
                    firstLoad = false;
                    Conversations.Add(new Conversation { IsAI = true, Message = CurrentSpeech });
                }
            }
        }
        #endregion
        #region Commands
        public Command MuteCommand { get; set; }
        public Command YesCommand { get; set; }
        public Command NoCommand { get; set; }
        public Command RepeatCommand { get; set; }
        public Command HelpCommand { get; set; }
        public Command GetItemsToPickCommand { get; set; }
        public Command PickItemCommand { get; set; }

        private void InitializeCommands()
        {
            MuteCommand = new Command(() => ExecuteMuteCommand());
            YesCommand = new Command(() => ExecuteYesCommand());
            NoCommand = new Command(() => ExecuteNoCommand());
            RepeatCommand = new Command(() => ExecuteRepeatCommand());
            HelpCommand = new Command(() => ExecuteHelpCommand());
            //CheckinCommand = new Command(() => ExecuteCheckinCommand());
            GetItemsToPickCommand = new Command(() => GetItemsToPick());
            PickItemCommand = new Command(async (e) => await ExecutePickItemCommand(e));
        }

        private async Task ExecutePickItemCommand(object selectedItem)
        {
            var item = (Item)selectedItem;
            var currentItemIndex = ItemsToPick.IndexOf(item);
            if(currentItemIndex != -1 && currentItemIndex < ItemsToPick.Count)
            {
                if((NextItemToPick != null && NextItemToPick.ItemId != item.ItemId) || (NextItemToPick == null && currentItemIndex != 0))
                {// Out of order item picked or First item being clicked upon. Check if it's the first item in the list
                    DisplayUserMessage(true, "Item picked out of order. Please pick item at the top.", "Red");
                }
                else
                {
                    if(ItemsToPick.Count > 1)
                    {
                        NextItemToPick = ItemsToPick[currentItemIndex + 1];
                    }
                    ItemsPicked.Add(ItemsToPick[currentItemIndex]);
                    IsItemPickingErrorTextVisible = false;
                    if (currentItemIndex == ItemsToPick.Count - 1)
                    {   // Last item. Make an Update API call
                        
                        webAPIService = new WebAPIService("picking");
                        var scannedItemIds = "";
                        var scannedItemCount = ItemsPicked.Count;
                        for (int i=0; i< scannedItemCount; i++)
                            scannedItemIds += i == scannedItemCount-1 ? $"{ItemsPicked[i].ItemId}" : $"{ItemsPicked[i].ItemId},";
                        try
                        {
                            await webAPIService.UpatePickedItemsByIdsAsync(scannedItemIds);
                            NextItemToPick = null;
                            ItemsPicked.Clear();
                            DisplayUserMessage(true, "Congratulations! Items in the Order picked successfully. \n Click GET ITEMS to fetch items from next order", "Green");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                    
                    ItemsToPick.Remove(ItemsToPick[currentItemIndex]);
                }
            }
            
        }

        private void DisplayUserMessage(bool showMessage = false, string message = "", string color = "Red")
        {
            IsItemPickingErrorTextVisible = showMessage;
            ItemPickingErrorText = message;
            TextColor = color;
        }
        private void ExecuteYesCommand()
        {
            Conversations.Add(new Conversation { IsAI = false, Message = "Yes." });
        }
        private void ExecuteNoCommand()
        {
            Conversations.Add(new Conversation { IsAI = false, Message = "No." });
        }
        private void ExecuteRepeatCommand()
        {
            Conversations.Add(new Conversation { IsAI = false, Message = "Repeat." });
            Conversations.Add(new Conversation { IsAI = true, Message = lastAIText });
            Audio.PlayBase64(lastTTS);
        }
        private void ExecuteHelpCommand()
        {
            Conversations.Add(new Conversation { IsAI = false, Message = "Help." });
            CurrentSpeech = "Ok, no problem! I will send a supervisor to help you.";
            lastAIText = CurrentSpeech;
            Conversations.Add(new Conversation { IsAI = true, Message = lastAIText });
            hubConnector.GetText2Speech("XamarinDemoApp", CurrentSpeech);
        }
        private async void ExecuteCheckinCommand()
        {
            var speech = "Call made successfully";
            webAPIService = new WebAPIService("values");
            //GetUserRoles();
            var user = await GetUsersById(new Guid("9b38397e-c459-4555-ba21-0992d4971c4c")); // Hardcoded user Id from Azure SQL server db
            if(user != null)
            {
                APICallButtonText = "CHECKED-IN";
                User = user;
            }
            Audio.PlayBase64(speech);
        }

        // Get Items for Picking
        private async void GetItemsToPick()
        {
            webAPIService = new WebAPIService("picking");
            var result = await webAPIService.GetItemsToPickAsync();
            if(result.Count > 0)
            {
                DisplayUserMessage(false, "", "");
                result.ToList().ForEach(item => {
                    ItemsToPick.Add(item);
                });
            }
            else
                DisplayUserMessage(true, "No Orders avaialble for picking. Please try again later.", "Red");
        }

        // Gets the User by matching ID available in the database
        private async Task<User> GetUsersById(Guid id)
        {
            var result = await webAPIService.GetUserByIdAsync(id);
            return result;
        }
        private void ExecuteMuteCommand()
        {
            if(MuteText == "MUTE")
            {
                MuteText = "UN-MUTE";
                recorder.StartRecording();
            }
            else
            {
                MuteText = "MUTE";
                recorder.StopRecording();
            }
        }
        #endregion
    }
}
