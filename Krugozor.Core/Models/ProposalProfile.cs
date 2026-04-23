using System;
using System.Collections.Generic;
using Core.Models;

namespace Krugozor.Core.Models
{
  public class ProposalProfile : BaseEntity
  {

    public int StrapiProposalId { get; set; } // идшинк предложения в базе страпи
    public int StrapiCompanyId { get; set; } // идшинк компании-владельца объявления в базе страпи
    public ICollection<Request>? Requests { get; set; } // запросы для инвестирования от пльзователей в данном профиле
    public List<Favour>? Favours { get; set; } // оферы на инве
    public ICollection<Grade>? Grades { get; set; }
    public DateTime CreatedAt { get; set; }
    public ProposalProfileStatus Status { get; set; }
    public int ProposalProfileStatusId { get; set; }
    public double OveralInvestSum { get; set; }
    public string? InvestPassportLink { get; set; }
  }
}

