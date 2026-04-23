using Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using Krugozor.Identity.Extensions;
using Microsoft.AspNetCore.Authorization;
using Krugozor.Infrastructure.Database;
using Krugozor.Core.Models.Messages;
using Krugozor.Infrastructure.Specifications;
using AutoMapper;
using System.Diagnostics;
using Microsoft.Extensions.Logging;


namespace Krugozor.Infrastructure.Messenger.Hubs
{

    public interface IChatHub
    {

        Task<object> SendMessageToUserMain(string message, string guId, int recepientId);
        Task<object> GetChats();
        Task ReadMessagesInChat(int chatId);
        Task<object> GetUnreadMessages();
        Task<object> GetAllMessagesOnConnect(int recepientId);
        Task<object> GetconnectionItems();
        Task<object> SendMessageToUserFromService(Message message);


    }



}