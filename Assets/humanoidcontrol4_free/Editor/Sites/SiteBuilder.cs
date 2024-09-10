using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

namespace Passer {

    /// <summary>
    /// A component for building \ref Site "Humanoid Sites"
    /// </summary>
    /// 
    /// Sites can be built by selecting the _File->Build Sites_ menu.
    /// After this you will be able to select which sites will be built
    /// by selecting the appropriate sites in the dialog:
    /// 
    /// \image html BuildSitesDialog.png
    /// \image rtf BuildSitesDialog.png
    /// 
    /// When the Build button is pressed, all sites will be build and become available in the Assets/SiteBuilds folder
    /// with a submap for each platform (like Windows, Android...)
    /// 
    /// When the Build to Folder is pressed, the built sites will additionally be copied to the selected target folder.
    /// This enables you to directly publish to a web site for instance.
    /// 
    /// \version 4 and higher
    [InitializeOnLoad]
    public class SiteBuilder {

        public static Sites siteBuilds = null;

        private class SiteBuilderWindow : EditorWindow {
            [MenuItem("File/Build Sites", false, 100)] // somehow priority does not work in File Menu?
            public static void OpenWindow() {
                GenerateSiteBuilds();
                SiteBuilderWindow window = (SiteBuilderWindow)EditorWindow.GetWindow(typeof(SiteBuilderWindow), true, "Build Sites");
                window.ShowUtility();
            }

            private Vector2 scrollPos;

            private void OnGUI() {
                EditorGUILayout.LabelField("Sites in Build", EditorStyles.boldLabel);

                scrollPos =
                    EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

                for (int i = 0; i < siteBuilds.list.Count; i++) {
                    siteBuilds.list[i].enabled = EditorGUILayout.ToggleLeft(siteBuilds.list[i].siteName, siteBuilds.list[i].enabled);
                }
                EditorGUILayout.EndScrollView();


                bool buildSites = false;
                bool buildToFolder = false;
                using (new GUILayout.HorizontalScope()) {
                    GUILayout.FlexibleSpace();
                    buildSites = GUILayout.Button("Build", GUILayout.Width(110));
                    buildToFolder = GUILayout.Button("Build to Folder", GUILayout.Width(110));
                }

                if (buildSites) BuildSites("");
                if (buildToFolder) StartBuildToFolder();
            }
        }

        public static void GenerateSiteBuilds() {
            string[] assetbundles = AssetDatabase.GetAllAssetBundleNames();

            if (assetbundles.Length > 0) {
                if (siteBuilds == null) {
                    siteBuilds = AssetDatabase.LoadAssetAtPath<Sites>("Assets/SiteBuilds/SiteList.asset");
                    if (siteBuilds == null) {
                        if (!Directory.Exists("Assets/SiteBuilds"))
                            Directory.CreateDirectory("Assets/SiteBuilds");

                        siteBuilds = (Sites)ScriptableObject.CreateInstance(typeof(Sites));
                        AssetDatabase.CreateAsset(siteBuilds, "Assets/SiteBuilds/SiteList.asset");
                    }
                }
            }
            bool hasChanged = false;

            for (int i = 0; i < assetbundles.Length; i++) {
                PrepareSite(assetbundles[i]);

                Sites.SiteBuild siteBuild = siteBuilds.list.Find(sb => sb.siteName == assetbundles[i]);
                if (siteBuild == null) {
                    siteBuild = new Sites.SiteBuild() {
                        siteName = assetbundles[i],
                        enabled = true,
                    };
                    siteBuilds.list.Add(siteBuild);
                    hasChanged = true;
                }
            }
            if (hasChanged)
                AssetDatabase.SaveAssets();
        }

        private static void PrepareSite(string sitename) {
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(sitename, sitename);
            if (paths.Length == 0)
                return;

            Scene scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(paths[0]);
            if (scene == null) {
                Debug.Log("Could not load " + sitename);
                return;
            }
            Debug.Log("Prepare " + sitename);
            NetworkObject_Check.CheckNetworkObjects();
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        private static void StartBuildToFolder() {
            string savePath = EditorUtility.SaveFolderPanel("Target folder", "", "");
            BuildSites(savePath);
        }


        /// <summary>
        /// Build all sites in this project and save them in the indicated location
        /// </summary>
        /// Currently this function will build all sites for Windows and Android platforms only.
        /// <param name="savePath">The full path to the folder in which the sites should be saved</param>
        public static void BuildSites(string savePath) {
            BuildWindowsSites(savePath);
            BuildAndroidSites(savePath);
            BuildWebGLSites(savePath);
        }

        /// <summary>
        /// Build all sites for the Windows Standalone platform in this project and save them in the indicated location
        /// </summary>
        /// <param name="savePath">The full path to the folder in which the sites should be saved</param>
        public static void BuildWindowsSites(string savePath) {
            Debug.Log("[SiteBuilder] Building Windows Sites");
            System.Console.WriteLine("[SiteBuilder] Building Windows Sites");
            string assetBundleDirectory = "Assets/SiteBuilds/Windows";
            if (Directory.Exists(assetBundleDirectory))
                Directory.Delete(assetBundleDirectory, true);
            if (!Directory.Exists(assetBundleDirectory))
                Directory.CreateDirectory(assetBundleDirectory);

            AssetBundleBuild[] buildMap = GetBuildmap();
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            string[] filePaths = Directory.GetFiles(assetBundleDirectory);
            foreach (string filePath in filePaths) {
                if (!filePath.Contains(".")) {
                    if (filePath.Substring(filePath.Length - 8) == "\\Windows" ||
                        filePath.Substring(filePath.Length - 17) == "\\Windows.manifest") {
                        File.Delete(filePath);
                        File.Delete(filePath + ".meta");
                        continue;
                    }
                    Debug.Log("Built " + filePath);

                    // Rename assetbundles to include platform
                    string newFilePath = filePath + ".windows.site";
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);
                    File.Move(filePath, newFilePath);
                    // Rename .meta
                    string newMetaPath = newFilePath + ".meta";
                    string metaPath = filePath + ".meta";
                    if (File.Exists(newMetaPath))
                        File.Delete(newMetaPath);
                    if (File.Exists(metaPath))
                        File.Move(metaPath, newMetaPath);

                    if (savePath != "") {
                        int slashPos = newFilePath.LastIndexOf("\\");
                        string fileName = newFilePath.Substring(slashPos + 1);
                        string saveFilePath = savePath + "/" + fileName;
                        System.Console.WriteLine("[SiteBuilder] Copy " + newFilePath + " to " + saveFilePath);
                        File.Copy(newFilePath, saveFilePath, true);
                    }
                }

            }
        }

