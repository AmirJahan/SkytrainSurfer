using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer {

    public class VisitorName : MonoBehaviour {

        public UnityEngine.UI.Text nameText;
        public UnityEngine.UI.InputField nameField;

        protected virtual void Awake() {
            Humanoid.HumanoidControl humanoid = GetComponentInParent<Humanoid.HumanoidControl>();
            if (humanoid == null)
                return;

            if (humanoid.isRemote == false) {
                humanoid.name = VisitorConfiguration.configuration.name;
                if (nameField != null) {
                    nameField.text = humanoid.name;
                    nameField.onEndEdit.AddListener(UpdateName);
                }
            }
            if (nameText != null)
                nameText.text = humanoid.name;
        }

        private void UpdateName(string newName) {
            VisitorConfiguration.configuration.name = newName;
        }

        protected void OnDestroy() {
            VisitorConfiguration.SaveConfiguration();
        }
    }
}