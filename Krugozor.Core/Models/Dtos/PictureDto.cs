using System;
using Core.Models;

namespace Krugozor.Core.Models.Items
{
  public class PictureDto : BaseEntity
  {
    public bool IsMain { get; set; }
    public string PictureUrl { get; set; }
    public DateTime EnrolledDate { get; set; }

  }
}