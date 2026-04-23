using Core.Helpers;
using Krugozor.Core.Models.Messages;

namespace Krugozor.Infrastructure.Specifications
{
  public class ImageSpecification : BaseSpecification<Image>
  {
    public ImageSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      // AddOrderByDescending(x => x.UpdatedAt);

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


    public ImageSpecification()
    : base()
    {
      // AddInclude(x => x.Messages);
      // AddInclude(x => x.Status);
    }




    public ImageSpecification(int messageId) : base(x => x.MessageId == messageId)
    {
      // AddInclude(x => x.Messages);
      // AddOrderByDescending(x => x.UpdatedAt);
    }




  }


}