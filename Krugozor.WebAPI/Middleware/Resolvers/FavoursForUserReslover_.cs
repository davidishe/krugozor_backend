using System.Collections.Generic;
using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Microsoft.Extensions.Configuration;
using Krugozor.Core.Models;
using Krugozor.Infrastructure.Database;
using Krugozor.Infrastructure.Specifications;

namespace WebAPI.Helpers
{

  // FavoursForUserReslover
  public class FavoursForUserReslover : IValueResolver<AppUser, UserToReturnDto, List<Favour>>
  {
    public FavoursForUserReslover()
    {
    }

    private readonly IConfiguration _config;
    private readonly IGenericRepository<ProposalProfile> _proposalProfileRepo;


    public FavoursForUserReslover(
      IGenericRepository<ProposalProfile> proposalProfileRepo,
      IConfiguration config)
    {
      _config = config;
      _proposalProfileRepo = proposalProfileRepo;
    }

    public List<Favour> Resolve(AppUser source, UserToReturnDto destination, List<Favour> destMember, ResolutionContext context)
    {
      var spec = new ProposalProfileSpecification(source.Id, false);
      var favours = _proposalProfileRepo.GetEntityWithSpec(spec).Result;
      var res = favours.Favours;
      return res;
    }
  }

}