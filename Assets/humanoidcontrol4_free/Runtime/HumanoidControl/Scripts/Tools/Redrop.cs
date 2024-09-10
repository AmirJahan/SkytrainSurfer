using UnityEngine;

namespace Passer {

    public class Redrop : MonoBehaviour {

        // Script which is to be replaced by CollisionEventHandler

        public Collider groundCollider;

        private Rigidbody thisRigidbody;
        private Vector3 startPosition;
        private Quaternion startRotation;

        void Start() {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        void Update() {
            if (groundCollider == null && transform.position.y < 0) {
                MoveToStart();
            }
        }

        private void OnCollisionEnter(Collision collision) {
            if (collision.collider == groundCollider) {
                MoveToStart();
            }
        }

        private void MoveToStart() {
            thisRigidbody = transform.GetComponent<Rigidbody>();
            if (thisRigidbody != null) {
                thisRigidbody.MovePosition(new Vector3(startPosition.x, startPosition.y + 0.1F, startPosition.z));
                thisRigidbody.MoveRotation(startRotation);
                thisRigidbody.velocity = Vector3.zero;
                thisRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

}