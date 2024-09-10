using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Passer.Humanoid {

    /// <summary>
    /// Sets preferences for \ref Humanoid "Humanoid Control"
    /// </summary>
    /// 
    /// The preferences can be accessed by the Edit->Preferences... menu.
    /// 
    /// \image html HumanoidPreferences.png
    /// \image rtf HumanoidPreferences.png
    /// 
    /// * SteamVR Support, enables support for SteamVR devices beyound Unity XR (VR, Plus, Pro)
    /// * Vive Tracker Support, enables support for Vive Trackers (Plus, Pro)
    /// * Leap Motion Support, enables support for Leap Motion (Plus, Pro)
    /// * Kinect 1 Support, enables support for Microsoft Kinect 360/Kinect for Windows (Plus, Pro)
    /// * Kinect 2 Support, enables support for Microsoft Kinect 2 (Plus, Pro)
    /// * Azure Kinect Support, enables support for Azure Kinect (Plus, Pro)
    /// * Orbbec Astra Support, enables support for Orbbec Astra (Plus, Pro)
    /// * Hydra Support, enables support for Razer Hydra (Pro)
    /// * Tobii Support, enables support for Tobii desktop eye tracking (Pro)
    /// * Perception Neuron Support, enables support for Perception Neuron (Pro)
    /// * OptiTrack Support, enables support for OptiTrack (Pro)
    /// * Antilatency Support, enables support for Antilatency (Pro)
    /// 
    /// * The Testing Visitor selects which Visitor Scene will be used when a Site is started in Play mode.
    /// 
    //[System.Serializable]
    public class HumanoidPreferences : ScriptableObject {
        public static bool help = false;
        public const string settingsPath = "Assets/Passer/Humanoid/HumanoidPreferences.asset";

        public Configuration configuration;

        //internal static HumanoidPreferences GetOrCreateSettings() {
        //    string humanoidPath = Configuration_Editor.FindHumanoidFolder();
        //    humanoidPath = humanoidPath.Substring(0, humanoidPath.Length - 1); // strip last /
        //    humanoidPath = humanoidPath.Substring(0, humanoidPath.LastIndexOf('/') + 1); // strip Scripts;
        //    string settingsPath = "Assets" + humanoidPath + "HumanoidPreferences.asset";
        //    HumanoidPreferences settings = AssetDatabase.LoadAssetAtPath<HumanoidPreferences>(settingsPath);
        //    SerializedObject serializedSettings = new SerializedObject(settings);
        //    if (settings == null) {
        //        settings = CreateInstance<HumanoidPreferences>();

        //        AssetDatabase.CreateAsset(settings, settingsPath);
        //    }
        //    if (settings.configuration == null) {
        //        string configurationString = EditorPrefs.GetString("HumanoidConfigurationKey", "DefaultConfiguration");
        //        Configuration configuration = Configuration_Editor.LoadConfiguration(configurationString);
        //        if (configuration == null) {
        //            configurationString = "DefaultConfiguration";
        //            Configuration_Editor.LoadConfiguration(configurationString);
        //            if (configuration == null) {
        //                Debug.Log("Created new Default Configuration");
        //                // Create new Default Configuration
        //                configuration = CreateInstance<Configuration>();
        //                string path = "Assets" + humanoidPath + configurationString + ".asset";
        //                AssetDatabase.CreateAsset(configuration, path);
        //                AssetDatabase.SaveAssets();
        //            }
        //        }
        //        SerializedProperty configurationProp = serializedSettings.FindProperty("configuration");
        //        configurationProp.objectReferenceValue = configuration;
        //        EditorUtility.SetDirty(settings);
        //    }
        //    serializedSettings.ApplyModifiedProperties();
        //    return (HumanoidPreferences)serializedSettings.targetObject;//settings;
        //}

        //internal static SerializedObject GetOrCreateSerializedSettings() {
        //    string humanoidPath = Configuration_Editor.FindHumanoidFolder();
        //    humanoidPath = humanoidPath.Substring(0, humanoidPath.Length - 1); // strip last /
        //    humanoidPath = humanoidPath.Substring(0, humanoidPath.LastIndexOf('/') + 1); // strip Scripts;
        //    string settingsPath = "Assets" + humanoidPath + "HumanoidPreferences.asset";
        //    HumanoidPreferences settings = AssetDatabase.LoadAssetAtPath<HumanoidPreferences>(settingsPath);

        //    if (settings == null) {
        //        Debug.Log("Created new Settings");
        //        settings = CreateInstance<HumanoidPreferences>();

        //        AssetDatabase.CreateAsset(settings, settingsPath);
        //    }
        //    if (settings.configuration == null) {
        //        Debug.Log("Settings Configuration is not set");
        //        string configurationString = "DefaultConfiguration";
        //        Configuration configuration = Configuration_Editor.LoadConfiguration(configurationString);
        //        if (configuration == null) {
        //            configurationString = "DefaultConfiguration";
        //            Configuration_Editor.LoadConfiguration(configurationString);
        //            if (configuration == null) {
        //                Debug.Log("Created new Default Configuration");
        //                // Create new Default Configuration
        //                configuration = CreateInstance<Configuration>();
        //                string path = "Assets" + humanoidPath + configurationString + ".asset";
        //                AssetDatabase.CreateAsset(configuration, path);
        //                AssetDatabase.SaveAssets();
        //            }
        //        }
        //        settings.configuration = configuration;
        //    }
        //    SerializedObject serializedSettings = new SerializedObject(settings);
        //    return serializedSettings;
        //}

#if UNITY_EDITOR
        private const string networkingSupportKey = "HumanoidNetworkingSupport";
        public static NetworkingSystems networkingSupport {
            get {
                NetworkingSystems networkingSupport = (NetworkingSystems)EditorPrefs.GetInt(networkingSupportKey, 0);
                return networkingSupport;
            }
            set {
                EditorPrefs.SetInt(networkingSupportKey, (int)value);
            }
        }

        private const string visitorSceneNameKey = "VisitorSceneName";
        public static string visitorSceneName {
            get {
                string visitorSceneName = EditorPrefs.GetString(visitorSceneNameKey, "");
                return visitorSceneName;
            }
            set {
                EditorPrefs.SetString(visitorSceneNameKey, value);
            }
        }
#endif
    }

#if UNITY_EDITOR
    static class HumanoidPreferencesIMGUIRegister {
        //public static bool reload;

        [SettingsProvider]
        public static SettingsProvider CreateHumanoidSettingsProvider() {
            var provider = new SettingsProvider("Preferences/HumanoidControlSettings", SettingsScope.User) {
                label = "Humanoid Control",
                guiHandler = (searchContext) => {

                    VisitorSceneInspector();

                },
                keywords = new HashSet<string>(
                    new[] { "Humanoid", "Oculus", "SteamVR" }
                    )
            };
            return provider;
        }

        private static string[] visitorNames;
        private static bool VisitorSceneInspector() {
            string visitorScenePath = HumanoidPreferences.visitorSceneName;

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(visitorScenePath);            
            sceneAsset = (SceneAsset) EditorGUILayout.ObjectField("Visitor Scene", sceneAsset, typeof(SceneAsset), false);
            visitorScenePath = AssetDatabase.GetAssetPath(sceneAsset);

            bool anyChanged = visitorScenePath != HumanoidPreferences.visitorSceneName;

            HumanoidPreferences.visitorSceneName = visitorScenePath;

            return anyChanged;
        }
    }
#endif
}