        /// <summary>
        /// Build all sites for the Android platform in this project and save them in the indicated location
        /// </summary>
        /// <param name="savePath">The full path to the folder in which the sites should be saved</param>
        public static void BuildAndroidSites(string savePath) {

            Debug.Log("[SiteBuilder] Building Android Sites");
            System.Console.WriteLine("[SiteBuilder] Building Android Sites");
            string assetBundleDirectory = "Assets/SiteBuilds/Android";
            if (Directory.Exists(assetBundleDirectory))
                Directory.Delete(assetBundleDirectory, true);
            if (!Directory.Exists(assetBundleDirectory))
                Directory.CreateDirectory(assetBundleDirectory);

            AssetBundleBuild[] buildMap = GetBuildmap();
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);

            string[] filePaths = Directory.GetFiles(assetBundleDirectory);
            foreach (string filePath in filePaths) {
                if (!filePath.Contains(".")) {
                    if (filePath.Substring(filePath.Length - 8) == "\\Android" ||
                        filePath.Substring(filePath.Length - 17) == "\\Android.manifest") {
                        File.Delete(filePath);
                        File.Delete(filePath + ".meta");
                        continue;
                    }

                    Debug.Log("Built " + filePath);

                    string newFilePath = filePath + ".android.site";
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);
                    File.Move(filePath, newFilePath);
                    // Rename .meta
                    string newMetaPath = newFilePath + ".meta";
                    string metaPath = filePath + ".meta";
                    if (File.Exists(newMetaPath))
                        File.Delete(newMetaPath);
                    if (File.Exists(metaPath))
                        File.Move(metaPath, newMetaPath);

                    if (savePath != "") {
                        int slashPos = newFilePath.LastIndexOf("\\");
                        string fileName = newFilePath.Substring(slashPos + 1);
                        string saveFilePath = savePath + "/" + fileName;
                        System.Console.WriteLine("[SiteBuilder] Copy " + newFilePath + " to " + saveFilePath);
                        File.Copy(newFilePath, saveFilePath, true);
                    }
                }
            }
        }

        /// <summary>
        /// Build all sites for the WebGL platform in this project and save them in the indicated location
        /// </summary>
        /// <param name="savePath">The full path to the folder in which the sites should be saved</param>
        public static void BuildWebGLSites(string savePath) {
            Debug.Log("[SiteBuilder] Building WebGL Sites");
            System.Console.WriteLine("[SiteBuilder] Building WebGL Sites");
            string assetBundleDirectory = "Assets/SiteBuilds/WebGL";
            if (Directory.Exists(assetBundleDirectory))
                Directory.Delete(assetBundleDirectory, true);
            if (!Directory.Exists(assetBundleDirectory))
                Directory.CreateDirectory(assetBundleDirectory);

            AssetBundleBuild[] buildMap = GetBuildmap();
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, BuildTarget.WebGL);

            string[] filePaths = Directory.GetFiles(assetBundleDirectory);
            foreach (string filePath in filePaths) {
                if (!filePath.Contains(".")) {
                    if (filePath.Substring(filePath.Length - 6) == "\\WebGL" ||
                        filePath.Substring(filePath.Length - 15) == "\\WebGL.manifest") {
                        File.Delete(filePath);
                        File.Delete(filePath + ".meta");
                        continue;
                    }

                    Debug.Log("Built " + filePath);

                    string newFilePath = filePath + ".webgl.site";
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);
                    File.Move(filePath, newFilePath);
                    // Rename .meta
                    string newMetaPath = newFilePath + ".meta";
                    string metaPath = filePath + ".meta";
                    if (File.Exists(newMetaPath))
                        File.Delete(newMetaPath);
                    if (File.Exists(metaPath))
                        File.Move(metaPath, newMetaPath);

                    if (savePath != "") {
                        int slashPos = newFilePath.LastIndexOf("\\");
                        string fileName = newFilePath.Substring(slashPos + 1);
                        string saveFilePath = savePath + "/" + fileName;
                        System.Console.WriteLine("[SiteBuilder] Copy " + newFilePath + " to " + saveFilePath);
                        File.Copy(newFilePath, saveFilePath, true);
                    }
                }
            }
        }

        protected static AssetBundleBuild[] GetBuildmap() {
            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();

            for (int i = 0; i < siteBuilds.list.Count; i++) {
                if (siteBuilds.list[i].enabled == false)
                    continue;

                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = siteBuilds.list[i].siteName;

                string[] AssetsInBundle = AssetDatabase.GetAssetPathsFromAssetBundle(build.assetBundleName);
                build.assetNames = AssetsInBundle;

                buildList.Add(build);
            }

            AssetBundleBuild[] buildMap = buildList.ToArray();
            return buildMap;
        }
    }

}