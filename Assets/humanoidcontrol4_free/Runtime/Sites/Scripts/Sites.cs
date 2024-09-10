using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer {

    [System.Serializable]
    public class Sites : ScriptableObject {

        [System.Serializable]
        public class SiteBuild {
            public string siteName;
            public bool enabled;
        }

        public List<SiteBuild> list = new List<SiteBuild>();
    }
}