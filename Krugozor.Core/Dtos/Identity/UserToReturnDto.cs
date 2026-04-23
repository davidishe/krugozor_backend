using System;
using System.Collections.Generic;

namespace Core.Dtos
{
  public class UserToReturnDto
  {
    public int? Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? PictureUrl { get; set; }
    public string? UserDescription { get; set; }
    public string? Token { get; set; }
    public string? CurrentLanguage { get; set; }
    public IList<string>? UserRoles { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? WasOnline { get; set; }
    public long? TelegramId { get; set; }
    public string? TelegramUserName { get; set; }
    public string? InstagramUserName { get; set; }
    public string? FacebookUserName { get; set; }
    public string? PnoneNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? BirthDate { get; set; } = DateTime.Now.AddYears(-27);
    public int? StrapiCompanyId { get; set; }
    public bool? IsAgency { get; set; }


  }
}