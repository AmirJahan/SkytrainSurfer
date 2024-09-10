using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Passer {

    /// <summary>
    /// Possessions can be owned by an Humanoid
    /// </summary>
    /// 
    /// \image html PossessionInspector.png
    /// \image rtf PossessionInspector.png
    /// 
    /// * %Possession type, see Possession::possessionType
	/// * %Cross site allowed, see Possession::crossSite
	/// * %Is unique, see Possession::isUnique
    /// 
    /// \version 4
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_possessable.html")]
    public class Possessable : MonoBehaviour {

        /// <summary>
        /// The possession type
        /// </summary>
        public enum Type {
            Generic, ///< A generic Possession
            Avatar, ///< An avatar can be worn by a Humanoid
        }
        /// <summary>
        /// The Type of Possession
        /// </summary>
        public Type possessionType;

        /// <summary>
        /// If true, this Posession can be taken to other Sites.
        /// </summary>
        /// Non cross site possessions will be removed from the Humanoid's possessions
        /// when they leave the site.
        public bool crossSite = true;

        /// <summary>
        /// An unique Possession can be possessed only once.
        /// </summary>
        public bool isUnique = false;

        [SerializeField]
        private string _siteLocation;
        public string siteLocation {
            get {
                if (_siteLocation == null)
                    DetermineSiteLocation();

                return _siteLocation;
            }
        }
        private void DetermineSiteLocation() {
            if (SiteNavigator.currentSite == null)
                _siteLocation = "";
            else {
                string siteLocation = SiteNavigator.currentSite.siteLocation;
                _siteLocation = (siteLocation + "_possessions");
            }
        }

        protected virtual void Awake() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (string.IsNullOrEmpty(_siteLocation))
                DetermineSiteLocation();
        }

        public string assetPath;
    }
}