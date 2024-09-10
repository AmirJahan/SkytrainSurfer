using UnityEngine;
using UnityEngine.UI;

namespace Passer {

    public class SitesUI : MonoBehaviour {

        public VisitorSites.SiteList sites;

        private Canvas canvas;

        public GameObject mainMenuUI;
        public SiteDetailsUI siteDetailsUI;

        #region Init

        private void Awake() {
            canvas = GetComponent<Canvas>();

            sites = VisitorSites.GetSiteList();
        }

        private void OnEnable() {
            GenerateMenu();
        }

        #endregion

        #region Stop

        protected void OnDisable() {
            Debug.Log("Save Sitelist when disable");
            VisitorSites.SaveSiteList(sites);
        }

        protected virtual void OnDestroy() {
            Debug.Log("Save Sitelist");
            VisitorSites.SaveSiteList(sites);
        }

        #endregion

        #region UI

        protected void ClearMenu() {
            if (canvas == null)
                return;

            Button[] buttons = canvas.GetComponentsInChildren<Button>();
            foreach (Button button in buttons)
                Destroy(button.gameObject);
            InputField[] inputs = canvas.GetComponentsInChildren<InputField>();
            foreach (InputField input in inputs)
                Destroy(input.gameObject);
        }

        protected void GenerateMenu() {
            if (canvas == null)
                return;

            int lineNr = 0;
            ShowAddSite(lineNr++);
            ShowSetHome(lineNr++);
#if pUNITYXR
            Humanoid.HumanoidControl humanoid = GetComponentInParent<Humanoid.HumanoidControl>();
            if (humanoid != null && humanoid.unityXR.enabled == false)
#endif
                ShowEnterURL(lineNr++);            
            ShowSites(lineNr);
        }

        protected void RefreshMenu() {
            ClearMenu();
            GenerateMenu();
        }

        protected void ShowAddSite(int position) {
            if (SiteNavigator.currentSite == null)
                return;

            GameObject menuButtonPrefab = (GameObject)Resources.Load("MenuButton");
            if (menuButtonPrefab == null)
                Debug.LogError("MenuButton prefab is not found in a Resources folder");

            GameObject menuButton = Instantiate(menuButtonPrefab);
            menuButton.transform.SetParent(canvas.transform);
            menuButton.transform.localPosition = new Vector3(0, 125 - position * 75, 0);
            menuButton.transform.localRotation = Quaternion.identity;
            menuButton.transform.localScale = Vector3.one;
            menuButton.name = "Add Site";

            Text text = menuButton.GetComponentInChildren<Text>();
            text.text = "Add this Site";

            string siteName = SiteNavigator.currentSite.siteName;
            string siteLocation = SiteNavigator.currentSite.siteLocation;

            Button button = menuButton.GetComponent<Button>();
            button.onClick.AddListener(() => AddSite(siteName, siteLocation));
        }

        protected void ShowSetHome(int position) {
            if (SiteNavigator.currentSite == null)
                return;

            GameObject menuButtonPrefab = (GameObject)Resources.Load("MenuButton");
            if (menuButtonPrefab == null)
                Debug.LogError("MenuButton prefab is not found in a Resources folder");

            GameObject menuButton = Instantiate(menuButtonPrefab);
            menuButton.transform.SetParent(canvas.transform);
            menuButton.transform.localPosition = new Vector3(0, 125 - position * 75, 0);
            menuButton.transform.localRotation = Quaternion.identity;
            menuButton.transform.localScale = Vector3.one;
            menuButton.name = "Set Home";

            Text text = menuButton.GetComponentInChildren<Text>();
            text.text = "Set Home";

            string siteName = SiteNavigator.currentSite.siteName;
            string siteLocation = SiteNavigator.currentSite.siteLocation;

            Button button = menuButton.GetComponent<Button>();
            button.onClick.AddListener(() => SetHome(siteName, siteLocation));
        }

        protected void ShowEnterURL(int position) {
            GameObject inputFieldPrefab = (GameObject)Resources.Load("InputField");
            if (inputFieldPrefab == null)
                Debug.LogError("InputField prefab is not found in a Resources folder");

            GameObject inputField = Instantiate(inputFieldPrefab);
            inputField.transform.SetParent(canvas.transform);
            inputField.transform.localPosition = new Vector3(0, 125 - position * 75, 0);
            inputField.transform.localRotation = Quaternion.identity;
            inputField.transform.localScale = Vector3.one;

            InputField field = inputField.GetComponent<InputField>();
            field.onEndEdit.AddListener(value => {
                string siteLocation = value;
                string siteName = siteLocation.Substring(siteLocation.LastIndexOf('/') + 1);
                AddSite(siteName, siteLocation);
            }
            );
        }

