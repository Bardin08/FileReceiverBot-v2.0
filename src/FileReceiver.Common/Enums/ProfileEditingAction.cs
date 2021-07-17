using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileReceiver.Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProfileEditingAction
    {
        UpdateFirstName = 0,
        UpdateLastName = 1,
        UpdateSecretWord = 2,
    }
}
