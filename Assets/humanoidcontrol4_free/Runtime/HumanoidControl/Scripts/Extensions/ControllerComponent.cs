using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer.Tracking {

	public class ControllerComponent : SensorComponent {
        public bool isLeft;

        public Vector3 primaryAxis;
        public Vector3 secondaryAxis;
        public float trigger1;
        public float trigger2;
        public float button1;
        public float button2;
        public float button3;
        public float button4;
        public float option;

        public float battery;


        #region Start

        public virtual void StartComponent(Transform trackerTransform, bool isLeft) {
            StartComponent(trackerTransform);
            this.isLeft = isLeft;
        }

        #endregion

        #region Update

        #endregion
    }
}