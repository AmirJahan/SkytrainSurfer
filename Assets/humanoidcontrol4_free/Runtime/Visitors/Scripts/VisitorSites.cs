using System.IO;
using System.Collections;
using System.Collections.Generic;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Passer {

    public class VisitorSites : MonoBehaviour {
        protected static string filePath {
            get {
                string filePath = Path.Combine(Application.persistentDataPath, "MySites.json");
                return filePath;
            }
        }

        [System.Serializable]
        public class Site {
            public string name;
            public string siteLocation;
        }

        [System.Serializable]
        public class SiteList : IEnumerator, IEnumerable {
            public List<Site> list = new List<Site>();

            public void Add(Site site) {
                list.Add(site);
            }
            public void Remove(Site site) {
                list.Remove(site);
            }

            public Site Find(System.Predicate<Site> match) {
                return list.Find(match);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return list.GetEnumerator();
            }

            bool IEnumerator.MoveNext() {
                IEnumerator enumerator = list.GetEnumerator();
                return enumerator.MoveNext();
            }

            void IEnumerator.Reset() {
                IEnumerator enumerator = list.GetEnumerator();
                enumerator.Reset();
            }

            object IEnumerator.Current {
                get {
                    IEnumerator enumerator = list.GetEnumerator();
                    return enumerator.Current;
                }
            }
        }

        public static SiteList GetSiteList() {
            SiteList sites = null;

            Debug.Log("GetSiteList: path " + filePath);
            if (File.Exists(filePath)) {
                string json = File.ReadAllText(filePath);
                SiteList readSites = JsonUtility.FromJson<SiteList>(json);
                sites = readSites;
            }
            if (sites == null)
                sites = new SiteList();

            return sites;
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void SyncFiles();
#endif

        public static void SaveSiteList(SiteList sites) {
            try {
                Debug.Log("SaveSiteList: path " + filePath);
                string json = JsonUtility.ToJson(sites);
                Debug.Log(json);
                if (!File.Exists(filePath)) {
                    Debug.Log("Create sitslist");
                    File.Create(filePath).Dispose();
                }
                File.WriteAllText(filePath, json);
#if UNITY_WEBGL
                Debug.Log("Sync Files");
                SyncFiles();
#endif
                Debug.Log("Save complete");
            }
            catch (System.Exception e) {
                Debug.LogError("Save Sitelist failed: " + e);
            }
        }
    }
}