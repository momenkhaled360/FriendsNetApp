using API.DTOs;
using API.Entities;
using System.Linq.Expressions;

namespace API.Extensions
{
    public static class MessageExtenstions
    {
        public static MessageDto ToDto(this Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderDisplayName = message.Sender.DisplayName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientDisplayName = message.Recipient.DisplayName,
                RecipientImageUrl = message.Recipient.ImageUrl,
                Content = message.Content,
                MessageSent = message.MessageSent,
                DateRead = message.DateRead,
            };

        }

        internal static Expression<Func<Message,MessageDto>> ToDtoProjection()
        {
            return message => new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderDisplayName = message.Sender.DisplayName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientDisplayName = message.Recipient.DisplayName,
                RecipientImageUrl = message.Recipient.ImageUrl,
                Content = message.Content,
                MessageSent = message.MessageSent,
                DateRead = message.DateRead,
            };
        }
    }
}
