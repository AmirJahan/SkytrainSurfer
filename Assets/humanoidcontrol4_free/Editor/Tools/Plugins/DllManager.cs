using System.IO;
using UnityEngine;
using UnityEditor;

namespace Passer {

    [InitializeOnLoad]
    public class DllManager {
        static DllManager() {
            // Default plugins
            string pluginsPath = GetPluginsPath();
            InstallPlugins(pluginsPath);

            // Passer Humanoid plugins
            pluginsPath = "/Passer/Humanoid/Plugins/";
            InstallPlugins(pluginsPath);
        }

        private static string GetPluginsPath() {
            string pluginsPath = "/Passer/Plugins/";

            // Determine in which (sub)folder DllManager has been placed
            // This makes it possible to place DllManager in a different folder
            string[] hcScripts = AssetDatabase.FindAssets("DllManager");
            for (int i = 0; i < hcScripts.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(hcScripts[i]);
                if (assetPath.Length > 30 && assetPath.Substring(assetPath.Length - 21, 21) == "/Editor/DllManager.cs") {
                    pluginsPath = assetPath.Substring(6, assetPath.Length - 26);
                }
            }

            return pluginsPath;
        }

        private static void InstallPlugins(string pluginsPath) {
            string installedPath = Application.dataPath + pluginsPath;
            string installablesPath = installedPath + "Installables/";
            //Debug.Log("Checking for Installable plugins in " + installablesPath);
            if (!Directory.Exists(installablesPath))
                return;

            string[] installableFileNames = Directory.GetFiles(installablesPath, "*.dll");

            if (installableFileNames.Length == 0)
                return;

            Debug.Log("Found " + installableFileNames.Length + " installable dlls");

            foreach (string installableFileName in installableFileNames) {
                int nameindex = installableFileName.LastIndexOf('/') + 1;
                string pluginName = installableFileName.Substring(nameindex);
                bool success = InstallPlugin(installableFileName, installedPath + pluginName);
                if (success == false)
                    return;
            }
        }

        private static bool InstallPlugin(string sourceFileName, string destFileName) {
            if (File.Exists(destFileName)) {
                try {
                    File.Delete(destFileName);
                }
                catch (System.Exception) {
                    EditorUtility.DisplayDialog("Plugin Installation", "Please restart Unity to install new plugins", "Dismiss");
                    return false;
                }
            }

            int nameindex = sourceFileName.LastIndexOf('/') + 1;
            string pluginName = sourceFileName.Substring(nameindex);
            Debug.Log("Installing " + pluginName);

            File.Move(sourceFileName, destFileName);
            return true;
        }
    }
}