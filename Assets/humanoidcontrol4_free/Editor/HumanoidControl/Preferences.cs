/*
#if UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Passer {
    using Humanoid;
    using Humanoid.Tracking;
    using Passer.Tracking;

    [InitializeOnLoad]
    public class HumanoidConfiguration : MonoBehaviour {
        static HumanoidConfiguration() {
            //Configuration_Editor.GlobalDefine("pHUMANOID");
#if hLEAP
            LeapDevice.LoadDlls();
#endif
#if hORBBEC
            AstraDevice.LoadDlls();
#endif
#if hNEURON
            NeuronDevice.LoadDlls();
#endif
        }
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        // Have we loaded the prefs yet
        public static Configuration configuration;

    }

}
#endif
*/