//-
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SocialNetwork2.Models;

public class MessageRepository : Repository<Message>
{
    public MessageRepository(ApplicationDbContext db)
        : base(db)
    {
    }

    public List<Message> GetMessages(User sender, User recipient)
    {
        base.Set.Include(x => x.Recipient);
        base.Set.Include(x => x.Sender);

        var from = base.Set.AsEnumerable()
            .Where(x => x.SenderId == sender.Id && x.RecipientId == recipient.Id).ToList();

        var to = base.Set.AsEnumerable()
            .Where(x => x.SenderId == recipient.Id && x.RecipientId == sender.Id).ToList();

        var itog = new List<Message>();
        itog.AddRange(from);
        itog.AddRange(to);
        itog.OrderBy(x => x.Id);

        return itog;
    }

    public async Task<List<Message>> GetMessagesAsync(User sender, User recipient)
    {
        base.Set.Include(x => x.Recipient);
        base.Set.Include(x => x.Sender);

        var from = await base.Set.AsQueryable()
            .Where(x => x.SenderId == sender.Id && x.RecipientId == recipient.Id).ToListAsync();

        var to = await base.Set.AsQueryable()
            .Where(x => x.SenderId == recipient.Id && x.RecipientId == sender.Id).ToListAsync();

        var itog = new List<Message>();
        itog.AddRange(from);
        itog.AddRange(to);
        itog.OrderBy(x => x.Id);
        
        return itog;
    }
}
