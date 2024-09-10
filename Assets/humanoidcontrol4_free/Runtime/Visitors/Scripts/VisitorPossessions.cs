using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using Passer.Humanoid;

namespace Passer {

    /// <summary>
    /// The Possession of a Humanoid Visitor
    /// </summary>
    /// 
    /// \image html PawnPossessionsInspector.png
    /// \image rtf PawnPossessionsInspector.png
    /// 
    /// * Default possessions, see PawnPossessions::defaultPossessions
    /// * Clear at start, see PawnPossessions::clearAtStart
    /// \version 4.0 and higher
    public class VisitorPossessions : MonoBehaviour {

        [System.Serializable]
        public class Possession {
            public string name;
            public Possessable.Type type;
            public bool persistent;
            public bool removable = true;
            public string siteLocation;
            public string assetPath;
            [System.NonSerialized]
            public Possessable scenePossession;
        }

        #region Locals 

        protected class CachedPossessionBundle {
            public string siteLocation;
            public AssetBundle assetBundle;

            public static CachedPossessionBundle Find(string possessionLocation) {
                foreach (CachedPossessionBundle bundle in bundleCache) {
                    if (bundle.siteLocation == possessionLocation && bundle.assetBundle != null)
                        return bundle;
                }
                return null;
            }
        }

        [System.Serializable]
        protected class CachedPossession {
            public CachedPossessionBundle possessionBundle;
            public string assetPath;
            public GameObject possession;
            public bool preserved = false; // when the possession was a scenePossession which is preseved

            public static void Update(CachedPossessionBundle cachedBundle, string possessablePath, GameObject prefab) {
                CachedPossession foundPossession = cache.Find(entry =>
                    entry.assetPath == possessablePath
                );
                if (foundPossession == null) {
                    CachedPossession cachedPossession = new CachedPossession() {
                        possessionBundle = cachedBundle,
                        assetPath = possessablePath,
                        possession = prefab,
                    };
                    cache.Add(cachedPossession);
                }
                else {
                    foundPossession.possession = prefab;
                }
            }
        }

        protected static List<CachedPossessionBundle> bundleCache = new List<CachedPossessionBundle>();
        protected static List<CachedPossession> cache = new List<CachedPossession>();

        #endregion

        public class Possessions {
            public List<Possession> list = new List<Possession>();
        }

        //// May include an amount later
        //public static Possessions possessions;

        public List<Possession> possessions;
        public List<Possession> myPossessions {
            get {
                return possessions;
            }
        }


        /// <summary>
        /// The possessions which are available from the start
        /// </summary>
        public Possessable[] defaultPossessions;
        /// <summary>
        /// Clear the possessions when the application is started
        /// </summary>
        /// Default possessions will not be cleared.
        /// This works only on real Humanoids, not on HumanoidInterfaces/Sites
        public bool clearOnAwake;

        private string filePath {
            get {
                string filePath = Path.Combine(Application.persistentDataPath, "MyPossessions.json");
                return filePath;
            }
        }

        #region Init

        protected virtual void Awake() {
            // Only do this for real humanoids
            HumanoidControl humanoid = GetComponentInParent<HumanoidControl>();
            if (humanoid != null) {
                if (!clearOnAwake) {
                    //Debug.Log("Retrieve possessions from " + filePath);

                    if (File.Exists(filePath)) {
                        string json = File.ReadAllText(filePath);
                        Possessions possessionList = JsonUtility.FromJson<Possessions>(json);
                        possessions = possessionList.list; //JsonUtility.FromJson<Possessions>(json);
                    }
                    else {
                        Debug.Log("Cleared Possessions");
                        possessions = new List<Possession>();//new Possessions();
                    }
                }
                else {
                    Debug.Log("Cleared Possessions");
                    possessions = new List<Possession>(); // new Possessions();
                }

                if (humanoid.avatarRig != null) {
                    Possessable possessableAvatar = humanoid.avatarRig.GetComponent<Possessable>();
                    if (possessableAvatar != null)
                        AddPossessions(new Possessable[] { possessableAvatar }, true);
                }
            }

            // Default Possessions cannot be deleted
            AddPossessions(defaultPossessions, false);

        }

        #endregion

        #region Stop

        protected virtual void OnDestroy() {
            //Debug.Log("Store possessions to " + filePath);

            string json = JsonUtility.ToJson(possessions);
            File.WriteAllText(filePath, json);
            //Debug.Log("Possesions stored");
        }

        public static void DestroyScenePossessions() {
            //possessions.list.RemoveAll(poss => poss.persistent == false);
        }

        #endregion

