using System;
using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Exceptions;
using FileReceiver.Common.Extensions;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiver.Bl.Impl.Handlers.TelegramUpdate
{
    public class FileReceivingSessionCreatingUpdateHandler : IUpdateHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IFileReceivingSessionService _receivingSessionService;
        private readonly ITelegramBotClient _botClient;

        public FileReceivingSessionCreatingUpdateHandler(
            IBotMessagesService botMessagesService,
            IFileReceivingSessionService receivingSessionService,
            ITelegramBotClient botClient)
        {
            _botMessagesService = botMessagesService;
            _receivingSessionService = receivingSessionService;
            _botClient = botClient;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var userId = update.GetTgUserId();
            var sessionId = await _receivingSessionService
                .GetFirstActiveFileReceivingSessionIdByUserIdAsync(userId);
            if (!sessionId.HasValue)
            {
                await _botMessagesService.SendErrorAsync(userId, "All your sessions are configured and executed now. " +
                                                                 "To check the list of active file receiving sessions " +
                                                                 "use command /active_sessions");
                return;
            }

            var sessionState = await _receivingSessionService.GetSessionStateAsync(sessionId.Value);

            var data = new FileReceivingSessionDataDto()
            {
                UserId = userId,
                SessionId = sessionId.Value,
                Update = update,
            };

            // TODO: rewrite with pattern matching
            switch (sessionState)
            {
                case FileReceivingSessionState.FileSizeConstraintSet:
                    await SetFileSizeConstraintAsync(data);
                    break;
                case FileReceivingSessionState.FileNameConstraintSet:
                    await SetFileNameConstraintAsync(data);
                    break;
                case FileReceivingSessionState.FileExtensionConstraintSet:
                    await SetFileExtensionsConstraintAsync(data);
                    break;
                case FileReceivingSessionState.SessionMaxFilesConstraint:
                    await SetSessionMaxFilesConstraintAsync(data);
                    break;
                case FileReceivingSessionState.SetFilesStorage:
                    await SetFileStorageAsync(data);
                    break;
                case FileReceivingSessionState.ActiveSession:
                    await ExecuteFileReceivingSessionAsync(data.UserId);
                    break;
                case FileReceivingSessionState.EndedSession:
                    await _botMessagesService.SendTextMessageAsync(data.UserId, "This session ended");
                    break;
                default:
                    await _botMessagesService.SendErrorAsync(userId,
                        "Invalid action. Use command /cancel and then /start_receiving to recreate session");
                    break;
            }
        }

        private async Task SetFileSizeConstraintAsync(FileReceivingSessionDataDto data)
        {
            try
            {
                var maxFileSize = int.Parse(data.Update.Message.Text);
                if (maxFileSize <= 0)
                {
                    await _botMessagesService.SendErrorAsync(data.UserId, "File size must be greater than zero");
                    return;
                }

                await _receivingSessionService.SetFileSizeConstraintAsync(data.SessionId, maxFileSize);
                await _botMessagesService.SendTextMessageAsync(data.UserId,
                    // TODO: Add regex pattern examples
                    "Now send me file name constraints written with regex pattern. " +
                    "You can check your pattern [here](https://regex101.com/). " +
                    "To send several patterns separate them with a coma");
            }
            catch (Exception)
            {
                // TODO: Log the exception
                await _botMessagesService.SendErrorAsync(data.UserId,
                    "An error occured while setting file size constraint");
            }
        }

        private async Task SetFileNameConstraintAsync(FileReceivingSessionDataDto data)
        {
            try
            {
                await _receivingSessionService.SetFileNameConstraintAsync(data.SessionId, data.Update.Message.Text);
                await _botMessagesService.SendTextMessageAsync(data.UserId,
                    "Now send me this file extension constraints. For example: *pdf, docx* ");
            }
            catch (Exception)
            {
                // TODO: Log the exception
                await _botMessagesService.SendErrorAsync(data.UserId,
                    "An error occured while setting file name constraint");
            }
        }

        private async Task SetFileExtensionsConstraintAsync(FileReceivingSessionDataDto data)
        {
            try
            {
                await _receivingSessionService.SetFileExtensionConstraintAsync(data.SessionId,
                    data.Update.Message.Text);
                await _botMessagesService.SendTextMessageAsync(data.UserId,
                    "Now send me how much files can be stored at this session");
            }
            catch (Exception)
            {
                // TODO: Log the exception
                await _botMessagesService.SendErrorAsync(data.UserId,
                    "An error occured while setting file extensions constraint");
            }
        }

        private async Task SetSessionMaxFilesConstraintAsync(FileReceivingSessionDataDto data)
        {
            try
            {
                var maxFilesAmount = int.Parse(data.Update.Message.Text);
                if (maxFilesAmount <= 0)
                {
                    await _botMessagesService.SendErrorAsync(data.UserId, "Files amount must be greater that zero");
                    return;
                }

                await _receivingSessionService.SetSessionMaxFilesConstraintAsync(data.UserId, data.SessionId,
                    maxFilesAmount);
                InlineKeyboardMarkup fileStorageKeyboard = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Mega", "fr-session-storage-mega"),
                    },
                });
                await _botMessagesService.SendMessageWithKeyboardAsync(data.UserId, "Select the file storage",
                    fileStorageKeyboard);
            }
            catch (Exception)
            {
                // TODO: Log the exception
                await _botMessagesService.SendErrorAsync(data.UserId,
                    "An error occured while setting file receiving session storage");
            }
        }

        private async Task SetFileStorageAsync(FileReceivingSessionDataDto data)
        {
            try
            {
                var storageType = Enum.Parse<FileStorageType>(data.Update.Message.Text);

                try
                {
                    await _receivingSessionService.SetFilesStorageAsync(data.UserId, storageType);
                }
                catch (FileReceivingSessionActionErrorException ex)
                {
                    // TODO: Log the exception
                    await _botMessagesService.SendErrorAsync(data.UserId, ex.Message);
                }

                InlineKeyboardMarkup submitSession = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Execute", "fr-session-execute"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Cancel", "fr-session-cancel"),
                    },
                });
                await _botMessagesService.SendMessageWithKeyboardAsync(data.UserId, "Session info", submitSession);
            }
            catch (Exception)
            {
                // TODO: Log the exception
                await _botMessagesService.SendErrorAsync(data.UserId,
                    "An error occured while confirming the session data");
            }
        }

        private async Task ExecuteFileReceivingSessionAsync(long userId)
        {
            try
            {
                try
                {
                    await _botMessagesService.SendTextMessageAsync(userId,
                        "Okay, this operation may take some time. I need to create all the required infrastructure");
                    await _botClient.SendChatActionAsync(userId, ChatAction.FindLocation);
                    var sessionId = await _receivingSessionService.ExecuteSessionAsync(userId);
                    await Task.Delay(7000);
                    // TODO: Remove delay
                    await _botMessagesService.SendTextMessageAsync(userId, $"Great. Your session id is: `{sessionId}`");
                }
                catch (FileReceivingSessionActionErrorException ex)
                {
                    // TODO: Log the exception
                    await _botMessagesService.SendErrorAsync(userId, ex.Message);
                }
            }
            catch (Exception)
            {
                // TODO: Log the exception
                await _botMessagesService.SendErrorAsync(userId,
                    "An error occured while creating the session's infrastructure");
            }
        }

        private sealed record FileReceivingSessionDataDto
        {
            public long UserId { get; init; }
            public Guid SessionId { get; init; }
            public Update Update { get; init; }
        }
    }
}
