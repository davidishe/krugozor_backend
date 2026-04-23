using Core.Helpers;
using Krugozor.Core.Models.Messages;

namespace Krugozor.Infrastructure.Specifications
{
  public class ChatSpecification : BaseSpecification<Chat>
  {
    public ChatSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      AddInclude(x => x.Messages);
      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      AddOrderByDescending(x => x.UpdatedAt);

      if (!string.IsNullOrEmpty(userParams.sort))
      {
        switch (userParams.sort)
        {
          case "name":
            // AddOrderByAscending(s => s.CreatedAt);
            break;
          default:
            // AddOrderByAscending(x => x.CreatedAt);
            break;
        }
      }
    }


    public ChatSpecification()
    : base()
    {
      AddInclude(x => x.Messages);
      // AddInclude(x => x.Status);
    }



    /// <summary>
    /// возаращает все сообщения для конкрнетного пользователя
    /// </summary>
    /// <param name="recepientId"></param>
    /// <returns></returns>
    public ChatSpecification(int recepientId) : base(x =>
                    (x.RecepientId == recepientId) ||
                    (x.AuthorId == recepientId))
    {
      AddInclude(x => x.Messages);
      AddOrderByDescending(x => x.UpdatedAt);
    }



    /// <summary>
    /// возаращает все сообщения для конкрнетного пользователя
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public ChatSpecification(int chatId, bool param) : base(x => x.Id == chatId)
    {
      AddInclude(x => x.Messages);
      AddOrderByDescending(x => x.UpdatedAt);
    }




    /// <summary>
    /// возвращает все чаты где пользователь является контрагентом с данным recepientId - то есть конкретный чат
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="recepientId"></param>
    /// <returns></returns>
    public ChatSpecification(int userId, int recepientId) : base(x =>
            (x.RecepientId == userId && x.AuthorId == recepientId) ||
            (x.AuthorId == userId && x.RecepientId == recepientId))
    {
      AddInclude(x => x.Messages);
      AddOrderByAscending(x => x.CreatedAt);
    }




  }


}