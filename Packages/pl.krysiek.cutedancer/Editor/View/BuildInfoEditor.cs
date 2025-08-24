using System.IO;

using UnityEditor;
using UnityEngine.UIElements;


namespace VRF
{
    public class BuildInfoEditor : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BuildInfoEditor, UxmlTraits> { }

        public BuildInfoData BuildInfoData
        {
            set
            {
                if (value)
                {
                    this.Q<Label>("Version").text = value.Version;
                    this.Q<Label>("BuildDate").text = value.BuildDate.ToString();
                    this.Q<Label>("Location").text = Path.GetDirectoryName(AssetDatabase.GetAssetPath(value));
                }
                else
                {
                    this.Q<Label>("Version").text = "-";
                    this.Q<Label>("BuildDate").text = "-";
                    this.Q<Label>("Location").text = "-";

                }
            }
        }

        public BuildInfoEditor()
        {
            CuteResources.LoadView("BuildInfoView").CloneTree(this);
        }
    }
}
