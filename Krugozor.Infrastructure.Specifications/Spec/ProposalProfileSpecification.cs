using System.Linq;
using Core.Helpers;
using Krugozor.Core.Models;

namespace Krugozor.Infrastructure.Specifications
{
  public class ProposalProfileSpecification : BaseSpecification<ProposalProfile>
  {
    public ProposalProfileSpecification(UserParams userParams)
    : base(x =>
          (string.IsNullOrEmpty(userParams.Search)) &&
          (!userParams.typeId.HasValue)
        )
    {
      AddInclude(x => x.Favours);
      AddInclude(z => z.Requests);


      ApplyPaging((userParams.PageSize * (userParams.PageIndex)), userParams.PageSize);
      AddOrderByDescending(x => x.CreatedAt);

      if (!string.IsNullOrEmpty(userParams.sort))
      {
        switch (userParams.sort)
        {
          case "ammount":
            AddOrderByAscending(s => s.CreatedAt);
            break;
          default:
            AddOrderByAscending(x => x.CreatedAt);
            break;
        }
      }
    }


    public ProposalProfileSpecification()
    : base()
    {
      AddInclude(x => x.Requests);
      AddInclude(z => z.Favours);
    }

    public ProposalProfileSpecification(int strapiProposalId) : base(x => x.StrapiProposalId == strapiProposalId)
    {
      AddInclude(x => x.Requests);
      AddInclude(z => z.Favours);
    }

    public ProposalProfileSpecification(int userId, bool fake) : base()
    {
      AddInclude(z => z.Favours.Where(z => z.AppUserId == userId));
    }

    public ProposalProfileSpecification(int strapiCompanyId, int fake) : base(x => x.StrapiCompanyId == strapiCompanyId)
    {
      AddInclude(x => x.Requests);
      AddInclude(z => z.Favours);
    }



  }


}


// public ProposalProfileSpecification(int strapiProposalId) : base(x => x.StrapiProposalId == strapiProposalId)
// {
//   AddInclude(x => x.Requests);
//   AddInclude(z => z.Offers);
// }