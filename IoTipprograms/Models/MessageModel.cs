using System;
using System.Collections.Generic;

namespace IoTipprograms.Models
{
    public class MessageModel
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        public string MessageAuthor { get; set; }
        public string MessagePriority { get; set; }        
        public string MessageBirth { get; set; }
        public string MessageDeath { get; set; }
        public string MessageException { get; set; }
        public DateTime MessageDeathForAspNet { get; set; }
        public List<MessageModel> MessagesList;
    }
}
