using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiver.Common.Constants
{
    public static class Keyboards
    {
        public static readonly InlineKeyboardMarkup ProfileEditActionsKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Edit First Name", "profile-edit-first-name"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Edit Last Name", "profile-edit-last-name"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Edit Secret Word", "profile-edit-secret-word"),
            },
        });

    }
}
