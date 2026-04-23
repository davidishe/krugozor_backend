using System;
using Core.Models;

namespace Krugozor.Core.Models
{

  /// <summary>
  /// здесь храним заказы, которые добавлены в избранное или заказаны
  /// </summary>
  public class Request : BaseEntity
  {
    public int ProposalProfileId { get; set; }
    public int InvestorUserId { get; set; }
    public bool IsFavour { get; set; }
    public DateTime CreatedAt { get; set; }
    public double ShareValue { get; set; }
    public int StrapiProposalNumber { get; set; } // дубль - идшник объекта пропоузал в страпи, для которого создается "профиль"
    public RequestStatus Status { get; set; }
    public int RequestStatusId { get; set; }
    // public DateTime? UpdatedDate { get; set; }


  }
}