        /// <summary>
        /// Try to add the gameObject to this humanoidpossessions.
        /// This only succeeds whent the hameObject has a Possessable component.
        /// </summary>
        /// <param name="gameObject">The GameObject to add</param>
        public void TryAddGameObject(GameObject gameObject) {
            Possessable possession = gameObject.GetComponentInChildren<Possessable>();
            if (possession == null)
                return;

            Add(possession);
        }

        public void AddPossessions(Possessable[] scenePossessions, bool removable = true) {
            if (possessions == null || scenePossessions == null)
                return;

            foreach (Possessable possession in scenePossessions) {
                if (possession == null)
                    continue;

                Possession persistentPossession = Add(possession, true);
                //persistentPossession.scenePossession = PreservePossession(possession);
                persistentPossession.persistent = possession.crossSite;
                persistentPossession.removable = removable;
            }
        }

        private static Possessable PreservePossession(Possessable possession) {
            // Keep the possession as an disabled object for later reference
            // Note: this does not work for networked setups!!!
            GameObject preservedPossession = Instantiate(possession.gameObject);
            preservedPossession.SetActive(false);
            Object.DontDestroyOnLoad(preservedPossession);
            return preservedPossession.GetComponent<Possessable>();
        }

        /// <summary>
        /// Add the possessable object to the humanoidPossessions
        /// </summary>
        /// <param name="possessable">The possessable object to add</param>
        /// <returns>The persistent possession</returns>
        public Possession Add(Possessable possessable, bool preserved = false) {
            if (possessable == null)
                return null;

            if (possessable.isUnique) {
                Possession foundPossession = possessions.Find(
                    persistentPossession => persistentPossession.assetPath == possessable.assetPath
                    );
                if (foundPossession != null)
                    return foundPossession;
            }

            Possession newPossession = new Possession() {
                name = possessable.name,
                siteLocation = possessable.siteLocation,
                assetPath = possessable.assetPath,
                type = possessable.possessionType,
            };
            if (preserved)
                newPossession.scenePossession = PreservePossession(possessable);

            possessions.Add(newPossession);

            CachedPossessionBundle possessionBundle = CachedPossessionBundle.Find(possessable.siteLocation);

            CachedPossession cachedPossession = new CachedPossession() {
                assetPath = possessable.assetPath,
                possessionBundle = possessionBundle,
                possession = preserved ? newPossession.scenePossession.gameObject : possessable.gameObject,
                preserved = preserved,
            };
            cache.Add(cachedPossession);

            Debug.Log("Possession cache: ");
            foreach (CachedPossession poss in cache)
                Debug.Log(" * " + poss.assetPath + " || " + poss.possessionBundle + " || " + poss.possession + " || " + poss.preserved);

            Debug.Log("cache: ");
            foreach (CachedPossessionBundle pos in bundleCache) {
                Debug.Log(" - " + pos.siteLocation + " || " + pos.assetBundle);
            }

            return newPossession;
        }

        public void DeletePossession(Possession possession) {
            Debug.Log("deleting " + possession);
            Possession foundPossession = possessions.Find(
                persistentPossession => persistentPossession.assetPath == possession.assetPath
                );
            if (foundPossession == null)
                return;

            possessions.Remove(foundPossession);
            Debug.Log("deleted");
        }

        private static AssetBundle lastAssetBundle;

        public static IEnumerator RetrievePossessionAsync(Possession possession, System.Action<GameObject> callback) {
            Debug.Log("Possession cache: ");
            foreach (CachedPossession poss in cache)
                Debug.Log(" * " + poss.assetPath + " || " + poss.possessionBundle + " || " + poss.possession + " || " + poss.preserved);

            Debug.Log("cache: ");
            foreach (CachedPossessionBundle pos in bundleCache) {
                Debug.Log(" - " + pos.siteLocation + " || " + pos.assetBundle);
            }

            if (possession.siteLocation == "") {
                Debug.Log("Get scene possession");
                GameObject prefab = possession.scenePossession.gameObject;
                prefab.SetActive(true);
                callback(prefab);
                prefab.SetActive(false);
                yield return null;
            }
            else
                yield return RetrievePossessableAsync(possession.siteLocation, possession.assetPath, callback);
        }

