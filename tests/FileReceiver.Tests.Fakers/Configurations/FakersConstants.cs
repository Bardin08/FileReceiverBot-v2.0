using System;

namespace FileReceiver.Tests.Fakers.Configurations
{
    public static class FakersConstants
    {
        // Common constants
        public const int FakersSeed = 193;

        // File receiving session faker's constants
        public const int MinFilesPerSession = 10;
        public const int MaxFilesPerSession = 100;

        // User faker's constants
        public const long MinUserId = 100000;
        public const long MaxUserId = 999999;
        public static readonly DateTimeOffset RegistrationBeginTimeOffset = DateTimeOffset.UtcNow.AddHours(-12);
    }
}
