namespace FileReceiver.Dal.Entities.Enums
{
    public enum FileReceivingSessionStateDb
    {
        NewSession = 0,
        FileSizeConstraintSet = 1,
        FileNameConstraintSet = 2,
        FileExtensionConstraintSet = 3,
        SessionMaxFilesConstraint = 4,
        SetFilesStorage = 5,
        ActiveSession = 99,
        EndedSession = 100,
    }
}
