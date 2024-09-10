using UnityEngine;
using UnityEngine.SceneManagement;

namespace Passer {

    public class MouseCursor : MonoBehaviour {

        public bool shown = true;

        protected void Start() {
            //Cursor.lockState = CursorLockMode.None;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoad;
        }

        protected void ShowMousePointer(bool showCursor) {
            shown = showCursor;
        }

        protected void Update() {
            Cursor.visible = shown;
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
                canvas.worldCamera = Camera.main;
        }
    }
}