        protected void ShowSites(int position) {
            foreach (VisitorSites.Site site in sites)
                ShowSite(site, position++);
        }

        protected void ShowSite(VisitorSites.Site site, int position) {
            SiteNavigator siteNavigator = FindObjectOfType<SiteNavigator>();
            if (siteNavigator == null)
                Debug.LogError("Could not find a site navigator");

            GameObject menuButtonPrefab = (GameObject)Resources.Load("MenuButton");
            if (menuButtonPrefab == null)
                Debug.LogError("MenuButton prefab is not found in a Resources folder");

            int yPosition = 125 - position * 75;

            GameObject menuButton = Instantiate(menuButtonPrefab);
            menuButton.transform.SetParent(canvas.transform);
            menuButton.transform.localPosition = new Vector3(0, yPosition, 0);
            menuButton.transform.localRotation = Quaternion.identity;
            menuButton.transform.localScale = Vector3.one;
            menuButton.name = site.name;

            Text text = menuButton.GetComponentInChildren<Text>();
            text.text = site.name;

            Button button = menuButton.GetComponent<Button>();
            button.onClick.AddListener(() => GoToSite(site));

            GameObject deleteButton = Instantiate(menuButtonPrefab);
            deleteButton.transform.SetParent(canvas.transform);
            RectTransform rectTransform = deleteButton.GetComponent<RectTransform>();
            deleteButton.transform.localPosition = new Vector3(-210, yPosition, 20);
            deleteButton.transform.localRotation = Quaternion.identity;
            deleteButton.transform.localScale = Vector3.one;
            rectTransform.sizeDelta = new Vector2(60, 60);
            deleteButton.name = "Delete_" + site.name;

            text = deleteButton.GetComponentInChildren<Text>();
            text.text = "X";

            button = deleteButton.GetComponent<Button>();
            button.onClick.AddListener(() => DeleteSite(site));

            /*
            GameObject shareButton = Instantiate(menuButtonPrefab);
            shareButton.transform.SetParent(canvas.transform);
            shareButton.transform.localPosition = new Vector3(250, yPosition, 20);
            shareButton.transform.localRotation = Quaternion.identity;
            shareButton.transform.localScale = Vector3.one;
            RectTransform shareRect = shareButton.GetComponent<RectTransform>();
            shareRect.sizeDelta = new Vector2(140, 60);
            shareButton.name = "Delete_" + site.name;

            text = shareButton.GetComponentInChildren<Text>();
            text.text = "Share";
            */
        }

        private void ShowDetails(VisitorSites.Site site) {
            canvas.gameObject.SetActive(false);
            siteDetailsUI.SetSite(site);
            siteDetailsUI.gameObject.SetActive(true);
        }


        #endregion

        public void AddSite(string siteName, string siteLocation) {
            //Debug.Log("Add site " + siteName);
            VisitorSites.Site foundSite = sites.Find(site => site.siteLocation == siteLocation);
            if (foundSite != null)
                return;

            VisitorSites.Site newSite = new VisitorSites.Site() {
                name = siteName,
                siteLocation = siteLocation,
            };
            sites.Add(newSite);
            RefreshMenu();
        }

        public void SetHome(string siteName, string siteLocation) {
            Debug.Log("Set home " + siteName);

            VisitorConfiguration.configuration.startSite = siteLocation;
        }

        private void GoToSite(VisitorSites.Site site) {
            SiteNavigator siteNavigator = FindObjectOfType<SiteNavigator>();
            if (siteNavigator == null) {
                Debug.LogError("Could not find a site navigator");
                return;
            }

            this.gameObject.SetActive(false);
            //transform.parent.gameObject.SetActive(false);
            transform.parent.parent.gameObject.SetActive(false); // HumanoidMenu
            if (mainMenuUI != null)
                mainMenuUI.SetActive(true);
            siteNavigator.LoadSiteFromURL(site.siteLocation);
        }


        private void DeleteSite(VisitorSites.Site site) {
            sites.Remove(site);
            RefreshMenu();
        }

    }


}