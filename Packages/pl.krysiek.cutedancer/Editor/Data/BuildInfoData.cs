using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRF
{
    public class BuildInfoData : ScriptableObject
    {
        [Serializable]
        public class FilePathGuid
        {
            public string path;
            public string guid;
        }

        [SerializeField]
        private string version = "0.0.0-dev";
        public string Version
        {
            set => version = value;
            get => version;
        }

        [SerializeField]
        private string buildDate;
        public DateTime BuildDate
        {
            set => buildDate = value.ToString();
            get => DateTime.Parse(buildDate);
        }


        [SerializeField]
        private string[] files;
        public string[] Files
        {
            set => files = value;
            get => files;
        }

        [SerializeField]
        public List<FilePathGuid> filePathUuids = new List<FilePathGuid>();
    }
}