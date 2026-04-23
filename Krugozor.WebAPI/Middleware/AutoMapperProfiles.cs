using Core.Dtos;
using Core.Identity;
using Krugozor.Core.Models;
using Krugozor.Core.Models.Messages;

namespace WebAPI.Helpers
{
  public class AutoMapperProfiles : AutoMapper.Profile
  {

    public AutoMapperProfiles()
    {
      CreateMap<AppUser, UserToReturnDto>()
      .ForMember(d => d.PictureUrl, m => m.MapFrom<UrlPictureForUserReslover>())
      .ForMember(d => d.UserRoles, m => m.MapFrom<UserRolesReslover>());

      CreateMap<AppUser, UserToReturnShortDto>();

      CreateMap<Correspondent, CorrespondentDto>()
          .ForMember(d => d.AppUser, m => m.MapFrom<UserForCorrespondentResolver>());


      CreateMap<ProposalProfile, ProposalProfileDto>()
        .ForMember(d => d.Status, m => m.MapFrom(s => s.Status.Name))
        .ForMember(d => d.InvestPassportLink, m => m.MapFrom<UrlFileResolver>())
        .ForMember(d => d.IsReadyToDeal, m => m.MapFrom<ProposalProfileReadyToDealResolver>());


      // .ForMember(d => d.DocByte, m => m.MapFrom<DocTypeResolver>());

      CreateMap<ConnectionItem, ConnectionItemDto>();
      CreateMap<Message, MessageDto>();
      CreateMap<Image, ImageDto>();


      CreateMap<Chat, ChatDto>()
        .ForMember(d => d.DestinationUser, m => m.MapFrom<RecepientUserForChatResolver>())
        .ForMember(d => d.AuthorUser, m => m.MapFrom<AuthorUserForChatResolver>());



      CreateMap<Request, RequestDto>()
          .ForMember(d => d.Investor, m => m.MapFrom<InvestorForRequestResolver>())
          .ForMember(d => d.Status, m => m.MapFrom(s => s.Status.Name));

      //   .ForMember(d => d.InvestorName, m => m.MapFrom(s => s.Investor.Name));



    }



  }
}