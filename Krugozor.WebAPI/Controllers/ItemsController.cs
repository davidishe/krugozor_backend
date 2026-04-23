using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using Krugozor.Core.Models;
using Krugozor.Identity.Extensions;
using Krugozor.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Controllers;
using Krugozor.Infrastructure.Specifications;
using System.Collections.Generic;
using Core.Models;
using System.IO;
using Newtonsoft.Json;

namespace Krugozor.WebAPI.Controllers
{


  [Authorize]
  public class ItemsController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<ProposalProfile> _profileManager;
    private readonly IGenericRepository<ProposalProfile> _proposalRepo;
    private readonly IGenericRepository<Favour> _favourRepo;
    private readonly IDbRepository<Request> _requestManager;
    private readonly IDbRepository<RequestStatus> _requestStatusRepo;
    private readonly IDbRepository<ProposalProfileStatus> _proposalStatusRepo;
    private readonly IMapper _mapper;
    private readonly IDbRepository<Favour> _favourDbRepo;


    private static List<Item> items = new List<Item>()
    {
      new Item()
      {
        Id = 1,
        Name = "Алексей Савватеев",
        Description = "Российский математик и специалист в математической экономике, популяризатор математики, видеоблогер. Доктор физико-математических наук, член-корреспондент РАН. Профессор МФТИ, профессор Адыгейского государственного университета, ведущий научный сотрудник.",
        CreatedAt = DateTime.Now.AddDays(-33),
        UpdatedAt = DateTime.Now.AddDays(-4),
        ImageProfile = "https://du4m3vcuyb.cloudcdn.info/wp-content/uploads/2021/07/savvateev-a.v.-2.jpg",
        Rating = 89,
        BornedYear = DateTime.Now.AddYears(-49)
      },
      new Item()
      {
        Id = 4,
        Name = "Оскар Хартманн",
        Description = "Предприниматель и меценат",
        CreatedAt = DateTime.Now.AddDays(-40),
        UpdatedAt = DateTime.Now.AddDays(-44),
        TgNicknameLink = "Oskar_Hartmann",
        YouTubeLink = "https://www.youtube.com/@oskar_hartmann1",
        ImageProfile = "https://cdn.forbes.ru/files/c/232x231/profile/uku.jpg__1495049450__65494.webp",
        Rating = 74,
        BornedYear = DateTime.Now.AddYears(-37)
      },
      new Item()
      {
        Id = 3,
        Name = "Евгений Панасенков",
        Description = "Историк и писатель",
        CreatedAt = DateTime.Now.AddDays(-42),
        UpdatedAt = DateTime.Now.AddDays(-13),
        YouTubeLink = "https://www.youtube.com/@adept_maestro",
        ImageProfile = "https://2ch.life/fag/thumb/5215545/15269321322281s.jpg",
        Rating = 44,
        BornedYear = DateTime.Now.AddYears(-37)
      },
      new Item()
      {
        Id = 5,
        Name = "Инна Петрова",
        Description = "Супер исследоватлеь",
        CreatedAt = DateTime.Now.AddDays(-40),
        UpdatedAt = DateTime.Now.AddDays(-13),
        ImageProfile = "https://images.unsplash.com/photo-1500917293891-ef795e70e1f6?ixlib=rb-=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=8&w=1024&h=1024&q=80",
        Rating = 94,
        BornedYear = DateTime.Now.AddYears(-44)
      }

    };

    //TODO: задеплоить - может быть в докере - или сделать нормальные пайплайны чтобы из переменных папки для простых сервисов публиковались


    public ItemsController(
      UserManager<AppUser> userManager,
      IDbRepository<ProposalProfile> profileManager,
      IDbRepository<Request> requestManager,
      IDbRepository<RequestStatus> requestStatusRepo,
      IGenericRepository<ProposalProfile> proposalRepo,
      IGenericRepository<Favour> favourRepo,
      IDbRepository<ProposalProfileStatus> proposalStatusRepo,
      IDbRepository<Favour> favourDbRepo,
      IMapper mapper)
    {
      _profileManager = profileManager;
      _mapper = mapper;
      _proposalRepo = proposalRepo;
      _requestManager = requestManager;
      _userManager = userManager;
      _requestStatusRepo = requestStatusRepo;
      _proposalStatusRepo = proposalStatusRepo;
      _favourDbRepo = favourDbRepo;
      _favourRepo = favourRepo;
    }

    private void WriteDebugLog(string runId, string hypothesisId, string location, string message, object data)
    {
      try
      {
        var payload = new
        {
          sessionId = "ea380a",
          runId,
          hypothesisId,
          location,
          message,
          data,
          timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        File.AppendAllText("/Users/akobiyada/Documents/SpecialProjects/01_Focus_for_developing/58_DAVINCI/.cursor/debug-ea380a.log", JsonConvert.SerializeObject(payload) + Environment.NewLine);
      }
      catch
      {
        // ignore debug logging failures
      }
    }



    /// <summary>
    /// Возвращает список всех инфлюенсеров
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult> GetItems()
    {
      #region agent log
      WriteDebugLog("pre-fix", "H1", "ItemsController.cs:GetItems", "returning hardcoded items without strapi", new { count = items.Count });
      #endregion
      return Ok(items);
    }



    [Authorize]
    [HttpGet]
    [Route("get_by_id/{id}")]
    public async Task<ActionResult> GetItemById([FromRoute] int Id)
    {
      var item = items.Where(x => x.Id == Id).FirstOrDefault();
      #region agent log
      WriteDebugLog("pre-fix", "H2", "ItemsController.cs:GetItemById", "returning hardcoded item without strapi", new { id = Id, found = item is not null });
      #endregion
      return Ok(item);
    }



    [Authorize]
    [HttpPost]
    [Route("favour/add/{strapiProposalId}")]
    public async Task<ActionResult<Favour>> AddUserFavour([FromRoute] int strapiProposalId)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return BadRequest("Такой пользователь не найден");
      return Ok(200);
    }


    [Authorize]
    [HttpPost]
    [Route("favour/remove/{strapiProposalId}")]
    public async Task<ActionResult<Favour>> RemoveUserFavour([FromRoute] int strapiProposalId)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return BadRequest("Такой пользователь не найден");

      var spec = new FavourSpecification(user.Id);
      var favour = await _favourRepo.GetEntityWithSpec(spec);

      if (favour is null)
        return NotFound("Такой записи не найдено");

      await _favourDbRepo.DeleteAsync(favour);
      return Ok(favour);
    }


    [Authorize]
    [HttpGet]
    [Route("favour/check")]
    public async Task<ActionResult<Favour>> CheckUserFavour()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return BadRequest("Такой пользователь не найден");

      var spec = new FavourSpecification(user.Id);
      var favour = await _favourRepo.ListAsync(spec);

      if (favour is null)
        return NotFound("Такой записи не найдено");

      return Ok(favour);
    }




  }
}