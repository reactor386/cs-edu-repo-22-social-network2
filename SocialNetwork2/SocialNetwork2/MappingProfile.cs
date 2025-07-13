//-
using System;
using AutoMapper;

using SocialNetwork2.Models;
using SocialNetwork2.ViewModels;


namespace SocialNetwork2;

/// <summary>
/// Настройки маппинга всех сущностей приложения
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterViewModel, User>()
            .ForMember(x => x.BirthDate, opt =>
                opt.MapFrom(c => new DateTime((int)c.Year, (int)c.Month, (int)c.Date)))
            .ForMember(x => x.Email, opt => opt.MapFrom(c => c.EmailReg))
            .ForMember(x => x.UserName, opt => opt.MapFrom(c => c.Login));

        CreateMap<LoginViewModel, User>();

        CreateMap<UserEditViewModel, User>();
        CreateMap<User, UserEditViewModel>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(c => c.Id));
        
        CreateMap<UserWithFriendExtViewModel, User>();
        CreateMap<User, UserWithFriendExtViewModel>();
    }
}
