using Krugozor.Core.Models.Messages;

namespace Krugozor.Infrastructure.Messenger.Hubs
{
    public class ChatCommonInfo
    {
        public int UnreadMessagesCount { get; set; }
        public List<ConnectionItem>? ConnectionItems { get; set; }
        public List<ConnectionItem>? PrtintingUsersItems { get; set; }
    }
}