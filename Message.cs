using System;
using System.Collections.Generic;
using System.Text;

namespace ailurus
{
    public enum MessageType
    {
        Info,
        Dialog,
        Fighting,
        Healing
    }

    public class Message
    {
        public string Contents { get; set; }
        public MessageType MessageType { get; set; }

        public Message(string contents, MessageType type)
        {
            Contents = contents;
            MessageType = type;
        }
    }
}
