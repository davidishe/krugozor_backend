using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Krugozor.Core.Models.Messages;
using Krugozor.Infrastructure.Database;
using Krugozor.Infrastructure.Messenger.Hubs;
using WebAPI.Controllers;
using Krugozor.Identity.Extensions;
using Krugozor.Infrastructure.Specifications;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading;

namespace Krugozor.WebAPI.Controllers.Messenger
{

    // https://engineering3962.rssing.com/chan-58580869/article74.html

    [Authorize]
    public class FileUploadController : BaseApiController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private IGenericRepository<Message> _messegeRepo;
        private readonly IHubContext<ChatHub> _hubContext;
        private IGenericRepository<Chat> _chatRepo;
        private IGenericRepository<Image> _imageRepo;



        public FileUploadController(
            UserManager<AppUser> userManager,
            IGenericRepository<Message> messegeRepo,
            IGenericRepository<Image> imageRepo,
            IGenericRepository<Chat> chatRepo,
            IHubContext<ChatHub> hubContext,
            IMapper mapper)
        {
            _mapper = mapper;
            _imageRepo = imageRepo;
            _userManager = userManager;
            _messegeRepo = messegeRepo;
            _hubContext = hubContext;
            _chatRepo = chatRepo;

        }


        // [Route("files/message/{recepientId}")]
        // [HttpPost]
        // public async Task<IActionResult> CreateMessageForFiles([FromRoute] int recepientId, [FromQuery] string text)
        // {
        //     var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
        //     var chat = await GetChatForMessage(recepientId, user.Id);

        //     var newMessage = new Message()
        //     {
        //         Text = text,
        //         GuId = Guid.NewGuid().ToString(),
        //         AuthorId = user.Id,
        //         RecepientId = recepientId,
        //         CreatedAt = DateTime.Now,
        //         IsReaded = false,
        //         ChatId = chat.Id,
        //         WithImages = true
        //     };
        //     var message = await _messegeRepo.AddAsync(newMessage);
        //     return Ok(message);
        // }

        [Route("files/message/{recepientId}")]
        [HttpPost]
        public async Task<IActionResult> CreateMessageForFiles([FromRoute] int recepientId, [FromQuery] string? text)
        {
            var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
            var chat = await GetChatForMessage(recepientId, user.Id);

            if (text is null)
                text = " ";

            var newMessage = new Message()
            {
                Text = text,
                GuId = Guid.NewGuid().ToString(),
                AuthorId = user.Id,
                RecepientId = recepientId,
                CreatedAt = DateTime.Now,
                IsReaded = false,
                ChatId = chat.Id,
                WithImages = true
            };
            var message = await _messegeRepo.AddAsync(newMessage);
            return Ok(message);
        }



        /// <summary>
        /// Создаем сообщение для добавления вложенных файлов
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [Route("files/{messageId}")]
        [HttpPost]
        public async Task<IActionResult> UploadFiles([FromRoute] int messageId)
        {

            Thread.Sleep(7000);
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);

                        var docByte = memoryStream.ToArray();
                        var newImage = new Image()
                        {
                            DocByte = docByte,
                            FileType = file.ContentType,
                            FileName = file.FileName,
                            MessageId = messageId,
                            Size = docByte.Length
                        };
                        await _imageRepo.AddAsync(newImage);
                    }
                }
            }

            var spec = new MessageSpecification(messageId);
            var message = await _messegeRepo.GetEntityWithSpec(spec);
            var mappedMessage = _mapper.Map<Message, MessageDto>(message);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var messageToReturn = JsonSerializer.Serialize<MessageDto>(mappedMessage, serializeOptions);
            // await _hubContext.Clients.All.SendAsync("ReceiveNewMessage", messageToReturn);
            return Ok(messageToReturn);
        }



        [Route("files/{messageId}")]
        [HttpGet]
        public async Task<IActionResult> GetImagesWithMessage([FromRoute] int messageId)
        {
            // var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
            var spec = new ImageSpecification(messageId);
            var images = await _imageRepo.ListAsync(spec);
            return Ok(images);
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


    }
}