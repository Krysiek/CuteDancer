using UnityEngine.UIElements;

namespace VRF
{
    public class AboutView : VisualElement
    {

        public AboutView()
        {
            CuteResources.LoadView("AboutView").CloneTree(this);
        }
    }
}