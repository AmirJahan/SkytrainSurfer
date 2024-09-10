using UnityEngine;

namespace Passer.Humanoid {

	public class HeadInterface : MonoBehaviour {
		protected HeadTarget headTarget;

		protected void Awake() {
			HumanoidControl.onNewHumanoid += NewHumanoid;
		}

		protected virtual void NewHumanoid(HumanoidControl humanoid) {
			headTarget = humanoid.headTarget;
        }

		protected void Update() {
			if (headTarget == null)
				return;

			this.transform.rotation = headTarget.transform.rotation;
			this.transform.position = headTarget.transform.position;
        }

		public void RotationX(float xAngle) {
			if (headTarget == null)
				return;

			headTarget.RotationX(xAngle);
        }
	}

}