using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileReceiver.Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegistrationState {
        NewUser = 0,
        FirstNameReceived = 1,
        LastNameReceived = 2,
        SecretWordReceived = 3,
        RegistrationComplete = 4,
    }
}
