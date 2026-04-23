using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using Krugozor.Core.Models;
using Krugozor.Identity.Extensions;
using Krugozor.Infrastructure.Database;
using Krugozor.Infrastructure.Strapi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using Krugozor.Infrastructure.Strapi.Models;
using Newtonsoft.Json;
using Core.Models;

namespace Krugozor.WebAPI.Controllers
{


  [AllowAnonymous]
  public class StrapiController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<ProposalProfile> _profileManager;
    private readonly IDbRepository<Request> _requestManager;
    private readonly IDbRepository<Grade> _gradeManager;
    private readonly IDbRepository<ProposalProfileStatus> _proposalProfileRepo;

    private readonly IStrapiService _strapiService;

    private readonly IMapper _mapper;
    private ILogger<StrapiController> _logger;




    public StrapiController(
      UserManager<AppUser> userManager,
      IDbRepository<ProposalProfile> profileManager,
      IDbRepository<ProposalProfileStatus> proposalProfileRepo,
      IDbRepository<Request> requestManager,
      IDbRepository<Grade> gradeManager,
      IStrapiService strapiService,
      ILogger<StrapiController> logger,
      IMapper mapper)
    {
      _profileManager = profileManager;
      _proposalProfileRepo = proposalProfileRepo;
      _mapper = mapper;
      _requestManager = requestManager;
      _userManager = userManager;
      _gradeManager = gradeManager;
      _strapiService = strapiService;
      _logger = logger;
    }

    // 

