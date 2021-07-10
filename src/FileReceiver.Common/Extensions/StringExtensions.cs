using FileReceiver.Common.Constants;

namespace FileReceiver.Common.Extensions
{
    public static class StringExtensions
    {
        public static string GetCommandFromMessage(this string message)
        {
            var tillIndex = message.IndexOfAny(StringConstants.Delimiters);
            return message.Substring(0, tillIndex != -1 ? tillIndex : message.Length);
        }
    }
}
