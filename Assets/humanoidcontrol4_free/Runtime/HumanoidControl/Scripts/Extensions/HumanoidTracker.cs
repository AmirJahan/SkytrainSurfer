using System.Collections.Generic;
using UnityEngine;

namespace Passer.Humanoid {
    using Passer.Tracking;
    using Tracking;

    /// <summary>
    /// A %Humanoid tracker
    /// </summary>
    public abstract class HumanoidTracker : Tracker {
        /// <summary>
        /// The humanoid for this tracker
        /// </summary>
        public HumanoidControl humanoid;

        #region Manage

        /// <summary>
        /// Check the status of the tracker
        /// </summary>
        /// <param name="humanoid">The humanoid for which the tracker needs to be checked</param>
        public virtual void CheckTracker(HumanoidControl humanoid) {
            if (this.humanoid == null)
                this.humanoid = humanoid;
        }

        /// <summary>
        /// Function delegate for retrieving the tracker
        /// </summary>
        /// <param name="transform">The root transform to start the searching of the tracker</param>
        /// <param name="localPosition">The default local position of the tracker</param>
        /// <param name="localRotation">The default local rotation of the tracker</param>
        /// <returns>The tracker component found or created</returns>
        /// The default position/rotation is relative to the humanoid's real world.
        public delegate TrackerComponent TrackerGetter(Transform transform, Vector3 localPosition, Quaternion localRotation);
        /// <summary>
        /// Function to check the status of a specific tracker
        /// </summary>
        /// <param name="humanoid">The humanoid for which the tracker needs to be checked</param>
        /// <param name="getTracker">Function delegate to retrieve the tracker</param>
        /// The default position/rotation for the tracker when created will be zero
        public void CheckTracker(HumanoidControl humanoid, TrackerGetter getTracker) {
            CheckTracker(humanoid, getTracker, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Function to check the status of a specific tracker
        /// </summary>
        /// <param name="humanoid">The humanoid for which the tracker needs to be checked</param>
        /// <param name="getTracker">Function delegate to retrieve the tracker</param>
        /// <param name="localPosition">The default local position of the tracker</param>
        /// <param name="localRotation">The default local rotation of the tracker</param>
        public void CheckTracker(HumanoidControl humanoid, TrackerGetter getTracker, Vector3 localPosition, Quaternion localRotation) {
            if (this.humanoid == null)
                this.humanoid = humanoid;
            if (this.humanoid == null)
                return;

            if (enabled) {
                if (trackerComponent == null) {
                    Transform realWorld = this.humanoid.realWorld;

                    //Vector3 position = realWorld.TransformPoint(localPosition);
                    //Quaternion rotation = realWorld.rotation * localRotation;
                    trackerComponent = getTracker(realWorld, localPosition, localRotation);
                }
                if (trackerComponent == null)
                    return;
            }
            else {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    if (trackerComponent != null)
                        UnityEngine.Object.DestroyImmediate(trackerComponent.gameObject, true);
                }
#endif
                trackerComponent = null;
            }
        }

        #endregion Manage

        #region Device

#if hFACE
        public virtual Vector3 GetBonePosition(uint actorId, FacialBone boneId) { return Vector3.zero; }
        public virtual Quaternion GetBoneRotation(uint actorId, FacialBone boneId) { return Quaternion.identity; }
        public virtual float GetBoneConfidence(uint actorId, FacialBone boneId) { return 0; }
#endif
        #endregion

        /// <summary>
        /// Get the sensor for the head
        /// </summary>
        /// Will return null when this sensor is not present
        public virtual HeadSensor headSensor {
            get { return null; }
        }
        /// <summary>
        /// Get the sensor for the left hand
        /// </summary>
        /// Will return null when this sensor is not present
        public virtual ArmSensor leftHandSensor {
            get { return null; }
        }
        /// <summary>
        /// Get the sensor for the right hand
        /// </summary>
        /// Will return null when this sensor is not present
        public virtual ArmSensor rightHandSensor {
            get { return null; }
        }
        /// <summary>
        /// Get the sensor for the hips
        /// </summary>
        /// Will return null when this sensor is not present
        public virtual TorsoSensor hipsSensor {
            get { return null; }
        }
        /// <summary>
        /// Get the sensor for the left foot
        /// </summary>
        /// Will return null when this sensor is not present
        public virtual LegSensor leftFootSensor {
            get { return null; }
        }
        /// <summary>
        /// Get the sensor for the right foot
        /// </summary>
        /// Will return null when this sensor is not present
        public virtual LegSensor rightFootSensor {
            get { return null; }
        }

        private HumanoidSensor[] _sensors = new HumanoidSensor[0];
        /// <summary>
        /// The sensors for this tracker
        /// </summary>
        public virtual HumanoidSensor[] sensors {
            get { return _sensors; }
        }

        #region Init

        /// <summary>Start the tracker</summary>
        public virtual void StartTracker(HumanoidControl _humanoid) {
            base.StartTracker();
            humanoid = _humanoid;
        }

        #endregion Init
    }

    public abstract class SubTracker : MonoBehaviour {
        public HumanoidTracker tracker;
        public int subTrackerId = -1;

        public abstract bool IsPresent();
        public abstract void UpdateTracker(bool showRealObjects);
    }
}
