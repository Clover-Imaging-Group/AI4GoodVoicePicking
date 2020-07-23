using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ai4Good_ConversationalAi.Common.Helpers;
using Ai4Good_ConversationalAi.Common.Interfaces;
using Newtonsoft.Json;

namespace Ai4Good_ConversationalAi.AzureSpeech
{
    public class SpeechService : ISpeechService
    {
        SpeechConfig _config = new SpeechConfig();
        public SpeechService(object config)
        {
            _config = (SpeechConfig) config;
        }
        public string ConvertToBase64([NotNull] byte[] audioBytes)
        {
            try
            {
                var speechEncodedBytes = Convert.ToBase64String(audioBytes);
                return speechEncodedBytes;
            }
            catch (Exception e)
            {
                var message = $"An error occurred converting the {nameof(audioBytes)}. {e.Message}";
                throw;
            }
        }

        public string ConvertTextToSpeechBase64([NotNull] string text)
        {
            var data = ConvertTextToSpeechAsync(text).Result;
            return ConvertToBase64(data);
        }

        public byte[] ConvertTextToSpeech([NotNull] string text)
        {
            return ConvertTextToSpeechAsync(text).Result;
        }

        public async Task<byte[]> ConvertTextToSpeechAsync([NotNull] string text)
        {
            var accessToken = await CognitiveServicesHelper.FetchTokenAsync(_config.TokenEndPoint, _config.APIKey);

            var body = $"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='{_config.Language}'>" +
                $"<voice name='Microsoft Server Speech Text to Speech Voice ({_config.Language}, {_config.VoiceName})'" +
                $@">{text}</voice></speak>";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, _config.TextToSpeechEndPoint))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/ssml+xml");
                request.Headers.Add(@"Authorization", $@"Bearer {accessToken}");
                request.Headers.Add(@"Connection", @"Keep-Alive");
                request.Headers.Add(@"User-Agent", _config.AppName);
                request.Headers.Add(@"X-Microsoft-OutputFormat", _config.AudioFormat);

                using (var response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    using (var dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var memStream = new MemoryStream())
                    {
                        await dataStream.CopyToAsync(memStream).ConfigureAwait(false);

                        memStream.Close();

                        return memStream.ToArray();
                    }
                }

            }

        }

        public string GetSpeech(string message)
        {
            return ConvertTextToSpeechBase64(message);
        }
        public async Task<string> ConvertSpeechToText([NotNull] byte[] audioBytes, string lang = "en-US")
        {
            if (_config.Language.ToUpper() != lang.ToUpper())
                _config.Language = lang;
            var baseURI = _config.SynthesisEndPoint;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _config.APIKey);
            byte[] byteData = audioBytes;
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(baseURI, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SpeechSynthesisResult>(responseString);
                    return result.DisplayText;
                }
                else
                    return null;
            }
        }
    }
}
