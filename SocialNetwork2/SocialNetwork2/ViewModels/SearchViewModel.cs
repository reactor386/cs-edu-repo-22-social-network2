//-
using System;
using System.Collections.Generic;


namespace SocialNetwork2.ViewModels;

public class SearchViewModel
{
    public List<UserWithFriendExtViewModel> UserList { get; set; }
    public SearchViewModel()
    {
        UserList = [];
    }
}
