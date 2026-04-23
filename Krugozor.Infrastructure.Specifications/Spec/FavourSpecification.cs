using Core.Helpers;
using Krugozor.Core.Models;

namespace Krugozor.Infrastructure.Specifications
{
  public class FavourSpecification : BaseSpecification<Favour>
  {
    public FavourSpecification(UserParams userParams)
    : base(x =>
          (string.IsNullOrEmpty(userParams.Search)) &&
          (!userParams.typeId.HasValue)
        )
    {
      AddInclude(x => x.ProposalProfile);
      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      AddOrderByDescending(x => x.CreatedAt);

    }


    public FavourSpecification()
    : base()
    {
      AddInclude(x => x.ProposalProfile);
    }

    // x => x.AppUserId == userId
    public FavourSpecification(int userId) : base(x => x.AppUserId == userId)
    {
      AddInclude(x => x.ProposalProfile);
    }




  }


}

