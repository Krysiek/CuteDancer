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
        void HandleAdd();
        void HandleRemove(bool silent = false);
        ApplyStatus GetStatus();
    }

}