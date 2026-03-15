using System;
using System.Collections.Generic;

namespace KFA.Vote_Search.Models;

public partial class UserMessage
{
    public int Id { get; set; }
    public long ChatId { get; set; }
    public DateTime MessageDate { get; set; }
    public string LanguageCode { get; set; } = null!;
    public string Message { get; set; } = null!;
}
