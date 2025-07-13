//-
using System;


namespace SocialNetwork2.Models;

public class Friend
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    public string? CurrentFriendId { get; set; }

    public User? CurrentFriend { get; set; }
}
