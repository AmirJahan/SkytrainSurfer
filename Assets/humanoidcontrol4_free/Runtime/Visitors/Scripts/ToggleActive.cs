using UnityEngine;

namespace Passer {

    public class ToggleActive : MonoBehaviour {
        // Temporary helper script to implement toggling the gameobject active state
        public void Toggle() {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }
    }

}