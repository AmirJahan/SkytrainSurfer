using UnityEngine;

namespace Passer {

	public static class RigidbodyTools {
		public static void DestroyGameObject(this Rigidbody rigidbody) {
			Object.Destroy(rigidbody.gameObject);
        }
	}

}