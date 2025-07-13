//-
using System;
using System.Collections.Generic;

using SocialNetwork2.Models;


namespace SocialNetwork2.ViewModels;

public class UserViewModel
{
    public User User { get; set; }

    public List<User> Friends { get; set; }

    public UserViewModel(User user)
    {
        User = user ?? new User();
        Friends = [];
    }
}
