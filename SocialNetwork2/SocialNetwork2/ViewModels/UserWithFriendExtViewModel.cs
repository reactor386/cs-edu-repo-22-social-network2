//-
using System;

using SocialNetwork2.Models;


namespace SocialNetwork2.ViewModels;

public class UserWithFriendExtViewModel : User
{
    public bool IsFriendWithCurrent { get; set; }
}
