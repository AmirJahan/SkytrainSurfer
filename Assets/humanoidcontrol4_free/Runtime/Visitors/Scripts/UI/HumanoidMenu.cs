using UnityEngine;

namespace Passer.Humanoid {

    public class HumanoidMenu : MonoBehaviour {

        public Canvas mainMenu;

        private void OnEnable() {
            Canvas[] childCanvases = GetComponentsInChildren<Canvas>();
            foreach (Canvas childCanvas in childCanvases) {
                if (childCanvas == mainMenu)
                    continue;

                childCanvas.gameObject.SetActive(false);
            }

            if (mainMenu != null)
                mainMenu.gameObject.SetActive(true);
        }
    }
}