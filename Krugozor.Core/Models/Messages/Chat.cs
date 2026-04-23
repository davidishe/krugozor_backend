using System;
using System.Collections.Generic;
using Core.Models;

namespace Krugozor.Core.Models.Messages
{
    public class Chat : BaseEntity
    {
        public ICollection<Message>? Messages { get; set; }
        public int RecepientId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}