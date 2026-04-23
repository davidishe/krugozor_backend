using System;
using Core.Models;

namespace Krugozor.Core.Models.Items
{
  public class Picture : BaseEntity
  {
    public bool IsMain { get; set; }
    public string PictureUrl { get; set; }
    public int ProposalId { get; set; }
    // public Proposal? Proposal { get; set; }
    public DateTime EnrolledDate { get; set; }

  }
}