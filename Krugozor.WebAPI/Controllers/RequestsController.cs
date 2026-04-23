using Krugozor.Identity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Krugozor.Core.Models;
using Krugozor.Infrastructure.Database;
using Krugozor.Infrastructure.Specifications;
using Krugozor.Infrastructure.Strapi;
using WebAPI.Controllers;

namespace Krugozor.WebAPI.Controllers
{

  [Authorize]
  public class RequestsController : BaseApiController
  {

    private readonly IGenericRepository<Request> _requestsGenericRepo;
    private readonly IGenericRepository<ProposalProfile> _profilesRepo;
    private readonly ILogger<RequestsController> _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<RequestStatus> _requstStatusRepo;
    private readonly IDbRepository<ProposalProfileStatus> _proposalStatusRepo;
    private readonly IDbRepository<Request> _requesetsRepo;
    private readonly IStrapiService _strapiService;



    public RequestsController(
      IMapper mapper,
      IGenericRepository<Request> requestsGenericRepo,
      IGenericRepository<ProposalProfile> profileRepo,
      IDbRepository<RequestStatus> requstStatusRepo,
      IDbRepository<Request> requesetsRepo,
      UserManager<AppUser> userManager,
      IDbRepository<ProposalProfileStatus> proposalStatusRepo,
      IStrapiService strapiService,
    ILogger<RequestsController> logger)
    {
      _requestsGenericRepo = requestsGenericRepo;
      _profilesRepo = profileRepo;
      _requstStatusRepo = requstStatusRepo;
      _requesetsRepo = requesetsRepo;
      _proposalStatusRepo = proposalStatusRepo;
      _logger = logger;
      _mapper = mapper;
      _strapiService = strapiService;
      _userManager = userManager;
    }


    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllAsync([FromQuery] int proposalId)
    {
      var spec = new RequestSpecification();
      Thread.Sleep(1000);
      var entitys = await _requestsGenericRepo.ListAsync(spec);
      var initiativesToReturn = _mapper.Map<IReadOnlyList<Request>, IReadOnlyList<RequestDto>>(entitys).Where(z => z.ProposalProfileId == proposalId);
      return Ok(initiativesToReturn);
    }






    /// <summary>
    /// Возвращает лимиты
    /// </summary>
    /// <param name="strapiProposalId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("limits/{strapiProposalId}")]
    public async Task<ActionResult<ProposalLimits>> GetProposalProfileLimits([FromRoute] int strapiProposalId)
    {

      var spec = new ProposalProfileSpecification(strapiProposalId: strapiProposalId);
      var entity = await _strapiService.GetProposalEntity(strapiProposalId);
      var proposalProfile = await _profilesRepo.GetEntityWithSpec(spec);
      var minShareValue = 10;

      if (proposalProfile is null)
      {
        var result = new ProposalLimits()
        {
          Max = (double)entity.Attributes.Price / 2,
          Min = (double)entity.Attributes.Price / minShareValue
        };
        return Ok(result);
      }

      double currentPledgesAmount = 0.00;
      foreach (var request in proposalProfile.Requests)
      {
        currentPledgesAmount += request.ShareValue;
      }

      var remainingAmount = proposalProfile.OveralInvestSum - currentPledgesAmount;
      var proposalLimits = new ProposalLimits()
      {
        Max = (double)remainingAmount,
        Min = (remainingAmount >= proposalProfile.OveralInvestSum / minShareValue) ? (proposalProfile.OveralInvestSum / minShareValue) : remainingAmount
      };

      return Ok(proposalLimits);
    }





    /// <summary>
    /// Метод для изменения статуса только заявки, без предложения и всех связанных заявок
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("change/request")]
    public async Task<ActionResult<OrderToReturnDto>> ChangeOrderStatusById([FromQuery] int orderId, [FromQuery] int statusId)
    {


      var status = await _requstStatusRepo.GetByIdAsync(statusId);
      if (status is null)
        return BadRequest($"Не найден такой status c id: {statusId}");


      var request = await _requestsGenericRepo.GetByIdAsync(orderId);
      if (request is null)
        return BadRequest("Не найден такой request");


      // TODO: выделить в приватный метод
      var strapiProposalId = request.StrapiProposalNumber;
      // берем все связанные запросы по данному предложению
      // проводим обновление всех запросов, если набрано максимальное количество участников

      var proposalSpec = new ProposalProfileSpecification(strapiProposalId);
      var proposal = await _profilesRepo.GetEntityWithSpec(proposalSpec);
      if (proposal is null)
        return BadRequest($"Не найден такой proposal c id: {strapiProposalId}");

      if (proposal.Requests.Count == 0)
        return BadRequest($"Количество значений 0");

      // var otherOrdersStatus = await _requstStatusRepo.GetByIdAsync(statusId);
      // параметр, который передаем в поле с общим пулом вложений для proposal
      // он означает общую сумму запросов по предложению
      double curentShareValues = 0;
      foreach (var req in proposal.Requests.Where(x => x.RequestStatusId != 5))
      {
        curentShareValues += req.ShareValue;
      }

      var requstToUpdateSpec = new RequestSpecification(orderId);
      var requstToUpdate = await _requestsGenericRepo.GetEntityWithSpec(requstToUpdateSpec);
      requstToUpdate.RequestStatusId = statusId;
      requstToUpdate.Status = status;
      await _requesetsRepo.UpdateAsync(requstToUpdate);
      var entityToReturn = _mapper.Map<Request, RequestDto>(requstToUpdate);
      return Ok(entityToReturn);
    }




    /// <summary>
    /// Просто берет заказ по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("getbyid")]
    public async Task<ActionResult<OrderToReturnDto>> GetById([FromQuery] int id)
    {
      var spec = new RequestSpecification(id);
      var entity = await _requestsGenericRepo.GetEntityWithSpec(spec);
      var entityToReturn = _mapper.Map<Request, RequestDto>(entity);
      return Ok(entityToReturn);
    }


    /// <summary>
    /// Получаем все запросы для конкртеного пользователя
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("user")]
    public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetRequestsForUser()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      var spec = new RequestSpecification(user.Id, true);
      var requests = await _requestsGenericRepo.ListAsync(spec);
      var entityToReturn = _mapper.Map<IReadOnlyList<Request>, IReadOnlyList<RequestDto>>(requests);
      return Ok(entityToReturn);
    }




    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult<IReadOnlyList<ProposalProfile>>> Delete([FromQuery] int id)
    {
      var entity = await _requestsGenericRepo.GetByIdAsync(id);
      _requestsGenericRepo.Delete(entity);
      return Ok(entity);
    }


  }
}