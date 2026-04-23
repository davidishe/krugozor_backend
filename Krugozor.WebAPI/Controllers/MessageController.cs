using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Krugozor.Core.Models.Messages;
using Krugozor.Infrastructure.Database;
using WebAPI.Controllers;

namespace Krugozor.WebAPI.Controllers;


[Authorize]
public class MessageController : BaseApiController
{

  private readonly UserManager<AppUser> _userManager;
  private readonly IMapper _mapper;
  private IGenericRepository<Correspondent> _correspondentRepo;


  public MessageController(
    UserManager<AppUser> userManager,
    IGenericRepository<Correspondent> correspondentRepo,
    IMapper mapper)
  {
    _mapper = mapper;
    _userManager = userManager;
    _correspondentRepo = correspondentRepo;
  }



  [HttpGet]
  [Route("correspondents")]
  public async Task<ActionResult<IReadOnlyList<UserToReturnDto>>> GetAllAsync([FromQuery] int userId)
  {

    // var spec = new CorrespondentSpecification(user.Id, recepientId);
    // var correspondents = await _correspondentRepo.GetEntityWithSpec(spec);

    // var entitys = await _requestsGenericRepo.ListAsync(spec);
    // var initiativesToReturn = _mapper.Map<IReadOnlyList<Request>, IReadOnlyList<RequestDto>>(entitys).Where(z => z.ProposalProfileId == proposalId);
    // return Ok(initiativesToReturn);
    return Ok(200);
  }


}




////////
///

//
