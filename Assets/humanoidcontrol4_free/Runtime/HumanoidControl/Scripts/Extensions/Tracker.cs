using System.Collections.Generic;
using UnityEngine;

namespace Passer.Tracking {

    /// <summary>
    /// A tracker
    /// </summary>
    public abstract class Tracker {
        /// <summary>
        /// The tracking status
        /// </summary>
        public enum Status {
            Unavailable, ///< The tracking device is not available
            Present,     ///< The tracking device is available but not tracking
            Tracking     ///< The tracking device is actively tracking
        }

        /// <summary>
        /// The name of this tracker
        /// </summary>
        public virtual string name { get { return ""; } }

        /// <summary>
        /// Is this tracker enabled?
        /// </summary>
        public bool enabled;

        /// <summary>
        /// The tracking Status of the tracker
        /// </summary>
        public Status status;

        /// <summary>
        /// The tracking device
        /// </summary>
        public TrackerComponent trackerComponent;

        ///// <summary>
        ///// Returns trackerComponent and creates a new TrackerComponent if it does not exist
        ///// </summary>
        ///// <returns></returns>
        //public abstract TrackerComponent GetTrackerComponent();

        #region SubTrackers

        // For lighthouses, tracking camera's etc.
        // currently disabled, but may be used later again.
        // Alternative: use them as sensors

        /// <summary>
        /// Optional list of SubTrackers
        /// </summary>
        //public List<SubTracker> subTrackers = new List<SubTracker>();

        //public virtual void UpdateSubTracker(int i) {
        //    if (subTrackers[i] != null)
        //        subTrackers[i].UpdateTracker(humanoid.showRealObjects);
        //}

        //protected virtual Vector3 GetSubTrackerPosition(int i) {
        //    return Vector3.zero;
        //}

        //protected virtual Quaternion GetSubTrackerRotation(int i) {
        //    return Quaternion.identity;
        //}


        #endregion SubTrackers

        #region Init

        /// <summary>
        /// Start the tracker
        /// </summary>
        public virtual void StartTracker() { }

        #endregion Init

        #region Stop

        /// <summary>
        /// Stop the tracker
        /// </summary>
        public virtual void StopTracker() { }

        #endregion Stop

        #region Update

        /// <summary>
        /// Update the tracker state
        /// </summary>
        public virtual void UpdateTracker() { }

        #endregion Update

        /// <summary>
        /// Show or hide the Tracker renderers
        /// </summary>
        /// <param name="shown">Renderers are enabled when shown == true</param>
        public virtual void ShowTracker(bool shown) {
            if (trackerComponent == null)
                return;

            Renderer[] renderers = trackerComponent.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) {
                // LineRenderers are used for skeletons
                if (!(renderer is LineRenderer))
                    renderer.enabled = shown;
            }
        }


        #region Calibration

        /// <summary>
        /// Calibrate the tracker
        /// </summary>
        public virtual void Calibrate() { }

        /// <summary>
        /// Adjust the position of the tracker by the given delat
        /// </summary>
        /// <param name="positionalDelta">The positional delta to apply</param>
        /// <param name="rotationalDelta">The rotational delta to apply</param>
        public virtual void AdjustTracking(Vector3 positionalDelta, Quaternion rotationalDelta) {
            if (trackerComponent != null) {
                trackerComponent.transform.position += positionalDelta;
                trackerComponent.transform.rotation *= rotationalDelta;
            }
        }
        #endregion
    }
}