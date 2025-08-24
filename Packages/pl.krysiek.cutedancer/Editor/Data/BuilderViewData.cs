using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRF
{
    public class BuilderViewData : ScriptableObject
    {
        [NonSerialized]
        public Dictionary<string, List<DanceViewData>> dances;

        public string buildName;
        
        public string parameterName;

        public int parameterStartValue;
    }
}