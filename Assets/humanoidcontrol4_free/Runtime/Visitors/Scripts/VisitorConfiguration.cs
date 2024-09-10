using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer {

    [System.Serializable]
    public class VisitorConfiguration {

        public string name;
        public string startSite;

        protected static string filePath {
            get {
                string filePath = Path.Combine(Application.persistentDataPath, "startup.json");
                return filePath;
            }
        }
        protected static string streamingPath {
            get {
                string filePath = Path.Combine(Application.streamingAssetsPath, "startup.json");
                return filePath;
            }
        }

        private static VisitorConfiguration _configuration;
        public static VisitorConfiguration configuration {
            get {
                if (_configuration == null)
                    GetConfiguration();
                return _configuration;
            }
        }
        private static VisitorConfiguration GetConfiguration() {
            if (_configuration != null)
                return _configuration;

#if UNITY_ANDROID
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);
                _configuration = JsonUtility.FromJson<VisitorConfiguration>(json);
            }
            else {
                if (File.Exists(streamingPath)) {
                    string json = File.ReadAllText(streamingPath);
                    _configuration = JsonUtility.FromJson<VisitorConfiguration>(json);
                }
            }
#else
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);
                _configuration = JsonUtility.FromJson<VisitorConfiguration>(json);
            }
            else
                _configuration = new VisitorConfiguration();

            if (File.Exists(streamingPath)) {
                string json = File.ReadAllText(streamingPath);
                VisitorConfiguration sConfiguration = JsonUtility.FromJson<VisitorConfiguration>(json);
                _configuration.startSite = sConfiguration.startSite;
            }
#endif
            if (_configuration == null)
                _configuration = new VisitorConfiguration();

            return _configuration;
        }

        public static void SaveConfiguration() {
            string json = JsonUtility.ToJson(configuration);
            File.WriteAllText(filePath, json);
#if !UNITY_ANDROID
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            File.WriteAllText(streamingPath, json);
#endif
        }
    }
}