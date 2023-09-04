using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class CuteResources
{
    public static VisualTreeAsset LoadView(string name)
    {
        return Resources.Load<VisualTreeAsset>(Path.Combine("CuteDancer", "Views", name));
    }

}
