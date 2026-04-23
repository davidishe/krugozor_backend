using Krugozor.Infrastructure.Strapi.Models;
using Newtonsoft.Json;

namespace Krugozor.Infrastructure.Strapi;


public class StrapiProposalContract
{
  [JsonProperty("data")]
  public object? Data { get; set; }

  [JsonProperty("meta")]
  public object? Meta { get; set; }
}


/// <summary>
/// Proposal data fields
/// </summary>
public class Data
{

  [JsonProperty("name")]
  public string? Name { get; set; }

  [JsonProperty("description")]
  public string? Description { get; set; }

  [JsonProperty("isUrgent")]
  public bool? IsUrgent { get; set; }

  [JsonProperty("locale")]
  public string? Locale { get; set; }

  [JsonProperty("price")]
  public double? Price { get; set; }

  [JsonProperty("placeArea")]
  public double? PlaceArea { get; set; }

  [JsonProperty("storeAddress")]
  public string? StoreAddress { get; set; }

  [JsonProperty("storeRating")]
  public decimal? StoreRating { get; set; }

  [JsonProperty("storeRatingQuantity")]
  public int? StoreRatingQuantity { get; set; }

  [JsonProperty("authorId")]
  public int? AuthorId { get; set; }

  [JsonProperty("languageSpeaker")]
  public object? LanguageSpeaker { get; set; }

  [JsonProperty("uuid")]
  public string? Uuid { get; set; }

  [JsonProperty("createdAt")]
  public DateTime? CreatedAt { get; set; }

  [JsonProperty("updatedAt")]
  public DateTime? UpdatedAt { get; set; }

  [JsonProperty("publishedAt")]
  public DateTime? PublishedAt { get; set; }

  [JsonProperty("images")]
  public List<StrapiFieldDto>? Images { get; set; }

  [JsonProperty("cities")]
  public List<StrapiFieldDto>? Cities { get; set; }

  [JsonProperty("proposal_types")]
  public List<StrapiFieldDto>? ProposalTypes { get; set; }

  [JsonProperty("amenities")]
  public List<StrapiFieldDto>? Amenities { get; set; }

  [JsonProperty("company")]
  public StrapiFieldDto? Company { get; set; }

  [JsonProperty("email")]
  public string? Email { get; set; }

  [JsonProperty("website")]
  public string? Website { get; set; }

  [JsonProperty("address")]
  public string? Address { get; set; }

  [JsonProperty("phone")]
  public string? Phone { get; set; }

  [JsonProperty("instagramCompanyName")]
  public string? InstagramCompanyName { get; set; }

  [JsonProperty("telegramCompanyName")]
  public string? TelegramCompanyName { get; set; }

  [JsonProperty("facebookCompanyName")]
  public string? FacebookCompanyName { get; set; }

  [JsonProperty("isTechnical")]
  public bool? IsTechnical { get; set; }

  [JsonProperty("isRealBusinessEnable")]
  public bool? IsRealBusinessEnable { get; set; }

  [JsonProperty("isPublished")]
  public bool? IsPublished { get; set; }

  [JsonProperty("buildYear")]
  public int? BuildYear { get; set; }

  [JsonProperty("bedsQuantity")]
  public int? BedsQuantity { get; set; }

  [JsonProperty("electricityPower")]
  public int? ElectricityPower { get; set; }


}



public class StrapiProposalContractDto
{
  [JsonProperty("data")]
  public DataDto? Data { get; set; }

  [JsonProperty("meta")]
  public object? Meta { get; set; }
}

public class DataDto
{

  [JsonProperty("attributes")]
  public Data? Attributes { get; set; }

  [JsonProperty("id")]
  public int Id { get; set; }
}



