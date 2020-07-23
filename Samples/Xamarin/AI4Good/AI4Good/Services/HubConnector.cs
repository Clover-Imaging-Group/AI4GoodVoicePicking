using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI4Good.Services
{
    public delegate void MsgDelegate(string str, string str2);
    public class HubConnector
    {
        private HubConnection _hubConnection;
        private readonly string _DEFAULT_ConversationalHubUrl = "https://oosai4good-webapp.azurewebsites.net/ConversationalHub";
        private readonly string _DEFAULT_SendMessageMethodName = "SendMessage";
        private readonly string _DEFAULT_GetText2SpeechMethodName = "GetText2Speech";
        private readonly string _DEFAULT_Text2SpeechResponseMethodName = "Text2SpeechResponse";
        public event MsgDelegate TTSResponseDelegate;
        public HubConnector()
        {
            InitializeHub();
        }
        private void InitializeHub()
        {
            this.TTSResponseDelegate += HubConnector_TTSResponseDelegate;
            HubConnectionBuilder builder = new HubConnectionBuilder();
            _hubConnection = builder.WithUrl(_DEFAULT_ConversationalHubUrl).WithAutomaticReconnect().Build();
            _hubConnection.Closed += HubConnection_Closed;
            _hubConnection.Reconnected += HubConnection_Reconnected;
            _hubConnection.Reconnecting += HubConnection_Reconnecting;
            _hubConnection.On<string, string, DateTime>("Log", LogReceived);
            _hubConnection.On<string, string>(_DEFAULT_Text2SpeechResponseMethodName, TTSResponseReceived);
        }

        private Task HubConnection_Reconnected(string arg)
        {
            throw new NotImplementedException();
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            throw new NotImplementedException();
        }
        private void HubConnector_TTSResponseDelegate(string str, string str2)
        {
        }
        private void TTSResponseReceived(string user, string message)
        {
            this.TTSResponseDelegate.Invoke(user, message);
        }
        private void LogReceived(string user, string message, DateTime utcTime)
        {
            throw new NotImplementedException();
        }
        private Task HubConnection_Closed(System.Exception arg)
        {
            throw new System.NotImplementedException();
        }
        public HubConnectionState GetState()
        {
            return _hubConnection.State;
        }
        public Task StartAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected || _hubConnection.State == HubConnectionState.Connected)
                return null;
            else
                return _hubConnection.StartAsync();
        }
        public Task StopAsync()
        {
            return _hubConnection.StopAsync();
        }
        public void SendMessage(string UserName, string Message, CancellationToken cancellationToken = default)
        {
            _hubConnection.SendAsync(_DEFAULT_SendMessageMethodName, UserName, Message);
        }
        public void GetText2Speech(string UserName, string Message, CancellationToken cancellationToken = default)
        {
            _hubConnection.SendAsync(_DEFAULT_GetText2SpeechMethodName, UserName, Message);
        }


    }
}
