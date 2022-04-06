namespace Domain.Types
{
    public enum SanctionWidgetState
    {
        Default = 0x0000,
        DataLoading = 0x0001,
        DataLoaded = 0x0002,
        EditIdle = 0x0004,
        EditInProgress = 0x0008
    }
}
