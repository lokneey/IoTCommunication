using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using JWTAuthApi.Models;

namespace JWTAuthApi.Services
{
    public class MessagesSorter
    {
        public MessagesSorterModel _messagesSorterModel;

        public MessagesSorter(MessagesSorterModel messagesSorterModel)
        {
            _messagesSorterModel = messagesSorterModel;
        }

        public MessagesSorterModel MostImportantMessagesJson(List<MessageModel> allMessages)  
        {
            _messagesSorterModel = new MessagesSorterModel();
            _messagesSorterModel.ToRemoveMessages = new List<MessageModel>();
            _messagesSorterModel.HighImportanceMessages = new List<MessageModel>();
            _messagesSorterModel.MediumImportanceMessages = new List<MessageModel>();
            _messagesSorterModel.LowImportanceMessages = new List<MessageModel>();
            _messagesSorterModel.ActualGatheredMessages = new List<MessageModel>();
            int messageDeathFlag;
            
            foreach (var msg in allMessages)
            {                
                msg.MessageDeathForAspNet = DateTime.ParseExact(msg.MessageDeath, "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                messageDeathFlag = DateTime.Compare(msg.MessageDeathForAspNet, DateTime.Now);                
                if (messageDeathFlag < 0)
                {
                    _messagesSorterModel.ToRemoveMessages.Add(msg);
                }
                else
                {                    
                    if (msg.MessagePriority == "HIGH")
                    {
                        _messagesSorterModel.HighImportanceMessages.Add(msg);
                    }
                    else if (msg.MessagePriority == "MEDIUM")
                    {
                        _messagesSorterModel.MediumImportanceMessages.Add(msg);
                    }
                    else if (msg.MessagePriority == "LOW")
                    {
                        _messagesSorterModel.LowImportanceMessages.Add(msg);
                    }
                    else
                    {
                        _messagesSorterModel.MediumImportanceMessages.Add(msg);
                    }
                }                
            }
            if (_messagesSorterModel.HighImportanceMessages.Count > 1)
            {
                _messagesSorterModel.HighImportanceMessages.Sort((x, y) => DateTime.Compare(x.MessageDeathForAspNet, y.MessageDeathForAspNet));
            }
            if (_messagesSorterModel.MediumImportanceMessages.Count > 1)
            {
                _messagesSorterModel.MediumImportanceMessages.Sort((x, y) => DateTime.Compare(x.MessageDeathForAspNet, y.MessageDeathForAspNet));
            }
            if (_messagesSorterModel.LowImportanceMessages.Count > 1)
            {
                _messagesSorterModel.LowImportanceMessages.Sort((x, y) => DateTime.Compare(x.MessageDeathForAspNet, y.MessageDeathForAspNet));
            }
            if (_messagesSorterModel.HighImportanceMessages != null)
            {
                _messagesSorterModel.ActualGatheredMessages.AddRange(_messagesSorterModel.HighImportanceMessages); 
            }
            if (_messagesSorterModel.MediumImportanceMessages != null)
            {
                _messagesSorterModel.ActualGatheredMessages.AddRange(_messagesSorterModel.MediumImportanceMessages);
            }
            if (_messagesSorterModel.LowImportanceMessages != null)
            {
                _messagesSorterModel.ActualGatheredMessages.AddRange(_messagesSorterModel.LowImportanceMessages);
            }

            StringBuilder jsonFiveMessages = new StringBuilder();

            int i = 0;
            int actualGatheredMessagesListEnd = _messagesSorterModel.ActualGatheredMessages.Count;
            if (actualGatheredMessagesListEnd > 1)
            {
                jsonFiveMessages.Append("[");
            }
            if (actualGatheredMessagesListEnd > 5)
            {
                actualGatheredMessagesListEnd = 5;
            }
            while (i < actualGatheredMessagesListEnd)
            {
                jsonFiveMessages.Append(JsonConvert.SerializeObject(_messagesSorterModel.ActualGatheredMessages[i]));
                if (i < actualGatheredMessagesListEnd -1)
                {
                    jsonFiveMessages.Append(",");
                }
                i++;
            }
            if (actualGatheredMessagesListEnd > 1)
            {
                jsonFiveMessages.Append("]");
            }
            _messagesSorterModel.jsonResponse = jsonFiveMessages;
            return _messagesSorterModel;
        }

        public StringBuilder AllMessagesJson(List<MessageModel> allMessages) 
        {
            StringBuilder jsonAllMessages = new StringBuilder();
            if (allMessages.Count > 1)
            {
                jsonAllMessages.Append("[");
            }
            int i = 1;
            foreach (var msg in allMessages)
            {
                jsonAllMessages.Append(JsonConvert.SerializeObject(msg));
                if (i < allMessages.Count)
                {
                    jsonAllMessages.Append(",");
                }
                i++;
            }
            if (allMessages.Count > 1)
            {
                jsonAllMessages.Append("]");
            }
            _messagesSorterModel.jsonResponse = jsonAllMessages;
            return jsonAllMessages;
        }
    }
}
