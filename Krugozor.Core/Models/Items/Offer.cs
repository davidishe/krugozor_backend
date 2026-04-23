using System;
using Core.Models;

namespace Krugozor.Core.Models
{

  /// <summary>
  /// здесь храним заказы, которые предложены на рынке
  /// </summary>
  public class Favour : BaseEntity
  {
    public int ProposalProfileId { get; set; }
    public ProposalProfile ProposalProfile { get; set; }
    public int AppUserId { get; set; }
    public DateTime CreatedAt { get; set; }

  }
}