using UnityEngine;

namespace Passer.Tracking {

    /// <summary>
    /// A sensor component is used to add tracking to a transform
    /// </summary>
    /// Custom sensor implementation can be made by deriving from this class.
    public class SensorComponent : MonoBehaviour {
        /// <summary>
        /// The transform which is used as the root of the tracking space
        /// </summary>
        protected Transform trackerTransform;

        /// <summary>
        /// The tracking status of the sensor
        /// </summary>
        public Tracker.Status status;

        /// <summary>
        /// The confidence (0..1) of the tracked rotation
        /// </summary>
        public float rotationConfidence;
        /// <summary>
        /// The confidence (0..1) of the tracked position
        /// </summary>
        public float positionConfidence;

        /// <summary>
        /// Is used to set whether the sensor updates itself
        /// </summary>
        /// When enabled, the sensor will update itself.
        /// When disabled, StartComponent and UpdateComponent need to be called to update the tracking status.
        public bool autoUpdate = true;

        protected bool _show;
        /// <summary>
        /// The render status of the sensor
        /// </summary>
        /// When enabled, sensors with renderers attached will be rendered.
        /// When disabled, sensors will not be rendered.
        public virtual bool show {
            set {
                if (value == true && !_show) {
                    renderController = true;

                    _show = true;
                }
                else if (value == false && _show) {
                    renderController = false;

                    _show = false;
                }
            }
            get {
                return _show;
            }
        }

        /// <summary>
        /// Enable or disable the renderers for this sensor.
        /// </summary>
        protected bool renderController {
            set {
                Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                    renderer.enabled = value;
            }
        }

        /// <summary>
        /// Initializes the sensor.
        /// </summary>
        /// When trackerTransform is null, it will be set automatically to the parent of this transform.
        virtual protected void Awake() {
            if (trackerTransform == null)
                trackerTransform = transform.parent;
        }

        /// <summary>
        /// Starts the sensor
        /// </summary>
        /// Does nothing at this moment.
        virtual protected void Start() {
        }

        /// <summary>
        /// Start the manual updating of the sensor.
        /// </summary>
        /// <param name="trackerTransform"></param>
        /// When this function has been called, autoUpdate will be disabled and the sensor will no longer update from Unity Updates.
        /// Instead, UpdateComponent needs to be called to update the sensor data
        public virtual void StartComponent(Transform trackerTransform) {
            autoUpdate = false;
            this.trackerTransform = trackerTransform;
        }

        /// <summary>
        /// Updates the sensor
        /// </summary>
        /// When autoUpdate is enabled, this will call UpdateComponent.
        private void Update() {
            if (autoUpdate)
                UpdateComponent();
        }

        /// <summary>
        /// Update the component manually
        /// </summary>
        /// This function is meant to be overridden
        public virtual void UpdateComponent() {
            status = Tracker.Status.Unavailable;
            positionConfidence = 0;
            rotationConfidence = 0;
        }
    }

}
