using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer.Humanoid {

	public class HandInterface : MonoBehaviour {
		public bool isLeft;
		protected HandTarget handTarget;

		protected void Start() {
			HumanoidControl[] humanoids = FindObjectsOfType<HumanoidControl>();
			if (humanoids.Length != 1)
				return;

			handTarget = isLeft ? humanoids[0].leftHandTarget : humanoids[0].rightHandTarget;
		}

		protected void Update() {
			if (handTarget == null)
				return;

			this.transform.rotation = handTarget.transform.rotation;
			this.transform.position = handTarget.transform.position;
		}

		public void MoveTo(GameObject obj) {
			if (handTarget == null)
				return;

			handTarget.transform.position = obj.transform.position;
			handTarget.transform.rotation = obj.transform.rotation;
		}

		public void ParentTo(GameObject obj) {
			if (handTarget == null)
				return;

			handTarget.transform.parent = obj.transform;
		}

		public void Grab(GameObject obj) {
			if (handTarget == null)
				return;

			handTarget.Grab(obj, false);
		}

		public void LetGo() {
			if (handTarget == null)
				return;

			handTarget.LetGo();
		}

		public void EnableAnimator(bool enabled) {
			if (handTarget == null)
				return;

			handTarget.animator.enabled = enabled;
        }
	}

}