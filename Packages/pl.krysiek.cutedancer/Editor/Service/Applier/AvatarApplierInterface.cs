using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    interface AvatarApplierInterface
    {
        void SetAvatar(AvatarDescriptor avatar);
        void HandleAdd();
        void HandleRemove();
    }

}