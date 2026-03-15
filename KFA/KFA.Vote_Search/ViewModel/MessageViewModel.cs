using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFA.Vote_Search.ViewModel
{
    public class MessageViewModel
    {
        public int ChatId { get; set; }
        public DateTime MessageDate { get; set; }
        public string LanguageCode { get; set; }
        public string Message { get; set; }
    }
}
