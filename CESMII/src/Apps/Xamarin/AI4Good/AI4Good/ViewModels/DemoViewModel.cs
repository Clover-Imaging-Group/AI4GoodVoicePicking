using AI4Good.Models;
using AI4Good.Renderers;
using AI4Good.Services;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public ObservableCollection<BotAction> AvailableActions { get; set; }
        public Item NextItemToPick { get; set; }
        public List<Item> ItemsPicked { get; set; }
        public User User { get; set; }
        public string ScannedItemIds { get; set; }

        BotConnectorService _bot;

        public string UserName
        {
            get
            {
                if (User == null) return "AI4GoodUser";

                return User.UserName;
            }
        }

        string _messageInput;
        public string MessageInput
        {
            get
            {
                return _messageInput;
            }
            set
            {
                SetProperty(ref _messageInput, value);
            }
        }
        bool _isSendVisible = false;
        public bool IsSendVisible
        {
            get
            {
                return _isSendVisible;
            }
            set
            {
                SetProperty(ref _isSendVisible, value);
            }
        }

        private bool _areOptionsVisible;
        public bool AreOptionsVisible
        {
            get
            {
                return _areOptionsVisible;
            }
            set
            {
                SetProperty(ref _areOptionsVisible, value);
            }
        }

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
            AvailableActions = new ObservableCollection<BotAction>();
            //webAPIService = new WebAPIService();

            ItemsToPick = new ObservableCollection<Item>();
            ItemsPicked = new List<Item>();
            ScannedItemIds = "";
            MuteText = "MUTE";
            InitializeCommands();
            InitializeAudioServices();
            InitializeHubConnector();
        }

        #region HubConnector
        HubConnector hubConnector;
        private async Task InitializeHubConnector()
        {
            hubConnector = new HubConnector(UserName);
            await hubConnector.StartAsync();
            hubConnector.TTSResponseDelegate += HubConnector_TTSResponseDelegate;
            //hubConnector.SpeechToTextDelegate += HubConnector_SpeechToTextDelegate;
            IsSendVisible = true;
            await Task.Delay(500);
            _bot = new BotConnectorService(hubConnector);
            _bot.UIAction += _bot_UIAction;
            await _bot.StartBotConversation(UserName);
        }

        private void _bot_UIAction(string message, IList<Microsoft.Bot.Connector.DirectLine.CardAction> actions)
        {
            //if(str.ToUpper()== "Please select an item to pick".ToUpper())
            //{
            //    GetItemsToPick("PICK");
            //}
            AvailableActions.Clear();
            if (actions.Count < 1) return;
            foreach (var action in actions)
            {
                AvailableActions.Add(new BotAction
                {
                    ActionText = action.Value.ToString()
                });
            }
        }

        private async void HubConnector_TTSResponseDelegate(string user, string message, string originalMessage)
        {
            if (!string.IsNullOrEmpty(message))
            {
                
                while (Audio.IsPlaying)
                {
                    await Task.Delay(100);
                }
                lastTTS = message;
                lastAIText = originalMessage;
                await Task.Delay(100);
                MuteText = "MUTE";
                Audio.PlayBase64(message);

                if(originalMessage=="base64speech")
                {
                    await HandleBotMessageAsync(message, false);
                }
                else
                    Conversations.Add(new Conversation { IsAI = true, Message = originalMessage });
            }
        }
        #endregion

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
            //recorder.StartRecording();
        }

        private void Audio_StateChanged(object sender, EventArgs e)
        {
            if (MuteText != "MUTE" && !recorder.IsRecording)
            {
                recorder.StartRecording().ConfigureAwait(false);
            }
            if (!Audio.IsPlaying)
            {
                MuteText = "MUTE";
                recorder.StartRecording().ConfigureAwait(false);
            }
        }

        private async void Recorder_AudioInputReceived(object sender, string e)
        {
            if (sender.GetType() == typeof(AudioRecorderService))
            {
                var arService = (AudioRecorderService)sender;
                var filePath = arService.FilePath;
                SendAudioForTranslation(filePath);
            }
            if (!recorder.IsRecording)
            {
                //await Task.Delay(1000);
                await recorder.StartRecording();
            }
        }

        private void SendAudioForTranslation(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                var fileByteArray = ReadBytesFromStream(stream);
                var base64message = Convert.ToBase64String(fileByteArray);
                hubConnector.GetSpeech2TextMethodName(UserName, base64message);
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

        #region Commands
        public Command MenuCommand { get; set; }
        public Command MuteCommand { get; set; }
        public Command YesCommand { get; set; }
        public Command NoCommand { get; set; }
        public Command RepeatCommand { get; set; }
        public Command HelpCommand { get; set; }
        public Command CheckinCommand { get; set; }
        public Command SendCommand { get; set; }
        public Command OptionsCommand { get; set; }
        public Command PickItemCommand { get; set; }
        public Command SelectActionCommand { get; set; }


        private void InitializeCommands()
        {
            MenuCommand = new Command(() => ExecuteMenuCommand());
            MuteCommand = new Command(() => ExecuteMuteCommand());
            YesCommand = new Command(async () => await ExecuteYesCommand());
            NoCommand = new Command(async () => await ExecuteNoCommand());
            RepeatCommand = new Command(() => ExecuteRepeatCommand());
            HelpCommand = new Command(() => ExecuteHelpCommand());
            //CheckinCommand = new Command(() => ExecuteCheckinCommand());
            OptionsCommand = new Command((option) => GetItemsToPick(option));
            PickItemCommand = new Command(async (e) => await ExecutePickItemCommand(e));
            SendCommand = new Command((e) => ExecuteSendCommand());
            SelectActionCommand = new Command(async (e) => await ExecuteSelectActionCommand(e));

        }


        private void DisplayUserMessage(bool showMessage = false, string message = "", string color = "Red")
        {
            IsItemPickingErrorTextVisible = showMessage;
            ItemPickingErrorText = message;
            TextColor = color;
        }

        private async Task HandleBotMessageAsync(string message, bool isAi)
        {
            Conversations.Add(new Conversation { IsAI = isAi, Message = message });
            await _bot.SendMessage(message);
        }

        private async Task ExecuteMenuCommand()
        {
            await HandleBotMessageAsync("Workflow", false);
        }
        private async Task ExecuteYesCommand()
        {
            await HandleBotMessageAsync("Yes", false);
        }

        private async Task ExecuteNoCommand()
        {
            await HandleBotMessageAsync("No", false);
        }

        private void ExecuteRepeatCommand()
        {
            Conversations.Add(new Conversation { IsAI = false, Message = "Repeat." });
            Conversations.Add(new Conversation { IsAI = true, Message = lastAIText });
            Audio.PlayBase64(lastTTS);
        }

        private async void ExecuteHelpCommand()
        {
            await HandleBotMessageAsync("Help", false);
        }

        private async void ExecuteSendCommand()
        {
            CurrentSpeech = MessageInput;
            MessageInput = string.Empty;
            Conversations.Add(new Conversation { IsAI = false, Message = CurrentSpeech });
            await _bot.SendMessage(CurrentSpeech);
        }

        private async Task ExecuteSelectActionCommand(object action)
        {
            var _action = (BotAction)action;
            await HandleBotMessageAsync(_action.ActionText, false);


        }

        // Get Items for Picking
        private async void GetItemsToPick(object option)
        {
            await _bot.SendMessage(option.ToString());
            if (option.ToString() != "PICK")
                return;

            webAPIService = new WebAPIService("picking");
            var result = await webAPIService.GetItemsToPickAsync();
            if (result.Count > 0)
                result.ToList().ForEach(item => { ItemsToPick.Add(item); });
            else
                hubConnector.GetText2Speech(UserName, "No Orders avaialble for picking. Please try again later.");
        }


        private async Task ExecutePickItemCommand(object selectedItem)
        {
            var item = (Item)selectedItem;
            var currentItemIndex = ItemsToPick.IndexOf(item);
            if (currentItemIndex != -1 && currentItemIndex < ItemsToPick.Count)
            {
                if ((NextItemToPick != null && NextItemToPick.ItemId != item.ItemId) || (NextItemToPick == null && currentItemIndex != 0))
                {
                    // Out of order item picked or First item being clicked upon. Check if it's the first item in the list
                    hubConnector.GetText2Speech(UserName, "Item picked out of order. Please pick item at the top.");
                }
                else
                {
                    if (ItemsToPick.Count > 1)
                        NextItemToPick = ItemsToPick[currentItemIndex + 1];
                    
                    ItemsPicked.Add(ItemsToPick[currentItemIndex]);
                    IsItemPickingErrorTextVisible = false;
                    if (currentItemIndex == ItemsToPick.Count - 1)
                    {   // Last item. Make an Update API call

                        webAPIService = new WebAPIService("picking");
                        var scannedItemIds = "";
                        var scannedItemCount = ItemsPicked.Count;
                        for (int i = 0; i < scannedItemCount; i++)
                            scannedItemIds += i == scannedItemCount - 1 ? $"{ItemsPicked[i].ItemId}" : $"{ItemsPicked[i].ItemId},";
                        try
                        {
                            await webAPIService.UpatePickedItemsByIdsAsync(scannedItemIds);
                            NextItemToPick = null;
                            ItemsPicked.Clear();
                            hubConnector.GetText2Speech(UserName, "Congratulations! Items in the Order picked successfully. \n Select Pick to fetch items from next order");
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
