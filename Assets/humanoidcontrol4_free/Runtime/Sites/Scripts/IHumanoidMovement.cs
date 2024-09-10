using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer.Humanoid {

    public interface IHumanoidMovement {

        void Jump(float takeoffVelocity);

        void Rotation(float yAngle);
    }
}