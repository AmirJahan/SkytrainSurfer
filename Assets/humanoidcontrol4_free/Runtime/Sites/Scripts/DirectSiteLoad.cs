using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Passer {

    public class DirectSiteLoad : MonoBehaviour {

        public SiteNavigator siteNavigator;
        public void LoadSiteFromURL(InputField inputField) {
            if (siteNavigator == null)
                return;

            siteNavigator.LoadSiteFromURL(inputField.text);
            inputField.Select();
        }
    }
}