//-
using System;

using SocialNetwork2.Models;
using SocialNetwork2.ViewModels;


namespace SocialNetwork2.Extensions;

public static class UserExtensions
{
    public static User Convert(this User user, UserEditViewModel usereditvm)
    {
        user.Image = usereditvm.Image;
        user.LastName = usereditvm.LastName;
        user.MiddleName = usereditvm.MiddleName;
        user.FirstName = usereditvm.FirstName;
        user.Email = usereditvm.Email;
        user.BirthDate = usereditvm.BirthDate;
        user.UserName = usereditvm.UserName;
        user.Status = usereditvm.Status;
        user.About = usereditvm.About;

        return user;
    }
}
