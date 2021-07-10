namespace FileReceiver.Dal.Entities.Enums
{
    public enum RegistrationStateDb
    {
        NewUser = 0,
        FirstNameReceived = 1,
        LastNameReceived = 2,
        SecretWordReceived = 3,
        RegistrationComplete = 4,
    }
}
