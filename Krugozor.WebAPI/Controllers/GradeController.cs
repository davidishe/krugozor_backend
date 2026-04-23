using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using Krugozor.Core.Models;
using Krugozor.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Controllers;

namespace Krugozor.WebAPI.Controllers
{


  [Authorize]
  public class GradeController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<ProposalProfile> _profileManager;
    private readonly IDbRepository<Request> _requestManager;
    private readonly IDbRepository<Grade> _gradeManager;
    private readonly IMapper _mapper;


    public GradeController(
      UserManager<AppUser> userManager,
      IDbRepository<ProposalProfile> profileManager,
      IDbRepository<Request> requestManager,
      IDbRepository<Grade> gradeManager,
      IMapper mapper)
    {
      _profileManager = profileManager;
      _mapper = mapper;
      _requestManager = requestManager;
      _gradeManager = gradeManager;
      _userManager = userManager;
    }



    /// <summary>
    /// Получаем все запросы на инвестирование и оценки к конкретному предложению по strapiProposal id
    /// </summary>
    /// <param name="strapiProposalId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult> GetRequests([FromQuery] int strapiProposalId)
    {

      var result = _profileManager.GetAll().Where(x => x.StrapiProposalId == strapiProposalId).Include(x => x.Requests).Include(x => x.Grades);

      if (result is null)
        return NotFound("Ничего не найдено");

      return Ok(result);
    }




  }
}