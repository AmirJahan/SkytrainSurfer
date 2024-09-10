using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Passer {

    public class SiteDetailsUI : MonoBehaviour {
        public SitesUI sitesUI;

        public Text nameUI;
        public Button goUI;
        public Button deleteUI;

        protected VisitorSites.Site site;

        public void SetSite(VisitorSites.Site site) {
            this.site = site;
        }

        private void OnEnable() {
            if (site == null)
                return;

            nameUI.text = site.name;
            goUI.onClick.AddListener(() => GoToSite());
            deleteUI.onClick.AddListener(() => DeleteSite());
        }

        private void GoToSite() {
            SiteNavigator siteNavigator = FindObjectOfType<SiteNavigator>();
            if (siteNavigator == null) {
                Debug.LogError("Could not find a site navigator");
                return;
            }

            sitesUI.transform.parent.gameObject.SetActive(false);
            siteNavigator.LoadSiteFromURL(site.siteLocation);
        }

        private void DeleteSite() {
            //			sitesUI.DeleteSite(site);
            this.gameObject.SetActive(false);
            sitesUI.gameObject.SetActive(true);
        }
    }
}