using Newtonsoft.Json;

namespace Krugozor.Infrastructure.Strapi.Models
{
  public class StrapiFieldDto
  {
    [JsonProperty("id")]
    public int Id { get; set; }

  }


}