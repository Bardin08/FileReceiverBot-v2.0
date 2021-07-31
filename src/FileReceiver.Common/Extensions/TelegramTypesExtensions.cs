using System;

using Telegram.Bot.Types;

namespace FileReceiver.Common.Extensions
{
    public static class TelegramTypesExtensions
    {
        public static long GetTgUserId(this Update update)
        {
            return update switch
            {
                { Message: { From: { Id: var id } } } => id,
                { InlineQuery: { From: { Id: var id } } } => id,
                { ChosenInlineResult: { From: { Id: var id } } } => id,
                { CallbackQuery: { From: { Id: var id } } } => id,
                { EditedMessage: { From: { Id: var id } } } => id,
                { ChannelPost: { From: { Id: var id } } } => id,
                { EditedChannelPost: { From: { Id: var id } } } => id,
                { ShippingQuery: { From: { Id: var id } } } => id,
                { PreCheckoutQuery: { From: { Id: var id } } } => id,
                { PollAnswer: { User: { Id: var id } } } => id,
                { MyChatMember: { From: { Id: var id } } } => id,
                { ChatMember: { From: { Id: var id } } } => id,
                _ => throw new ArgumentOutOfRangeException(nameof(update))
            };
        }
    }
}
