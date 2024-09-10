using UnityEngine;

namespace Passer {
    using Tracking;

    /// <summary>
    /// A sensor used to track an object
    /// </summary>
    [System.Serializable]
    public abstract class Sensor {
        public bool enabled;
        public virtual Tracker.Status status { get; set; }

        public Target target;
        public Tracker tracker;

        public virtual string name { get { return ""; } }

        /// <summary>
        /// The sensor used for tracking
        /// </summary>
        public SensorComponent sensorComponent;

        public virtual void Start(Transform targetTransform) {
            target = targetTransform.GetComponent<Target>();
        }

        public virtual void Update() {
            if (tracker == null || !tracker.enabled || !enabled)
                return;
        }

        public virtual void ShowSensor(bool shown) {
            if (sensorComponent == null)
                return;

            if (!Application.isPlaying)
                sensorComponent.gameObject.SetActive(shown);

            Renderer[] renderers = sensorComponent.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++) {
                if (!(renderers[i] is LineRenderer))
                    renderers[i].enabled = shown;
            }
        }
    }
}