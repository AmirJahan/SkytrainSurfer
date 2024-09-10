using UnityEngine;

namespace Passer {

    /// <summary>
    /// Destroyer of things
    /// </summary>
    /// 
    public class Destroyer : MonoBehaviour {

        /// <summary>
        /// Destroy this GameObject
        /// </summary>
        public void SelfDestroy() {
            Object.Destroy(this.gameObject);
        }

        /// <summary>
        /// Destroy the given GameObject
        /// </summary>
        /// <param name="gameObject">The GameObject to Destroy</param>
        public void Destroy(GameObject gameObject) {
            Object.Destroy(gameObject);
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        public void ApplicationQuit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
