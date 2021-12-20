using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrumokApp.Data
{
    public enum eMessageLevel { Default, Info, Warning, Error,
        Success
    }
    public delegate void MessageDelegate(string title, string message, eMessageLevel messageLevel);

    public class MessageContainer
    {
        public MessageContainer(string text, eMessageLevel messageLevel)
        {
            Text = text;
            MessageLevel = messageLevel;
        }

        public string Text { get; }
        public eMessageLevel MessageLevel { get; }
    }
}
