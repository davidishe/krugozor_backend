using Core.Models;

namespace Krugozor.Core.Models.Messages
{
    public class Image : BaseEntity
    {
        public int MessageId { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public byte[]? DocByte { get; set; }
        public int Size { get; set; }
    }
}