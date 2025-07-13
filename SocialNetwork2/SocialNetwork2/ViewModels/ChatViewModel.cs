//-
using System;
using System.Collections.Generic;

using SocialNetwork2.Models;


namespace SocialNetwork2.ViewModels;

public class ChatViewModel
{
    public User You { get; set; }

    public User ToWhom { get; set; }

    public List<Message> History { get; set; }

    public MessageViewModel NewMessage { get; set; }

    public ChatViewModel()
    {
        NewMessage = new MessageViewModel();
        You = new User();
        ToWhom = new User();
        History = [];
    }
}
