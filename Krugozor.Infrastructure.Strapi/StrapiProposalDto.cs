using Krugozor.Infrastructure.Strapi.Models;

namespace Krugozor.Infrastructure.Strapi
{
  public class StrapiProposalDto
  {
    public int StrapiProposalId { get; set; }
    public string? Name { get; set; }
    public bool? IsUrgent { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    public double? PlaceArea { get; set; }
    public List<StrapiFieldDto>? Images { get; set; }
    public List<StrapiFieldDto>? Cities { get; set; }
    public List<StrapiFieldDto>? ProposalTypes { get; set; }
    public List<StrapiFieldDto>? Amenities { get; set; }
    public StrapiFieldDto? Category { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? InstagramCompanyName { get; set; }
    public string? TelegramCompanyName { get; set; }
    public string? FacebookCompanyName { get; set; }
    public bool? IsTechnical { get; set; }
    public bool? IsPublished { get; set; }
    public bool? IsRealBusinessEnable { get; set; }
    public int? BuildYear { get; set; }
    public int? BedsQuantity { get; set; }
    public int? ElectricityPower { get; set; }


  }
}
