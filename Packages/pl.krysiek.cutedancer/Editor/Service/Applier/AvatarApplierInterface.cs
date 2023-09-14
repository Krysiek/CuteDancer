using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    interface AvatarApplierInterface
    {
        // void RenderForm();
        void SetAvatar(AvatarDescriptor avatar);
        // void ClearForm();
        // void RenderStatus();
        void HandleAdd();
        void HandleRemove();
    }

}