using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;
using Microsoft.AspNetCore.Identity;
using Core.Identity;
using Krugozor.Infrastructure.Database;
using Krugozor.Core.Models;
using Krugozor.Infrastructure.Specifications;

namespace WebAPI.Controllers
{



  [AllowAnonymous]
  public class FilesController : BaseApiController
  {


    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IGenericRepository<ProposalProfile> _proposalProfileRepo;


    public FilesController(
      UserManager<AppUser> userManager,
      IGenericRepository<ProposalProfile> proposalProfileRepo,
      IMapper mapper)
    {
      _mapper = mapper;
      _userManager = userManager;
      _proposalProfileRepo = proposalProfileRepo;
    }



    [Authorize]
    [HttpPost]
    [Route("invest_passport/{strapiProposalId}")]
    public async Task<ActionResult<UserToReturnDto>> AddInvestPassport([FromRoute] int strapiProposalId)
    {
      var spec = new ProposalProfileSpecification(strapiProposalId);
      var profile = await _proposalProfileRepo.GetEntityWithSpec(spec);
      var file = Request.Form.Files[0];
      var deletePreviousFile = await DeleteFileFromServer(profile.InvestPassportLink, "Files", "Passports");
      if (!deletePreviousFile)
        return BadRequest("Что-то пошло не так при удалении файла");

      var fileName = await SaveFileToServer(file, "Files", "Passports");

      profile.InvestPassportLink = fileName;
      await _proposalProfileRepo.UpdateAsync(profile);

      var newProfile = await _proposalProfileRepo.GetEntityWithSpec(spec);
      var result = _mapper.Map<ProposalProfile, ProposalProfileDto>(newProfile);
      return Ok(result);
    }


    private Task<string> SaveFileToServer(IFormFile file, string path, string subPath)
    {
      var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", path, subPath, fileName);

      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        file.CopyTo(stream);
      }

      return Task.FromResult(fileName);

    }

    private Task<bool> DeleteFileFromServer(string fileName, string path, string subPath)
    {

      if (fileName is null)
        return Task.FromResult(true);

      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", path, subPath, fileName);
      if (System.IO.File.Exists(fullPath))
      {
        System.IO.File.Delete(fullPath);
      }

      return Task.FromResult(true);

    }



  }
}