        public static IEnumerator RetrievePossessableAsync(string possessableLocation, string possessablePath, System.Action<GameObject> callback) {
            GameObject prefab;

            if (possessableLocation == "") {
                CachedPossession foundPossession = cache.Find(entry => entry.assetPath == possessablePath);
                if (foundPossession == null) {
                    Debug.Log("Cannot retrieve Possessable: location is not set");
                    callback(null);
                }
                else {
                    //Debug.Log("Load from cache: " + possessablePath);
                    prefab = foundPossession.possession;
                    if (prefab == null) {
                        Debug.LogError("Could not load " + possessablePath);
                        callback(null);
                    }
                    else
                        callback(prefab);
                }
            }
            else {
                CachedPossession foundPossession = cache.Find(entry => entry.assetPath == possessablePath);
                if (foundPossession != null && foundPossession.possession != null) {
                    //Debug.Log("Load from cache: " + foundPossession.possession);
                    prefab = foundPossession.possession;
                    if (foundPossession.preserved) {
                        //Debug.Log("preserved possession");
                        prefab.SetActive(true);
                        callback(prefab);
                        prefab.SetActive(false);
                    }
                    else
                        callback(prefab);
                    yield break;
                }

                CachedPossessionBundle foundPossessionBundle = bundleCache.Find(entry => entry.siteLocation == possessableLocation);
                if (foundPossessionBundle == null) {
#if UNITY_ANDROID
                    string url = "https://" + possessableLocation + ".android.site";
#elif UNITY_WEBGL
                    string url = "https://" + possessableLocation + ".webgl.site";
#else
                    string url = "https://" + possessableLocation + ".windows.site";
#endif
                    //Debug.Log("Loading possession: " + url);

                    UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
                    yield return request.SendWebRequest();

                    AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
                    if (assetBundle == null) {
                        Debug.LogError("Could not load " + url);
                        yield break;
                    }

                    //Debug.Log("Load: " + possessablePath);
                    prefab = LoadPossessableFromAssetBundle(assetBundle, possessablePath);
                    if (prefab == null) {
                        Debug.LogError("Could not load " + possessablePath);
                        yield break;
                    }

                    CachedPossessionBundle cachedBundle = new CachedPossessionBundle() {
                        siteLocation = possessableLocation,
                        assetBundle = assetBundle,
                    };
                    bundleCache.Add(cachedBundle);

                    CachedPossession.Update(cachedBundle, possessablePath, prefab);
                }
                else {
                    Debug.Log("Load: " + possessablePath);
                    prefab = LoadPossessableFromAssetBundle(foundPossessionBundle.assetBundle, possessablePath);
                    if (prefab == null) {
                        Debug.LogError("Could not load " + possessablePath);
                        yield break;
                    }

                    CachedPossession.Update(foundPossessionBundle, possessablePath, prefab);
                }
                callback(prefab);
            }
        }

        private static GameObject LoadPossessableFromAssetBundle(AssetBundle assetBundle, string possessablePath) {
            string possessableName = possessablePath;
            int lastSlashIx = possessablePath.LastIndexOf('/');
            if (lastSlashIx >= 0)
                possessableName = possessablePath.Substring(lastSlashIx + 1);

            possessableName = possessableName.ToLower();

            GameObject prefab = assetBundle.LoadAsset<GameObject>(possessableName);
            return prefab;
        }

        public static void UnloadPossession() {
            if (lastAssetBundle != null)
                lastAssetBundle.Unload(true);
        }


        private int currentAvatarIndex = 0;
        public void UseNextAvatar() {
            List<Possession> avatars = possessions.FindAll(possession => possession.type == Possessable.Type.Avatar);
            if (avatars.Count == 0)
                return;

            currentAvatarIndex = mod(currentAvatarIndex + 1, avatars.Count);
            UseAvatar(currentAvatarIndex);
        }

        public void UseAvatar(int avatarIndex) {
            List<Possession> avatars = possessions.FindAll(possession => possession.type == Possessable.Type.Avatar);
            if (avatarIndex < 0 || avatarIndex > avatars.Count)
                return;

            HumanoidControl humanoid = FindObjectOfType<HumanoidControl>();
            if (humanoid == null)
                return;


            if (avatars[avatarIndex] != null) {
                if (avatars[avatarIndex].scenePossession != null)
                    humanoid.ChangeAvatar(avatars[avatarIndex].scenePossession.gameObject);
                else
                    StartCoroutine(RetrieveAvatarAsync(avatars[avatarIndex]));
            }
        }

        private static IEnumerator RetrieveAvatarAsync(Possession possession) {
            HumanoidControl humanoid = FindObjectOfType<HumanoidControl>();
            if (humanoid == null)
                yield break;

            string url = "https://" + possession.siteLocation + ".windows" + ".site";
            Debug.Log("Loading possession: " + url);

            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return request.SendWebRequest();

            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            if (assetBundle == null) {
                Debug.LogError("Could not load " + url);
                yield break;
            }

            GameObject avatarPrefab = assetBundle.LoadAsset<GameObject>(possession.assetPath);
            if (avatarPrefab != null)
                humanoid.ChangeAvatar(avatarPrefab);
        }

        public static int mod(int k, int n) {
            k %= n;
            return (k < 0) ? k + n : k;
        }
    }

}