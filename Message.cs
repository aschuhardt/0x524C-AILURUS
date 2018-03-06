using System;

namespace ailurus
{
    public enum MessageType
    {
        Info,
        Dialog,
        Combat,
        Healing
    }

    public class Message
    {
        public string Contents { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(string contents, MessageType type)
        {
            Contents = contents;
            MessageType = type;
            Timestamp = DateTime.Now;
        }
    }
}
