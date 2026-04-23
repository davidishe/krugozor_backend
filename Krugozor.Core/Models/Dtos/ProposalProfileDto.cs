using System;
using System.Collections.Generic;

namespace Krugozor.Core.Models
{
  public class ProposalProfileDto
  {
    public int? StrapiProposalId { get; set; } // идшинк предложения в базе страпи
    public int? StrapiCompanyId { get; set; } // идшинк компании-владельца объявления в базе страпи
    public ICollection<RequestDto> Requests { get; set; } // запросы для инвестирования от пльзователей на данное предложение
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public int ProposalProfileStatusId { get; set; }
    public double OveralInvestSum { get; set; }
    public bool? IsReadyToDeal { get; set; }
    public string? InvestPassportLink { get; set; }
    // http://localhost:6014/Assets/Files/Passports/surgut.pdf

  }



}