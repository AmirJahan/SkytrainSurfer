using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Passer {

    /// <summary>
    /// The component which takes care of site navigation
    /// </summary>
    public class SiteNavigator : MonoBehaviour {

        #region Properties

        public bool loadSiteAtStart = false;
        /// <summary>
        /// The URL of the Site which should be visited at start
        /// </summary>
        /// The URL should not include a protocol (like https://) or an extension (like .site or .windows.site)
        public string startSite = "";
#if UNITY_EDITOR
        /// <summary>
        /// The name of the scene which should be visited at start
        /// </summary>
        /// This overrides the startSite but only works in the editor
        public string startScene;
#endif

        [System.Serializable]
        public class HistoryEntry {
            public string siteName;
            public string siteLocation;
            public string scenePath;
            public AssetBundle assetBundle;
        }

        protected static List<HistoryEntry> cache = new List<HistoryEntry>();
        public static Stack<HistoryEntry> history = new Stack<HistoryEntry>();
        public static HistoryEntry currentSite = null;
        protected static AssetBundle currentAssetBundle = null;
        protected static AssetBundle loadedAssetBundle = null;

        #endregion Properties

        #region Init

        static SiteNavigator _instance;
        public static SiteNavigator instance {
            get {
                if (_instance == null) {
                    Debug.LogWarning("No SiteNavigator instance found, instantiating a new instance...");
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<SiteNavigator>();
                }
                return _instance;
            }
        }


        protected virtual void Awake() {
            if (_instance == null) {
            // Bit of a hack, find the sitenavigator outside the menu...
                SiteNavigator parentSiteNavigator = GetComponentInParent<SiteNavigator>();
                if (parentSiteNavigator != null)
                    _instance = parentSiteNavigator;
                else
                    _instance = this;
            }

            if (loadSiteAtStart)
                LoadStartSiteFromStartupJSON();
        }

        protected void LoadStartSiteFromStartupJSON() {
            VisitorConfiguration configuration = VisitorConfiguration.configuration;
            string configurationStartSite = configuration.startSite;
            if (configurationStartSite != null && configurationStartSite != "")
                startSite = configurationStartSite;
        }

        protected virtual void Start() {
#if UNITY_EDITOR
            if (startScene != null && startScene != "" && startScene != "-none-") {
                Debug.Log("Load scene " + startScene);
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(startScene, new LoadSceneParameters(LoadSceneMode.Additive));
                currentSite = new HistoryEntry() {
                    siteName = "directload",
                    siteLocation = "serrarens.nl/sites/start",
                    scenePath = "",
                    assetBundle = currentAssetBundle,
                };
            }
            else
#endif
            if (startSite != null && startSite != "") {
                Debug.Log(this.transform.root + " start site[" + startSite + "]");
                LoadSiteFromURL(startSite);
            }
        }

        #endregion Init

        #region Update

        protected void Update() {
            if (loadedAssetBundle != currentAssetBundle) {
                if (loadedAssetBundle != null) {
                    Debug.Log("unload " + loadedAssetBundle.name);
                    loadedAssetBundle.Unload(false);
                }

                loadedAssetBundle = currentAssetBundle;
            }
        }

        #endregion Update

        public void GoHome() {
            if (VisitorConfiguration.configuration != null &&
                VisitorConfiguration.configuration.startSite != null &&
                VisitorConfiguration.configuration.startSite != ""
                ) {

                LoadSiteFromURL(VisitorConfiguration.configuration.startSite);
            }
        }


        /// <summary>
        /// Change the current Site
        /// </summary>
        /// <param name="siteLocation">The URL of the site to visit</param>
        /// The URL  should not include a protocol (like https://) or an extension (like .site or .windows.site)
        public void LoadSiteFromURL(string siteLocation) {
            SiteNavigator.instance.StartCoroutine(LoadSite(siteLocation));
        }

        protected static IEnumerator LoadSite(string siteLocation) {
            int slashPos = siteLocation.LastIndexOf('/');
            string siteName = siteLocation.Substring(slashPos + 1);

            if (currentSite != null) {
                if (slashPos < 0) {
                    int currentSlashPos = currentSite.siteLocation.LastIndexOf('/');
                    string currentPath = currentSite.siteLocation.Substring(0, currentSlashPos + 1);
                    siteLocation = currentPath + siteLocation;
                }

                // If we reload the current site,
                // unload the asset bundle for the current site
                if (loadedAssetBundle != null &&
                    siteName == currentSite.siteName)
                    //siteLocation == currentSite.siteLocation)
                    loadedAssetBundle.Unload(false);
            }

            string scenePath = null;

            HistoryEntry foundEntry = cache.Find(entry => entry.siteLocation == siteLocation);
            // cache is disabled because we unload the asset bundles
            foundEntry = null;
            if (foundEntry == null) {
#if UNITY_ANDROID
                string url = "https://" + siteLocation + ".android" + ".site";
#elif UNITY_WEBGL
                string url = "https://" + siteLocation + ".webgl" + ".site";
#else
                string url = "https://" + siteLocation + ".windows" + ".site";
#endif

                //ShowLoadingDialog();
                GameObject dialog = ShowDialog("Loading...");

                Debug.Log("Loading site: " + url);
                UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
                yield return request.SendWebRequest();

                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
                if (assetBundle != null) {
                    //Debug.Log("Retreived assetBundle");
                    DestroyAllSocketAttachments();
                    yield return null;

                    string[] scenePaths = assetBundle.GetAllScenePaths();
                    scenePath = scenePaths[0];
                    HistoryEntry entry = new HistoryEntry() {
                        siteLocation = siteLocation,
                        scenePath = scenePaths[0],
                        assetBundle = assetBundle,
                    };
                    history.Push(entry);
                    cache.Add(entry);

                    currentAssetBundle = assetBundle;
                    CloseDialog(dialog);
                }
                else {
                    CloseDialog(dialog);
                    Debug.LogError("Could not load " + url);
                    dialog = ShowDialog("Coud not load " + url);
                    yield return new WaitForSeconds(3);
                    CloseDialog(dialog);
                    yield break;
                }
            }
            else {
                Debug.Log("Unloading " + currentAssetBundle);
                currentAssetBundle.Unload(true);

                scenePath = foundEntry.scenePath;
                history.Push(foundEntry);
            }

            //string siteName = scenePath.Substring(scenePath.LastIndexOf("/") + 1);
            //siteName = siteName.Substring(0, siteName.IndexOf("."));
            currentSite = new HistoryEntry() {
                siteName = siteName,
                siteLocation = siteLocation,
                scenePath = scenePath,
                assetBundle = currentAssetBundle,
            };

            //Debug.Log("Loadin new scene: " + scenePath);
            UnityEngine.SceneManagement.SceneManager.LoadScene(scenePath);
        }

        //private static GameObject ShowLoadingDialog() {
        //    GameObject dialogObj = new GameObject();

        //    Canvas canvas = dialogObj.AddComponent<Canvas>();
        //    canvas.renderMode = RenderMode.ScreenSpaceCamera;

        //    UnityEngine.UI.Text text = dialogObj.AddComponent<UnityEngine.UI.Text>();
        //    text.text = "Loading...";
        //    text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        //    text.resizeTextForBestFit = true;
        //    text.alignment = TextAnchor.MiddleCenter;
        //    return dialogObj;
        //}

        private static GameObject ShowDialog(string dialogText) {
            GameObject dialogObj = new GameObject();

            Canvas canvas = dialogObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            UnityEngine.UI.Text text = dialogObj.AddComponent<UnityEngine.UI.Text>();
            text.text = dialogText;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.resizeTextForBestFit = true;
            text.alignment = TextAnchor.MiddleCenter;

            return dialogObj;
        }

        private static void CloseDialog(GameObject dialogObj) {
            Destroy(dialogObj);
        }

        public void ReloadSite() {
            if (currentSite != null) {
                if (currentAssetBundle != null) {
                    Debug.Log("unload " + currentAssetBundle.name);
                    currentAssetBundle.Unload(false);
                }
                LoadSiteFromURL(currentSite.siteLocation);
            }
        }

        private static void DestroyAllSocketAttachments() {
            Socket[] sockets = Resources.FindObjectsOfTypeAll<Socket>();
            foreach (Socket socket in sockets) {

                // This should end up in Socket/OnSceneUnload, but only when Humanoid 4 is branched
                // Because Possession is not part of Humanoid 3
                if (socket.attachedTransform != null) {
                    Possessable possession = socket.attachedTransform.GetComponent<Possessable>();
                    if (possession != null) {
                        // possesions are not destroyed
                        continue;
                    }
                }
                // end code for Socket.cs

                socket.destroyOnLoad = true;
            }
        }

        protected static IEnumerator UnloadLastAssetBundle(AssetBundle newAssetBundle) {
            Debug.Log("Wait");
            yield return new WaitForSeconds(1);

            Debug.Log("Unload");
            if (currentAssetBundle != null)
                currentAssetBundle.Unload(true);

            currentAssetBundle = newAssetBundle;
            yield return null;
        }

        public void GoBack() {
            if (history == null || history.Count <= 1) {
                Debug.Log("No previous site, quitting.");
                Quit();
                return;
            }

            history.Pop(); // pop the current site
            string siteLocation = history.Peek().siteLocation; // get the previous site            
            Debug.Log("going back to " + siteLocation);
            if (siteLocation == null || siteLocation == "") {
                Debug.Log("Could not find previous site, quitting.");
                Quit();
                return;
            }

            LoadSiteFromURL(siteLocation);
        }

        public void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}