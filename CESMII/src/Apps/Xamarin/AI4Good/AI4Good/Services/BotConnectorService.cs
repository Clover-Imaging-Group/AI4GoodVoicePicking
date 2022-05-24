using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;

namespace AI4Good.Services
{
    public delegate void ConversationDelegate(string message);
    public delegate void UIActionDelegate(string message, IList<CardAction> actions);
    public class BotConnectorService : IDisposable
	{
        private const string DirectLineSecret = "fMn-tvTSLls.sQcZKJZgyZnR35E7RwNTcDQ4qMmI8Ce8zWanlmhF30Q";
        private string BotId = "AI4Good-CNCYDemo";
        private DirectLineClient _client;
        private Conversation _conversation;
        private readonly HubConnector _hubConnector;
        public event UIActionDelegate UIAction;
        private string _userName;
        private string _channelId;

        public BotConnectorService(HubConnector hubConnector)
        {
            _hubConnector = hubConnector;
            _channelId = Guid.NewGuid().ToString();
        }

        public async Task StartBotConversation(string userName)
        {
            // if you are using a region-specific endpoint, change the uri and uncomment the code
            // var directLineUri = "https://directline.botframework.com/"; // endpoint in Azure Public Cloud
            // DirectLineClient client = new DirectLineClient(new Uri(directLineUri), new DirectLineClientCredentials(directLineSecret));

            _userName = userName;

            _client = new DirectLineClient(DirectLineSecret);
            _client.HttpClient.Timeout = TimeSpan.FromSeconds(120);
            _conversation = await _client.Conversations.StartConversationAsync();
            await Task.Delay(200);
            new System.Threading.Thread(async () =>
            {
                await ReadBotMessagesAsync(_conversation.ConversationId);

            }).Start();

            await Task.Delay(100);
            await SendMessage(string.Empty);
        }

        public async Task SendMessage(string input)
        {
            //if (input.Length > 0)
            {
                Activity userMessage = new Activity()
                {
                    From = new ChannelAccount(_channelId, _userName),
                    Text = input,
                    Type = ActivityTypes.Message
                };

                await _client.Conversations.PostActivityAsync(_conversation.ConversationId, userMessage);
            }
        }


        private async Task ReadBotMessagesAsync(string conversationId)
        {
            string watermark = null;

            while (true)
            {
                var activitySet = await _client.Conversations.GetActivitiesAsync(conversationId, watermark);
                watermark = activitySet?.Watermark;

                var activities = from x in activitySet.Activities
                                 where x.From.Id == BotId
                                 select x;

                foreach (Activity activity in activities)
                {
                    if (activity.Text != "Welcome to your bot.")
                    {
                        string activityText = activity.Text;
                        var actions = new List<string>();
                        if (activity.SuggestedActions != null && activity.SuggestedActions.Actions != null && activity.SuggestedActions.Actions.Count > 0)
                        {
                            foreach (var action in activity.SuggestedActions.Actions)
                                actions.Add(action.Value.ToString());

                            this.UIAction.Invoke(activity.Text, activity.SuggestedActions.Actions);
                        }
                        if (actions.Count > 0 && actions.Any(item => item == "Pick"))
                        {
                            activityText = activityText + " " + (actions.Count > 1 ? string.Join(", ", actions.Take(actions.Count - 1)) + " or " + actions.Last() : actions.FirstOrDefault());
                        }

                        _hubConnector.GetText2Speech(_userName, activityText);
                    }
                    if (activity.Text == "All items in the warehouse have been picked. Thank you.")
                    {
                        this.UIAction.Invoke(activity.Text, new List<CardAction>());
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

