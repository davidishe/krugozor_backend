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

    public class ChatHub : Hub, IChatHub
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<Message> _messageRepo;
        private IGenericRepository<Correspondent> _correspondentRepo;
        private IGenericRepository<Chat> _chatRepo;
        private readonly ILogger<ChatHub> _logger;
        private readonly IMapper _mapper;
        public static List<ConnectionItem> connectionItems = new List<ConnectionItem>();
        public static List<ConnectionItem> printingUsersItems = new List<ConnectionItem>();

        public ChatHub(
            UserManager<AppUser> userManager,
            IGenericRepository<Correspondent> correspondentRepo,
            IGenericRepository<Message> messageRepo,
            IGenericRepository<Chat> chatRepo,
            ILogger<ChatHub> logger,
            IMapper mapper
        )
        {
            _correspondentRepo = correspondentRepo;
            _userManager = userManager;
            _mapper = mapper;
            _messageRepo = messageRepo;
            _chatRepo = chatRepo;
            _logger = logger;
        }







        //TODO: реализовать клон метода GetMessagess, но который не отмечает сообщения прочитанными


        /// <summary>
        /// Создает сообщение в чате
        /// </summary>
        /// <param name="message"></param>
        /// <param name="guId"></param>
        /// <returns></returns>
        [Authorize]
        [HubMethodName("SendMessageToUser")]
        public async Task<object> SendMessageToUserMain(string message, string guId, int recepientId)
        {
            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;

            // проверяем есть ли чат с данным пользователем через chatRepo
            // если первое сообщение создаем чат
            var chat = await GetChatForMessage(recepientId, user.Id);


            // создаем новое сообщение
            var newMessage = new Message()
            {
                Text = message,
                GuId = guId,
                AuthorId = user.Id,
                RecepientId = recepientId,
                CreatedAt = DateTime.Now,
                IsReaded = false,
                ChatId = chat.Id,
                WithImages = false
            };


            await _messageRepo.AddAsync(newMessage);

            var mappedMessage = _mapper.Map<Message, MessageDto>(newMessage);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var messageToReturn = JsonSerializer.Serialize<MessageDto>(mappedMessage, serializeOptions);

            return Clients.All.SendAsync("ReceiveNewMessage", messageToReturn);
        }




        /// <summary>
        /// Создает сообщение в чате
        /// </summary>
        /// <param name="message"></param>
        /// <param name="guId"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<object> SendMessageToUserFromService(Message message)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var mappedMessage = _mapper.Map<Message, MessageDto>(message);
            var messageToReturn = JsonSerializer.Serialize<MessageDto>(mappedMessage, serializeOptions);

            return Clients.All.SendAsync("ReceiveNewMessage", messageToReturn);
        }





        /// <summary>
        /// Получаем чаты при загрузке приложения
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HubMethodName("GetChats")]
        public async Task<object> GetChats()
        {
            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;

            // собираем объект коннекшен 
            // в будущем объект переедет в сущность чата 
            // но там надо зарезолвить проблему с дублированием сущности Corresopondent для идентификации онлайн статуса
            var spec = new ChatSpecification(user.Id);
            var chats = await _chatRepo.ListAsync(spec);

            var mappedChats = _mapper.Map<IReadOnlyList<Chat>, IReadOnlyList<ChatDto>>(chats);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var chatsToReturn = JsonSerializer.Serialize<IReadOnlyList<ChatDto>>(mappedChats, serializeOptions);
            return Clients.Caller.SendAsync("ReceiveChats", chatsToReturn);
        }




        [Authorize]
        [HubMethodName("ReadMessagesInChat")]
        public async Task ReadMessagesInChat(int chatId)
        {
            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;
            if (user.FirstName is null)
                return;


            // собираем объект коннекшен 
            // в будущем объект переедет в сущность чата 
            // но там надо зарезолвить проблему с дублированием сущности Corresopondent для идентификации онлайн статуса
            var spec = new ChatSpecification(chatId, false);
            var chats = await _chatRepo.ListAsync(spec);

            foreach (var message in chats[0].Messages)
            {
                if (message is not null && (message.RecepientId == user.Id))
                {
                    message.IsReaded = true;
                    await _messageRepo.UpdateAsync(message);
                }
            }

            var mappedChats = _mapper.Map<IReadOnlyList<Chat>, IReadOnlyList<ChatDto>>(chats);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            // var chatsToReturn = JsonSerializer.Serialize<IReadOnlyList<ChatDto>>(mappedChats, serializeOptions);
            // return Clients.All.SendAsync("ReceiveSpecificChat", chatsToReturn);
        }



        /// TODO:   объединить и добавить тут статусы онлайн оффлайн /// 
        [Authorize]
        [HubMethodName("GetUnreadMessages")]
        public async Task<object> GetUnreadMessages()
        {
            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;
            var spec = new MessageSpecification(user.Id, 2.0);
            var messages = await _messageRepo.ListAsync(spec);
            var chatCommonInfo = new ChatCommonInfo()
            {
                UnreadMessagesCount = messages.Count,
                ConnectionItems = connectionItems,
                PrtintingUsersItems = printingUsersItems
            };

            do
            {
                Thread.Sleep(1000);
                return Clients.Caller.SendAsync("Send", chatCommonInfo);
            } while (true);


        }




        /// <summary>
        /// создает или возвращает ранее созданный объект чата для данного пользователя
        /// </summary>
        /// <param name="recepientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<Chat> GetChatForMessage(int recepientId, int userId)
        {
            var chatSpec = new ChatSpecification(recepientId, userId);
            var chat = await _chatRepo.GetEntityWithSpec(chatSpec);

            if (chat is null)
            {
                var newChat = new Chat()
                {
                    RecepientId = recepientId,
                    AuthorId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var createdChat = await _chatRepo.AddAsync(newChat);
                return createdChat;
            }
            else
            {
                chat.UpdatedAt = DateTime.Now;
                await _chatRepo.UpdateAsync(chat);
                return chat;
            }
        }



        /// <summary>
        /// Получает сообщения в чате при загрузке
        /// </summary>
        /// <param name="message"></param>
        /// <param name="guId"></param>
        /// <returns></returns>
        [Authorize]
        [HubMethodName("GetAllMessagesOnConnect")]
        // TODO: забирать данные сообщения по guid чата        
        // чат - это штука которая создается сначала при старте сообщения между пользаками
        // связь между чатом и сообщениями - один ко многим - реализовать
        // при загрузке чата возвразается чат и все сообщения к нему
        public async Task<object> GetAllMessagesOnConnect(int recepientId)
        {
            // проверяем есть ли чат с данным пользователем
            // если первое сообщение создаем чат

            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;
            if (user is null)
                return Task.FromResult<object>(401);

            user.WasOnline = DateTime.Now;
            await _userManager.UpdateAsync(user);


            var spec = new MessageSpecification(user.Id, recepientId);
            var messages = await _messageRepo.ListAsync(spec);

            // отмечаем прочитанные сообщения
            // await ReadMessagessForRecepientId(user.Id, messages);

            var mappedMessages = _mapper.Map<IReadOnlyList<Message>, IReadOnlyList<MessageDto>>(messages);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var messageToReturn = JsonSerializer.Serialize<IReadOnlyList<MessageDto>>(mappedMessages, serializeOptions);
            return Clients.All.SendAsync("ReceiveAllMessageWhenStart", messageToReturn);

        }





        /// <summary>
        /// При получении всех сообщений для пользователя делаем проверку
        /// Если сообщение адресовано пользователю, то отмечаем его как прочтенным
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        private async Task ReadMessagessForRecepientId(int userId, IReadOnlyList<Message> messages)
        {


            foreach (var message in messages)
            {
                // перебираем все сообщения отданные на сервер
                // если автор сообщения и получатель - одно и тоже лицо - значит сообщение прочитано
                if (message.RecepientId == userId)
                {
                    _logger.LogCritical("на проблему напали");
                    _logger.LogCritical("на проблему напали");
                    _logger.LogCritical("на проблему напали");
                    _logger.LogCritical("на проблему напали");
                    _logger.LogCritical("на проблему напали");
                    message.IsReaded = true;
                    await _messageRepo.UpdateAsync(message);
                }
            }
        }




        /// <summary>
        /// Получает список пользователей онлайн
        /// </summary>
        /// <returns></returns>
        /// TODO:   объединить /// 
        [Authorize]
        [HubMethodName("GetConnectionItems")]
        public async Task<object> GetconnectionItems()
        {
            var mappedConnectionItems = _mapper.Map<List<ConnectionItem>, List<ConnectionItemDto>>(connectionItems);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var res = JsonSerializer.Serialize<List<ConnectionItemDto>>(mappedConnectionItems, serializeOptions);
            return Clients.Caller.SendAsync("ReceiveconnectionItems", res);
        }



        [Authorize]
        [HubMethodName("SendPrintingUserItem")]
        public async Task SendPrintingUserItem()
        {
            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;
            if (printingUsersItems.Where(z => z.UserId == user.Id).ToArray().Length == 0)
            {
                var printingUsersItem = new ConnectionItem()
                {
                    UserId = user.Id
                };
                printingUsersItems.Add(printingUsersItem);
            }


            Thread.Sleep(500);
            var itemToRemove = printingUsersItems.Where(z => z.UserId == user.Id).FirstOrDefault();
            if (itemToRemove is not null)
                printingUsersItems.Remove(itemToRemove);

        }





        // <ThrowHubException>
        public Task ThrowException()
        {
            throw new HubException("This error will be sent to the client!");
        }


        /// <summary>
        /// При подключении
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {

            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;
            if (connectionItems.Where(z => z.UserId == user.Id).ToArray().Length == 0)
            {
                var connectionItem = new ConnectionItem()
                {
                    UserId = user.Id
                };
                connectionItems.Add(connectionItem);
            }

            await base.OnConnectedAsync();
        }




        /// <summary>
        /// Когда пользак отсоединяется
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var user = _userManager.FindByClaimsCurrentUser(Context.User).Result;
            var itemToRemove = connectionItems.Where(z => z.UserId == user.Id).FirstOrDefault();
            if (itemToRemove is not null)
                connectionItems.Remove(itemToRemove);

            await base.OnDisconnectedAsync(exception);

        }

    }
}