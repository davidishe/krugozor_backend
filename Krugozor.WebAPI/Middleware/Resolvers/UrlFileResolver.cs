using AutoMapper;
using Microsoft.Extensions.Configuration;
using Krugozor.Core.Models;

namespace WebAPI.Helpers
{

  public class UrlFileResolver : IValueResolver<ProposalProfile, ProposalProfileDto, string>
  {
    public UrlFileResolver()
    {
    }

    private readonly IConfiguration _config;

    public UrlFileResolver(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(ProposalProfile source, ProposalProfileDto destination, string destMember, ResolutionContext context)
    {
      if (!string.IsNullOrEmpty(source.InvestPassportLink))
      {
        return _config.GetSection("AppSettings:ApiUrl").Value + "Assets/Files/Passports/" + source.InvestPassportLink;
      }
      return null;
    }
  }

}