    /// <summary>
    /// Добавляет статус драфта у предложения
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("proposals/proposal/draft/{strapiProposalId}")]
    public async Task<ActionResult> SetToDraft([FromRoute] int strapiProposalId)
    {
      await _strapiService.RemoveOrAddDraftStatusForProposal(strapiProposalId, true);
      return Ok(200);
    }

    /// <summary>
    /// Публикует предложение
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("proposals/proposal/publish/{strapiProposalId}")]
    public async Task<ActionResult> SetToPublish([FromRoute] int strapiProposalId)
    {
      await _strapiService.RemoveOrAddDraftStatusForProposal(strapiProposalId, false);
      return Ok(200);
    }


    /// <summary>
    /// Обновляет объект proposal
    /// </summary>
    /// <param name="strapiDto"></param>
    /// <returns></returns>
    [Authorize]
    [AllowAnonymous]
    [HttpPut]
    [Route("update/proposal")]
    public async Task<ActionResult> CreateOrUpdateProposalInStrapi([FromBody] StrapiProposalDto strapiDto)
    {

      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);


      // get current entity
      var data = await _strapiService.GetProposalEntity(strapiDto.StrapiProposalId);
      if (data is null)
      {
        var company = new StrapiFieldDto
        {
          Id = (int)user.StrapiCompanyId
        };

        // map in new object and update to strapi database
        var dataToCreate = new Data
        {
          Name = strapiDto.Name,
          Description = strapiDto.Description,
          Price = strapiDto.Price,
          Company = company,
          AuthorId = user.Id,
          IsPublished = false
        };
        var entity = await _strapiService.AddProposalEntity(dataToCreate);
        var jsonText = JsonConvert.DeserializeObject<StrapiProposalContractDto>(entity.Content);
        var strapiProposalId = jsonText.Data.Id;
        var z = await CreateProfileAsync(user, (int)strapiProposalId);
        return Ok(entity);
      }

      //
      // map in new object and update to strapi database
      var newData = new Data
      {
        Name = strapiDto.Name,
        Description = strapiDto.Description,
        IsUrgent = strapiDto.IsUrgent,
        Price = strapiDto.Price,
        Images = strapiDto.Images,
        Cities = strapiDto.Cities,
        ProposalTypes = strapiDto.ProposalTypes,
        Amenities = strapiDto.Amenities,
        Address = strapiDto.Address,
        PlaceArea = strapiDto.PlaceArea,
        BedsQuantity = strapiDto.BedsQuantity,
        BuildYear = strapiDto.BuildYear,
        ElectricityPower = strapiDto.ElectricityPower,
        IsRealBusinessEnable = strapiDto.IsRealBusinessEnable
      };


      try
      {
        var result = await _strapiService.UpdateProposalEntity(newData, strapiDto.StrapiProposalId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return Ok(ex);
      }

    }


    /// <summary>
    /// Создает профиль //TODO: вынести в сервис
    /// </summary>
    /// <param name="user"></param>
    /// <param name="strapiProposalNumber"></param>
    /// <returns></returns>
    private async Task<ProposalProfile> CreateProfileAsync(AppUser user, int strapiProposalNumber)
    {
      var strapiEntity = await _strapiService.GetProposalEntity(strapiProposalNumber);
      var status = await _proposalProfileRepo.GetByIdAsync(1);
      var newProfile = new ProposalProfile
      {
        CreatedAt = DateTime.Now,
        StrapiCompanyId = user.StrapiCompanyId,
        StrapiProposalId = strapiProposalNumber,
        ProposalProfileStatusId = 1,
        Status = status,
        OveralInvestSum = 0
      };
      var profile = await _profileManager.AddAsync(newProfile);
      return profile;
    }



    /// <summary>
    /// Update company in strapi. Not needed to create - already created when user is created
    /// </summary>
    /// <param name="strapiDto"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut]
    [Route("update/company")]
    public async Task<ActionResult> UpdateCompanyInStrapi([FromBody] StrapiProposalDto strapiDto)
    {

      // TODO: поправить безопасность
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      // get current entity
      var data = await _strapiService.GetCompanyEntity(strapiDto.StrapiProposalId);
      if (data is null)
        return BadRequest("Не найден такой объект");

      // map in new object and update to strapi database
      var updatedData = new Data
      {
        Name = strapiDto.Name,
        Description = strapiDto.Description,
        Cities = strapiDto.Cities,
        Website = strapiDto.Website,
        Phone = strapiDto.Phone,
        Address = strapiDto.Address,
        Email = strapiDto.Email,
        InstagramCompanyName = strapiDto.InstagramCompanyName,
        TelegramCompanyName = strapiDto.TelegramCompanyName,
        FacebookCompanyName = strapiDto.FacebookCompanyName,
        IsTechnical = false,
        Images = strapiDto.Images
      };

      try
      {
        var result = await _strapiService.UpdateCompanyEntity(updatedData, strapiDto.StrapiProposalId);
        user.IsAgency = true;
        await _userManager.UpdateAsync(user);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return Ok(ex);
      }

    }



    [Authorize]
    [HttpPost]
    [Route("add/image")]
    public async Task<ActionResult> AddPhotoUser()
    {
      // var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      IFormFile file = Request.Form.Files[0];

      // var deletePreviousFile = await DeleteFileFromServer(user.PictureUrl, "Users");
      // if (!deletePreviousFile)
      //   return BadRequest("Что-то пошло не так при удалении файла");

      var fileName = await SaveFileToServer(file, "Users");
      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Images", "Users", fileName);
      var result = await _strapiService.AddImageToMediaLibrary(fullPath);
      return Ok(result);
    }





    private decimal CalculateStroreRating(ICollection<Grade> grades)
    {

      if (grades.ToArray().Length == 1)
        return grades.ToArray()[0].Rate;

      List<decimal> one = new();
      List<decimal> two = new();
      List<decimal> tree = new();
      List<decimal> four = new();
      List<decimal> five = new();


      foreach (var item in grades)
      {
        if (item.Rate == 1)
          one.Add(item.Rate);

        if (item.Rate == 2)
          two.Add(item.Rate);

        if (item.Rate == 3)
          tree.Add(item.Rate);

        if (item.Rate == 4)
          four.Add(item.Rate);

        if (item.Rate == 5)
          five.Add(item.Rate);
      }

      decimal share = ((decimal)one.Count / (decimal)grades.Count) * 1 +
                    ((decimal)two.Count / (decimal)grades.Count) * 2 +
                    ((decimal)tree.Count / (decimal)grades.Count) * 3 +
                    ((decimal)four.Count / (decimal)grades.Count) * 4 +
                    ((decimal)five.Count / (decimal)grades.Count) * 5;

      // double result =  / grades.Count;
      // decimal share = (18m / 58m) * 100m;
      // decimal share = (decimal)five.Count / (decimal)grades.Count;
      return share;

    }




    private Task<string> SaveFileToServer(IFormFile file, string subPath)
    {
      var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Images", subPath, fileName);

      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        file.CopyTo(stream);
      }

      return Task.FromResult(fileName);

    }

    private Task<bool> DeleteFileFromServer(string fileName, string subPath)
    {

      if (fileName is null)
        return Task.FromResult(true);

      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Images", subPath, fileName);
      if (System.IO.File.Exists(fullPath))
      {
        System.IO.File.Delete(fullPath);
      }

      return Task.FromResult(true);

    }





  }
}