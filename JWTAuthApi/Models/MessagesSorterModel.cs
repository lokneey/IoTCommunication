using System.Collections.Generic;
using System.Text;

namespace JWTAuthApi.Models
{
    public class MessagesSorterModel
    {
        public List<MessageModel> ActualGatheredMessages { get; set; }
        public List<MessageModel> ToRemoveMessages { get; set; }
        public List<MessageModel> HighImportanceMessages { get; set; }
        public List<MessageModel> MediumImportanceMessages { get; set; }
        public List<MessageModel> LowImportanceMessages { get; set; }
        public StringBuilder jsonResponse { get; set; }
    }
}
