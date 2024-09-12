using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

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
        void HandleRemove();
        ApplyStatus GetStatus();
    }

}