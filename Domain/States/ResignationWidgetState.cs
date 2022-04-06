using System;

namespace Domain.States
{
    [Flags]
    public enum ResignationWidgetState
    {
        Default = 0,
        DataLoading = 0x0001,
        ResignationDoesNotExist = 0x0002,
        ResignationExists = 0x0004,
        ResignationSubmitInProgress = 0x0008

    }
}
