using System;

using FileReceiver.Common.Constants;

namespace FileReceiver.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsPossibleCommand(this string text)
        {
            return text is not null && text.StartsWith('/');
        }

        public static string GetCommandFromMessage(this string text)
        {
            var messageAsSpan = text.AsSpan();
            var tillIndex = messageAsSpan.IndexOfAny(StringConstants.Delimiters);
            tillIndex = tillIndex is not -1 ? tillIndex : messageAsSpan.Length;
            return messageAsSpan[..tillIndex].ToString();
        }
    }
}
