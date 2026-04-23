using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;
using Core.Identity;
using Krugozor.Core.Responses;
using Krugozor.Identity;
using Krugozor.Identity.Extensions;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Krugozor.WebAPI.Modules;
using System.Web;
using System;
using Microsoft.Extensions.Configuration;
using Krugozor.Infrastructure.Strapi;
using Newtonsoft.Json;
using Krugozor.Core.Models;


namespace WebAPI.Controllers.Auth
{

  [Authorize]
  public class AccountController : BaseApiController
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<AccountController> _logger;
    private readonly IStrapiService _strapiService;
    private readonly string _emailDomainMailJetSender;
    private readonly string _baseUrl;

    public AccountController(
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager,
      ITokenService tokenService,
      IConfiguration config,
      IMapper mapper,
      IStrapiService strapiService
      )
    {
      _signInManager = signInManager;
      _tokenService = tokenService;
      _mapper = mapper;
      _userManager = userManager;
      _strapiService = strapiService;
      _emailDomainMailJetSender = config.GetSection("AppSettings:EmailDomain").Value;
      _baseUrl = config.GetSection("AppSettings:FrontendUrl").Value;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("health_check")]
    public async Task<ActionResult> HealthCheck()
    {
      return Ok("Все хорошо");
    }




    /// <summary>
    /// данный метод получает на входе эл.почту, создает пользователя если его нет
    /// отправляет токен на почту через модуль nopassword
    /// firstName указывается в случае если запрос идет со страницы регистрации, а не входа
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("register_email")]
    public async Task<ActionResult<UserToReturnDto>> RegisterEmail([FromQuery] string email, [FromQuery] string? firstName, [FromQuery] bool isAgency)
    {

      var user = await _userManager.FindByEmailAsync(email);

      if (user == null)
      {

        // create strapi company first and then get id
        var strapiData = new Data()
        {
          Name = "",
          Email = email
        };

        var result = await _strapiService.AddCompanyAsync(strapiData);
        if (result.IsSuccessful != true)
          return BadRequest("Проблемы при создании компании в Strapi");

        var content = result.Content;
        var jsonObject = JsonConvert.DeserializeObject<StrapiProposalContractDto>(content);
        var strapiCompanyId = jsonObject.Data.Id;

        var userToCreate = new AppUser()
        {
          Email = email,
          FirstName = firstName,
          UserName = email,
          CurrentLanguage = "ru-Ru",
          StrapiCompanyId = strapiCompanyId,
          IsAgency = isAgency,
          Currency = Currency.RUB,
          WasOnline = DateTime.Now
        };

        await _userManager.CreateAsync(userToCreate);
      }

      // send code via email
      var url = $"https://localhost:5090/nopassword/login?email={email}&baseUrl={_baseUrl}&mailFrom={_emailDomainMailJetSender}";
      var res = await RetriveNoPasswordAuth(url);
      if (res) return Ok(true);

      return BadRequest("Что-то пошло не так");
    }



    /// <summary>
    /// Получает на входе почту и токен (при регистрации - имя пользователя)
    /// Верифицирует почту
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <param name="firstName"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("login_with_email_code")]
    public async Task<ActionResult<UserToReturnDto>> LoginWithEmailCode([FromQuery] string email, [FromQuery] string code)
    {

      var decodedToken = HttpUtility.UrlDecode(code);
      var userForToken = await _userManager.FindByNameAsync(email);

      if (userForToken == null) return Unauthorized(new ApiResponse(404));

      // verify code for email here
      var verifyUrl = "https://localhost:5090/nopassword/verify?email=" + email + "&token=" + decodedToken;
      var res = await RetriveNoPasswordAuth(verifyUrl);
      if (res)
      {
        var userToReturn = new UserToReturnDto
        {
          Email = userForToken.Email,
          FirstName = userForToken.FirstName,
          SecondName = userForToken.SecondName,
          Token = await _tokenService.CreateToken(userForToken),
          UserRoles = await _userManager.GetRolesAsync(userForToken)
        };
        return Ok(userToReturn);
      }

      return BadRequest("Что-то пошло не так");

    }



    [Authorize]
    [HttpPut]
    [Route("update/profile_contacts")]
    public async Task<ActionResult<AppUser>> UpdateProduct([FromBody] UserToReturnDto userDto)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      user.TelegramUserName = userDto.TelegramUserName;
      user.InstagramUserName = userDto.InstagramUserName;
      user.FacebookUserName = userDto.FacebookUserName;
      user.PhoneNumber = userDto.PhoneNumber;

      var result = await _userManager.UpdateAsync(user);
      return Ok(result);
    }



    [Authorize]
    [HttpPut]
    [Route("update/profile_info")]
    public async Task<ActionResult<AppUser>> UpdateProfileInfo([FromBody] UserInfoDto userDto)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      user.FirstName = userDto.FirstName;
      user.SecondName = userDto.SecondName;
      user.UserDescription = userDto.UserDescription;
      await _userManager.UpdateAsync(user);
      var result = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      return Ok(result);
    }





    /// <summary>
    /// Меняет статус пользователя на onboarded = true
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPut]
    [Route("onboarded")]
    public async Task<IActionResult> SetOnboardedStatus()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user == null)
        return BadRequest("Пользователь не найден");

      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
        return BadRequest("Произошла ошибка при обновлении пользователя");

      return Ok(true);

    }


    /// <summary>
    /// получает по токену информацию о логине и текущем пользователе
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [Route("current")]
    public async Task<ActionResult<UserToReturnDto>> GetCurrentUser()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);

      if (user == null)
        return NotFound("Пользователь не найден");

      var token = await _tokenService.CreateToken(user);
      if (token is null)
        return BadRequest("Не удалось создать токен");

      var roles = await _userManager.GetRolesAsync(user);

      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);
      userToReturn.Token = token;
      userToReturn.UserRoles = roles;
      userToReturn.CreatedAt = RandomDay();
      return Ok(userToReturn);
    }



    /// <summary>
    /// Получаем пользователя по его id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("get_user_by_id")]
    public async Task<ActionResult<UserToReturnDto>> GetUserProfileById([FromQuery] string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);

      if (user == null)
        return NotFound("Пользователь не найден");

      var roles = await _userManager.GetRolesAsync(user);
      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);

      userToReturn.UserRoles = roles;
      userToReturn.CreatedAt = RandomDay();
      return Ok(userToReturn);
    }




    private Random gen = new Random();
    DateTime RandomDay()
    {
      DateTime start = new DateTime(1995, 1, 1);
      int range = (DateTime.Today - start).Days;
      return start.AddDays(gen.Next(range));
    }




    private async Task<bool> RetriveNoPasswordAuth(string requestUrl)
    {
      var client = new HttpClientModule();
      var result = await client.MakeHttpCallWithStream(requestUrl);
      return true;
    }


  }
}

