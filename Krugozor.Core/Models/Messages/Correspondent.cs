using System;
using Core.Models;

namespace Krugozor.Core.Models.Messages
{
    public class Correspondent : BaseEntity
    {
        public int CoresspondentId { get; set; }
        public int AnotherCoresspondentId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}