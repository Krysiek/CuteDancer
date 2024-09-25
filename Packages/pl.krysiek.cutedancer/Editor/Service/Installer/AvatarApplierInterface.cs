namespace VRF
{
    public enum ApplyStatus {
        EMPTY,
        ADD,
        REMOVE,
        UPDATE,
        BLOCKED
    }

    interface AvatarApplierInterface
    {
        void HandleAdd(bool silent = false);
        void HandleRemove(bool silent = false);
        ApplyStatus GetStatus();
    }

}