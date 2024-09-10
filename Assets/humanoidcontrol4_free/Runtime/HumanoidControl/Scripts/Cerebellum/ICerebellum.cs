using UnityEngine;

namespace Passer {
    /// <summary>
    /// The Cerebellum interface defines the interface to control 
    /// </summary>
    public interface ICerebellum {

        /// <summary>
        /// Retrieve the target for the joint
        /// </summary>
        /// <param name="jointId">The identification of the joint</param>
        /// <returns></returns>
        ICerebellumTarget GetTarget(sbyte jointId);

        /// <summary>
        /// Retrieve the joint
        /// </summary>
        /// <param name="jointId">The identification of the joint</param>
        /// <returns></returns>
        ICerebellumJoint GetJoint(sbyte jointId);
    }

    /// <summary>
    /// A joint target which is used to calculate how the joints need to move
    /// </summary>
    public interface ICerebellumTarget {

        /// <summary>
        /// The position of the target in world space
        /// </summary>
        /// The confidence will be set to 1 (maximum)
        Vector3 position { get; set; }
        /// <summary>
        /// The position of the target in the local space of the parent joint
        /// </summary>
        /// The confidence will be set to 1 (maximum)
        Vector3 localPosition { get; set; }

        /// <summary>
        /// The orientation of the target in world space
        /// </summary>
        /// The confidence will be set to 1 (maximum)
        Quaternion orientation { get; set; }
        /// <summary>
        /// The orientation fo the target in the local space of the parent joint
        /// </summary>
        /// The confidence will be set to 1 (maximum)
        Quaternion localOrientation { get; set; }

        /// <summary>
        /// Set the position of the target in world space with a confidence value
        /// </summary>
        /// <param name="position">The position of the target in world space</param>
        /// <param name="confidence">The confidence of the position in the range 0..1</param>
        void SetPosition(Vector3 position, float confidence);
        /// <summary>
        /// Set the orientation of the target in world space with a confidence value
        /// </summary>
        /// <param name="orientation">The orientation of the target in world space</param>
        /// <param name="confidence">The confidence of the orientation in the range 0..1</param>
        void SetOrientation(Quaternion orientation, float confidence);
    }

    /// <summary>
    /// The joint itself
    /// </summary>
    public interface ICerebellumJoint {

        /// <summary>
        /// The position of the joint in world space
        /// </summary>
        Vector3 position { set; }
        /// <summary>
        /// The position of the joint in the local space of the parent joint
        /// </summary>
        Vector3 localPosition { get; }

        /// <summary>
        /// The orientation of the joint in world space
        /// </summary>
        Quaternion orientation { get; set; }

        /// <summary>
        /// The orientation of the joint in the local space of the parent joint
        /// </summary>
        Quaternion localOrientation { get; set; }
    }
}