//-
using System;


namespace SocialNetwork2.Models;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set;} = null!;

    public string SenderId { get; set; } = null!;
    public User Sender { get; set; } = null!;

    public string RecipientId { get; set; } = null!;
    public User Recipient { get; set; } = null!;
}
