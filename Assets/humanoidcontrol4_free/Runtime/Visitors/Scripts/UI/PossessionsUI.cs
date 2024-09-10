using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Passer.Humanoid {

    public class PossessionsUI : MonoBehaviour {

        private Canvas _canvas;
        public Canvas canvas {
            get {
                if (_canvas == null)
                    _canvas = GetComponent<Canvas>();
                return _canvas;
            }
        }

        protected VisitorPossessions humanoidPossessions;

        public PossessionDetailsUI posessionDetailsUI;

        private void OnEnable() {
            GenerateMenu();
        }

        private void OnDisable() {
            ClearMenu();
        }

        private void GenerateMenu() {
            if (canvas == null)
                return;

            int position = 0;

            HumanoidControl humanoid = this.GetComponentInParent<HumanoidControl>();
            humanoidPossessions = humanoid.GetComponentInChildren<VisitorPossessions>();

            ShowPossessions(humanoidPossessions.myPossessions, position);
        }

        private void ShowPossessions(List<VisitorPossessions.Possession> possessions, int position) {
            foreach (VisitorPossessions.Possession possession in possessions)
                ShowPossession(possession, position++);
        }

        private void ShowPossession(VisitorPossessions.Possession possession, int position) {
            GameObject menuButtonPrefab = (GameObject)Resources.Load("MenuButton");
            if (menuButtonPrefab == null)
                Debug.LogError("MenuButton prefab is not found in a Resources folder");

            int yPosition = 125 - position * 75;

            GameObject possessionButton = Instantiate(menuButtonPrefab);
            possessionButton.transform.SetParent(canvas.transform);
            possessionButton.transform.localPosition = new Vector3(30, yPosition, 0);
            possessionButton.transform.localRotation = Quaternion.identity;
            possessionButton.transform.localScale = Vector3.one;
            possessionButton.name = possession.name;

            Text text = possessionButton.GetComponentInChildren<Text>();
            text.text = possession.name;

            Button button = possessionButton.GetComponent<Button>();
            button.onClick.AddListener(() => ShowDetails(possession));

            if (possession.removable) {
                GameObject deleteButton = Instantiate(menuButtonPrefab);
                deleteButton.transform.SetParent(canvas.transform);
                RectTransform rectTransform = deleteButton.GetComponent<RectTransform>();
                deleteButton.transform.localPosition = new Vector3(-180, yPosition, 10);
                deleteButton.transform.localRotation = Quaternion.identity;
                deleteButton.transform.localScale = Vector3.one;
                rectTransform.sizeDelta = new Vector2(60, 75);
                deleteButton.name = "Delete_" + possession.name;

                text = deleteButton.GetComponentInChildren<Text>();
                text.text = "X";

                button = deleteButton.GetComponent<Button>();
                button.onClick.AddListener(() => DeletePossession(possession));
            }
        }

        private void ShowDetails(VisitorPossessions.Possession possession) {
            canvas.gameObject.SetActive(false);
            posessionDetailsUI.gameObject.SetActive(true);
            posessionDetailsUI.RetrievePossession(possession);
        }

        private void DeletePossession(VisitorPossessions.Possession possession) {
            humanoidPossessions.DeletePossession(possession);
            RefreshMenu();
        }

        private void ClearMenu() {
            if (canvas == null)
                return;

            Button[] buttons = canvas.GetComponentsInChildren<Button>();
            foreach (Button button in buttons) {
                Destroy(button.gameObject);
            }
        }

        private void RefreshMenu() {
            ClearMenu();
            GenerateMenu();
        }
    }

}