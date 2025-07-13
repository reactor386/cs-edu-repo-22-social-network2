//-
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SocialNetwork2.Models;

public class FriendsRepository : Repository<Friend>
{
    public FriendsRepository(ApplicationDbContext db)
        : base(db)
    {
    }

    public void AddFriend(User target, User Friend)
    {
        var friends = base.Set.AsEnumerable()
            .FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

        if (friends == null)
        {
            var item = new Friend()
            {
                UserId = target.Id,
                User = target,
                CurrentFriend = Friend,
                CurrentFriendId = Friend.Id,
            };

            base.Create(item);
        }
    }

    public async Task AddFriendAsync(User target, User Friend)
    {
        var friends = base.Set.AsEnumerable()
            .FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

        if (friends == null)
        {
            var item = new Friend()
            {
                UserId = target.Id,
                User = target,
                CurrentFriend = Friend,
                CurrentFriendId = Friend.Id,
            };

            await base.CreateAsync(item);
        }
    }

    public List<User> GetFriendsByUser(User target)
    {
        var friends = base.Set
            .Include(x => x.CurrentFriend).Include(x => x.User).AsEnumerable()
            .Where(x => x.User.Id == target.Id).Select(x => x.CurrentFriend).OfType<User>();

        return friends.ToList();
    }

    public async Task<List<User>> GetFriendsByUserAsync(User target)
    {
        var friends = base.Set
            .Include(x => x.CurrentFriend).Include(x => x.User).AsQueryable()
            .Where(x => x.User.Id == target.Id).Select(x => x.CurrentFriend).OfType<User>();

        return await friends.ToListAsync();
    }

    public void DeleteFriend(User target, User Friend)
    {
        var friends = base.Set.AsEnumerable()
            .FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

        if (friends != null)
        {
            base.Delete(friends);
        }
    }

    public async Task DeleteFriendAsync(User target, User Friend)
    {
        var friends = base.Set.AsEnumerable()
            .FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

        if (friends != null)
        {
            await base.DeleteAsync(friends);
        }
    }
}
