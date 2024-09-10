using UnityEngine;
using UnityEngine.XR;

namespace Passer.Tracking {

    public static class UnityXRDevice {

        public static void Start() {
            xrDevice = DetermineLoadedDevice();
        }

        public enum XRDeviceType {
            None,
            Oculus,
            OpenVR,
            WindowsMR,
            Cardboard,
        };
        public static XRDeviceType xrDevice = XRDeviceType.None;

        private static XRDeviceType DetermineLoadedDevice() {
            if (XRSettings.enabled) {
                switch (XRSettings.loadedDeviceName) {
                    case "OpenVR":
                    case "OpenVR Display":
                        return XRDeviceType.OpenVR;
                    case "Oculus":
                    case "Oculus Display":
                    case "oculus display":
                        return XRDeviceType.Oculus;
                    case "WindowsMR":
                        return XRDeviceType.WindowsMR;
                    case "cardboard":
                        return XRDeviceType.Cardboard;
                }
            }
            return XRDeviceType.None;
        }
    }
}