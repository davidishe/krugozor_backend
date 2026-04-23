using Core.Helpers;
using Krugozor.Core.Models;

namespace Krugozor.Infrastructure.Specifications
{
  public class RequestSpecification : BaseSpecification<Request>
  {
    public RequestSpecification(UserParams userParams)
    : base(x =>
          string.IsNullOrEmpty(userParams.Search)
        )
    {
      AddInclude(x => x.Status);

      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      AddOrderByDescending(x => x.CreatedAt);

      if (!string.IsNullOrEmpty(userParams.sort))
      {
        switch (userParams.sort)
        {
          case "name":
            AddOrderByAscending(s => s.CreatedAt);
            break;
          default:
            AddOrderByAscending(x => x.CreatedAt);
            break;
        }
      }
    }


    public RequestSpecification()
    : base()
    {
      AddInclude(x => x.Status);
    }

    public RequestSpecification(int id) : base(x => x.Id == id)
    {
      AddInclude(x => x.Status);
    }

    public RequestSpecification(int userId, bool param) : base(x => x.InvestorUserId == userId)
    {
      AddInclude(x => x.Status);
    }




  }


}