using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ConversationalAI.Infrastructure.Interfaces.Services;
using ConversationalAI.SpeechService.Models.Config;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace ConversationalAI.SpeechService.Services
{
    public class SpeechService : ISpeechService
    {
        private readonly SpeechConfig _speechConfig;
        private readonly HttpClient _httpClient;
        // private readonly  SpeechServiceSettings _speechServiceSettings;
        
        public SpeechService(IOptionsMonitor<SpeechServiceSettings> speechSettings, HttpClient httpClient)
        {
            var speechServiceSettings = speechSettings.CurrentValue;
            _speechConfig = SpeechConfig.FromSubscription(speechServiceSettings.SubscriptionKey, speechServiceSettings.AzureRegion);
            _speechConfig.SpeechRecognitionLanguage = speechServiceSettings.Language;
            _httpClient = httpClient;
        }

        public async Task<string> GetSpeech(string message)
        {
            return await ConvertTextToSpeechBase64(message);
        }

        public string ConvertToBase64(byte[] audioBytes)
        {
            try
            {
                return Convert.ToBase64String(audioBytes);
            }
            catch (Exception e)
            {
                var message = $"An error occurred converting the {nameof(audioBytes)}. {e.Message}";
                throw;
            }
        }

        public async Task<string> ConvertTextToSpeechBase64(string text)
        {
            var data = await ConvertTextToSpeechAsync(text);
            return ConvertToBase64(data);
        }

        public async Task<byte[]> ConvertTextToSpeechAsync(string text)
        {
            
            using var synthesizer = new SpeechSynthesizer(_speechConfig);
            using var result = await synthesizer.SpeakTextAsync(text);
            
            switch (result.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    return result.AudioData;
                case ResultReason.Canceled:
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                    
                    throw new Exception("Speech synthesis canceled");
                }
                default:
                    throw new Exception("Speech synthesis failed");
            }
        }

        public async Task<string> ConvertSpeechToTextAsync([NotNull] string audioBase64)
        {
            var audioBytes = Convert.FromBase64String(audioBase64);
            return await ConvertSpeechToTextAsync(audioBytes);
        }

        public async Task<string> ConvertSpeechToTextAsync([NotNull] byte[] audioBytes)
        {
            try
            {
                // var reader = new BinaryReader(new MemoryStream(audioBytes));
                // using var audioInputStream = AudioInputStream.CreatePushStream();
                // using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
                // using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);
                //
                // byte[] readBytes;
                // do
                // {
                //     readBytes = reader.ReadBytes(1024);
                //     audioInputStream.Write(readBytes, readBytes.Length);
                // } while (readBytes.Length > 0);
                //
                // var result = await recognizer.RecognizeOnceAsync();
                // return result.Text;

                // var reader = new BinaryReader(new MemoryStream(audioBytes));
                // using var audioInputStream = AudioInputStream.CreatePushStream();
                // using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
                // using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);
                //
                // byte[] readBytes;
                // do
                // {
                //     readBytes = reader.ReadBytes(1024);
                //     audioInputStream.Write(readBytes, audioBytes.Length);
                // } while (readBytes.Length > 0);
                //
                // // audioInputStream.Write(audioBytes, audioBytes.Length);
                // var result = await recognizer.RecognizeOnceAsync();
                // return result.Text;
                var endPoint =
                    $"https://{_speechConfig.Region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US";
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _speechConfig.SubscriptionKey);
                using var content = new ByteArrayContent(audioBytes);
                
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                
                var response = await _httpClient.PostAsync(endPoint, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SpeechToTextResult>(responseString);
                    return result.DisplayText;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
    
    public class SpeechToTextResult
    {
        public string RecognitionStatus { get; set; }
        public string DisplayText { get; set; }
        public int Offset { get; set; }
        public int Duration { get; set; }
    }
}