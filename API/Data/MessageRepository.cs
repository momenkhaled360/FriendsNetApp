using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message?> GetMessage(string messageId)
        {
            return await context.Messages.FindAsync(messageId);
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Outbox" => query.Where(x => x.SenderId == messageParams.MemberId && x.SenderDeleted == false),
                "Inbox" => query.Where(x => x.RecipientId == messageParams.MemberId && x.SenderDeleted == false),
                _ => throw new Exception("Invalid container")
            };

            var messageQuery = query.Select(MessageExtenstions.ToDtoProjection());

            return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
        {
            await context.Messages
                 .Where(x => x.SenderId == recipientId
                 && x.RecipientId == currentMemberId && x.DateRead == null)
                 .ExecuteUpdateAsync(setters => setters
                 .SetProperty(x => x.DateRead, DateTime.UtcNow));

            return await context.Messages
                .Where(x => (x.RecipientId == currentMemberId 
                    && x.RecipientDeleted == false 
                    && x.SenderId == recipientId)
                    || (x.RecipientId == recipientId
                    && x.RecipientDeleted == false 
                    && x.SenderId == currentMemberId))
                .OrderBy(x=>x.MessageSent)
                .Select(MessageExtenstions.ToDtoProjection())
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
