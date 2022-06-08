using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    interface CuteGroup
    {
        void RenderForm();
        void SetAvatar(AvatarDescriptor avatar);
        void ClearForm();
        void RenderStatus();
    }

}