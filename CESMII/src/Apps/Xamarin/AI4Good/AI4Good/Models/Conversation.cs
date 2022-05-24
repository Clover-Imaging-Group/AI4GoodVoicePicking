using System;
using System.Collections.Generic;
using System.Text;

namespace AI4Good.Models
{
    public class Conversation
    {
        bool _isAI = false;
        public bool IsAI
        {
            get
            {
                return _isAI;
            }
            set
            {
                _isAI = value;
                if (_isAI)
                {
                    GridColumn = 1;
                    AIImageSource = "avatar.png";
                    ShowAIImage = true;
                    ShowUserImage = false;
                    ConversationColor = "#EEEEEE";
                }
                else
                {
                    GridColumn = 0;
                    UserImageSource = "user.png";
                    ShowAIImage = false;
                    ShowUserImage = true;
                    ConversationColor = "#EEEEEE";
                }
            }
        }
        public bool ShowAIImage { get; set; }
        public bool ShowUserImage { get; set; }
        public int GridColumn { get; set; }
        public string ConversationColor { get; set; }
        public string AIImageSource { get; set; }
        public string UserImageSource { get; set; }
        public string Message { get; set; }
    }
}
