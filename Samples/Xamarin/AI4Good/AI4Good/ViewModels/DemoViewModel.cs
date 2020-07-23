using AI4Good.Models;
using AI4Good.Renderers;
using AI4Good.Services;
using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AI4Good.ViewModels
{
    public class DemoViewModel:BaseViewModel
    {
        #region properties
        public ObservableCollection<Conversation> Conversations { get; set; }
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
        private void InitializeCommands()
        {
            MuteCommand = new Command(() => ExecuteMuteCommand());
            YesCommand = new Command(() => ExecuteYesCommand());
            NoCommand = new Command(() => ExecuteNoCommand());
            RepeatCommand = new Command(() => ExecuteRepeatCommand());
            HelpCommand = new Command(() => ExecuteHelpCommand());
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
