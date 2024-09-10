using UnityEngine;

namespace Passer {

    public static class UnityAngles {
        // Clamp all vector axis between the given min and max values
        // Angles are normalized
        public static Vector3 Clamp(Vector3 angles, Vector3 min, Vector3 max) {
            float x = Clamp(angles.x, min.x, max.x);
            float y = Clamp(angles.y, min.y, max.y);
            float z = Clamp(angles.z, min.z, max.z);
            return new Vector3(x, y, z);
        }

        // clamp the angle between the given min and max values
        // Angles are normalized
        public static float Clamp(float angle, float min, float max) {
            float normalizedAngle = Normalize(angle);
            return Mathf.Clamp(normalizedAngle, min, max);
        }

        // Determine the angle difference, result is a normalized angle
        public static float Difference(float a, float b) {
            float r = Normalize(b - a);
            return r;
        }

        // Normalize an angle to the range -180 < angle <= 180
        public static float Normalize(float angle) {
            while (angle <= -180) angle += 360;
            while (angle > 180) angle -= 360;
            return angle;
        }

        // Normalize all vector angles to the range -180 < angle < 180
        public static Vector3 Normalize(Vector3 angles) {
            float x = Normalize(angles.x);
            float y = Normalize(angles.y);
            float z = Normalize(angles.z);
            return new Vector3(x, y, z);
        }

        // Returns the signed angle in degrees between from and to.
        public static float SignedAngle(Vector3 from, Vector3 to) {
            float angle = Vector3.Angle(from, to);
            Vector3 cross = Vector3.Cross(from, to);
            if (cross.y < 0) angle = -angle;
            return angle;
        }

        // Returns the signed angle in degrees between from and to.
        public static float SignedAngle(Vector2 from, Vector2 to) {
            float sign = Mathf.Sign(from.y * to.x - from.x * to.y);
            return Vector2.Angle(from, to) * sign;
        }

        //public static Quaternion ToQuaternion(Rotation orientation) {
        //    return new Quaternion(orientation.x, orientation.y, orientation.z, orientation.w);
        //}
    }

    public static class Rotations {
        /// <summary>
        /// Rotate a rotation.
        /// Rotates rotation1 according to rotation2.
        /// This is needed, because rotation1 * rotation2 rotates the orientation.
        /// </summary>
        /// <param name="rotation1">The rotation to rotate</param>
        /// <param name="rotation2">The rotation</param>
        /// <returns></returns>
        public static Quaternion Rotate(Quaternion rotation1, Quaternion rotation2) {
            float angle;
            Vector3 axis;
            rotation1.ToAngleAxis(out angle, out axis);

            Vector3 newAxis = rotation2 * axis;
            Quaternion newRotation1 = Quaternion.AngleAxis(angle, newAxis);

            return newRotation1;
        }

        public static void ToSwingTwist(this Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist) {
            Vector3 r = new Vector3(q.x, q.y, q.z);

            // singularity: rotation by 180 degree
            if (r.sqrMagnitude < Mathf.Epsilon) {
                Vector3 rotatedTwistAxis = q * twistAxis;
                Vector3 swingAxis =
                  Vector3.Cross(twistAxis, rotatedTwistAxis);

                if (swingAxis.sqrMagnitude > Mathf.Epsilon) {
                    float swingAngle =
                      Vector3.Angle(twistAxis, rotatedTwistAxis);
                    swing = Quaternion.AngleAxis(swingAngle, swingAxis);
                }
                else {
                    // more singularity: 
                    // rotation axis parallel to twist axis
                    swing = Quaternion.identity; // no swing
                }

                // always twist 180 degree on singularity
                twist = Quaternion.AngleAxis(180.0f, twistAxis);
                return;
            }

            // meat of swing-twist decomposition
            Vector3 p = Vector3.Project(r, twistAxis);
            twist = new Quaternion(p.x, p.y, p.z, q.w);
            twist = Quaternion.Normalize(twist);
            swing = q * Quaternion.Inverse(twist);
        }
    }

    public static class Vectors {
        public static float DistanceToRay(Ray ray, Vector3 point) {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }
    }
    public static class Transforms {
        // transform local rotation to world rotation
        public static Quaternion TransformRotation(Transform transform, Quaternion localRotation) {
            if (transform.parent == null)
                return localRotation;
            else
                return transform.parent.rotation * localRotation;
        }

        //
        // Summary:
        //     ///
        //     Transforms rotation from local space to world space.
        //     ///
        //
        // Parameters:
        //   transform:
        public static Quaternion InverseTransformRotation(Transform transform, Quaternion rotation) {
            if (transform.parent == null)
                return rotation;
            else
                return Quaternion.Inverse(transform.parent.rotation) * rotation;
        }
    }

}