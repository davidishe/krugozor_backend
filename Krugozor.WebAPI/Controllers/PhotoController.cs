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
using Krugozor.Identity.Extensions;

namespace WebAPI.Controllers
{



  [AllowAnonymous]
  public class PhotoController : BaseApiController
  {


    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;


    public PhotoController(
      UserManager<AppUser> userManager,
      IMapper mapper)
    {
      _mapper = mapper;
      _userManager = userManager;
    }



    #region 3. Products CRUD functionality
    /* create, delete, update products */




    [Authorize]
    [HttpPost]
    [Route("user")]
    public async Task<ActionResult<UserToReturnDto>> AddPhotoUser()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      IFormFile file = Request.Form.Files[0];


      var deletePreviousFile = await DeleteFileFromServer(user.PictureUrl, "Users");
      if (!deletePreviousFile)
        return BadRequest("Что-то пошло не так при удалении файла");

      var pictureFileName = await SaveFileToServer(file, "Users");

      user.PictureUrl = pictureFileName;
      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
        return BadRequest("Ошибка при обновлении пользователя");

      user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return BadRequest("Такой пользователь не найден...");

      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);
      return Ok(userToReturn);
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

    private Task<bool> DeleteFileFromServer(string pictureFileName, string subPath)
    {

      if (pictureFileName is null)
        return Task.FromResult(true);

      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Images", subPath, pictureFileName);
      if (System.IO.File.Exists(fullPath))
      {
        System.IO.File.Delete(fullPath);
      }

      return Task.FromResult(true);

    }



    #endregion

  }
}