using AI4Good.Renderers;
using Android.Content;
using Android.Media;
using System;
using System.Threading.Tasks;

namespace AI4Good.Droid.Renderers
{
    public class AudioRenderer : IAudio
    {
        public AudioRenderer()
        {
            StateChanged += AudioService_StateChanged;

            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Completion += _mediaPlayer_Completion;
        }

        private void AudioService_StateChanged(object sender, EventArgs e)
        {
        }

        private MediaPlayer _mediaPlayer;
        private int currentVolume;

        public event EventHandler StateChanged;

        public bool IsPlaying { get; set; }

        public bool PlayBase64(string content)
        {
            IsPlaying = true;
            AudioManager am = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService);

            if (currentVolume == 0)
            {
                var maxVol = am.GetStreamMaxVolume(Stream.Music);
                var currVol = am.GetStreamVolume(Stream.Music);
                currentVolume = currVol == 0 ? maxVol / 2 : currVol;
            }
            am.SetStreamVolume(Stream.Music, currentVolume, VolumeNotificationFlags.AllowRingerModes);
            String url = "data:audio/wav;base64," + content;
            try
            {
                _mediaPlayer.Reset();
                _mediaPlayer.SetDataSource(url);
                _mediaPlayer.Prepare();
                _mediaPlayer.Start();
                StateChanged.Invoke(null, new EventArgs());
            }
            catch (Exception ex)
            {

            }
            return true;
        }
        private void _mediaPlayer_Completion(object sender, EventArgs e)
        {

            AudioManager am = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService);

            //  the "beep" plays on the music audio stream
            currentVolume = am.GetStreamVolume(Stream.Music);
            am.AdjustStreamVolume(Stream.Music, Adjust.ToggleMute, VolumeNotificationFlags.RemoveSoundAndVibrate);
            //_mediaPlayer.Completion -= _mediaPlayer_Completion;
            IsPlaying = false;
            StateChanged.Invoke(null, new EventArgs());
        }
        private async void SetAudioVolumeBackUp()
        {
            await Task.Delay(500);
            AudioManager am = (AudioManager)global::Android.App.Application.Context.GetSystemService(Context.AudioService);
            am.SetStreamVolume(Stream.Music, currentVolume, VolumeNotificationFlags.AllowRingerModes);
        }

    }
}