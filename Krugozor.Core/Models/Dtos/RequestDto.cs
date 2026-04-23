using System;
using Core.Dtos;

namespace Krugozor.Core.Models
{
  public class RequestDto
  {
    public int Id { get; set; }
    public int? ProposalProfileId { get; set; }
    public int InvestorUserId { get; set; }
    public bool IsFavour { get; set; }
    public string Status { get; set; }
    public int RequestStatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public double ShareValue { get; set; }
    public int StrapiProposalNumber { get; set; }
    public UserToReturnDto Investor { get; set; }

  }

}