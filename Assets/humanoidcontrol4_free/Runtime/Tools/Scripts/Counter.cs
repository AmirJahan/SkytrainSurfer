using System.Collections;
using UnityEngine;
#if hNW_PHOTON
using Photon.Pun;
#endif

namespace Passer {

    /// <summary>
    /// A Counter can be used to record a integer number.
    /// </summary>
    /// The Counter can be incremented and decremented and functions cal be called based on the value.
    /// 
    /// \image html CounterInspector.png
    /// \image rtf CounterInspector.png
    /// 
    /// * %Value, see Counter::value
    /// * %Minimum, see Counter::min
    /// * %Maximum, see Counter::max
    /// * %Network Sync, see Counter::networking. This property is only available when a supported networking solution is included in the project.
    /// * Events
    ///     - Value Change Event, see Counter::counterEvent
    ///
    /// \version 4.0 and higher
    [HelpURL("https://passervr.com/apis/HumanoidControl/Unity/class_passer_1_1_counter.html")]
    public class Counter : MonoBehaviour {

        #region Properties

        [SerializeField]
        protected int _value;
        /// <summary>
        /// Sets or gets the value of the Counter
        /// </summary>
        /// If networking is enabled, the value is synchoronized across the network.

        public int value {
            get { return _value; }
            set {
                LocalSetValue(value);
                CallRemote(nameof(LocalSetValue), value);
            }
        }
        [RemoteCallable]
        private void LocalSetValue(int value) {
            _value = value;
            counterEvent.value = _value;
        }

        /// <summary>
        /// The minimum value for the Counter
        /// </summary>
        public int min = 0;
        /// <summary>
        /// The maximum value for the Counter
        /// </summary>
        public int max = 10;

        public float timer = 0;
        private bool timerActive = false;

#if hNW_PHOTON        
        /// <summary>
        /// Synchronize the value across the network.
        /// </summary>
        public bool networking = false;

        protected PhotonView photonView;
#endif


        #endregion Properties

        #region Init

        protected virtual void Awake() {
#if hNW_PHOTON
            photonView = GetComponent<PhotonView>();
            if (photonView == null)
                networking = false;
#endif
            counterEvent.value = _value;
        }

        #endregion

        #region Update

        protected virtual void FixedUpdate() {
            CheckBounds();
            CheckTimer();
        }

        protected virtual void CheckBounds() {
            if (min > max)
                min = max;
            if (max < min)
                max = min;
            _value = Mathf.Clamp(_value, min, max);
        }

        private void CheckTimer() {
            if (timerActive) {
                if (value <= min) {
                    timer = 0;
                    timerActive = false;
                }
                return;
            }

            if (timer > 0)
                StartCoroutine(StartTimer());
        }

        IEnumerator StartTimer() {
            timerActive = true;
            while (_value > min && timerActive && timer > 0) {
                yield return new WaitForSeconds(timer);
                Decrement();
            }
            timer = 0;
            timerActive = false;
            yield return null;
        }

        #endregion

        /// <summary>
        /// Decrements the Counter value by 1
        /// </summary>
        /// If the Counter value is equal or lower than the minimum value,
        /// the value is not changed
        public void Decrement() {
            if (_value > min) {
                LocalDecrement();
                CallRemote(nameof(LocalDecrement));
            }
        }

        [RemoteCallable]
        private void LocalDecrement() {
            _value--;   // don't use value as this will set value on network
                        // NetworkedDecrement is network safe
            counterEvent.value = _value;
        }
        /// <summary>
        /// Increments the Counter value by 1
        /// </summary>
        /// If the Counter value is equal or higher than the maximum value,
        /// the value is not changed
        /// If networking is enabled, the value is incremented on all clients
        /// and event handlers are called on all clients
        public void Increment() {
            if (_value < max) {
                LocalIncrement();
                CallRemote(nameof(LocalIncrement));
            }
        }

        [RemoteCallable]
        private void LocalIncrement() {
            _value++;   // don't use value as this will set value on network
            counterEvent.value = _value;
        }

        /// <summary>
        /// Sets the Counter value to the minimum value
        /// </summary>
        public void SetValueToMin() {
            LocalSetToMin();
            CallRemote(nameof(LocalSetToMin));
        }

        [RemoteCallable]
        private void LocalSetToMin() {
            value = min;
        }

        /// <summary>
        /// Sets the Counter value to the maximum value
        /// </summary>
        public void SetValueToMax() {
            LocalSetToMax();
            CallRemote(nameof(LocalSetToMax));
        }

        [RemoteCallable]
        private void LocalSetToMax() {
            value = max;
        }

        public void SetTimer(int seconds) {
            SetTimer(seconds, 1);
        }

        public void SetTimer(int seconds, float speed) {
            _value = seconds;
            timer = speed;
            CheckTimer();
        }


        #region Networking

#if hNW_PHOTON
        class RemoteCallable : PunRPC { }
#else
        class RemoteCallable : System.Attribute { }
#endif
        protected void CallRemote(string functionName, params object[] parameters) {
#if hNW_PHOTON
            if (networking == false || photonView == null || !PhotonNetwork.IsConnected)
                return;

            photonView.RPC(functionName, RpcTarget.Others, parameters);
#endif
        }

        #endregion

        #region Events


        /// <summary>
        /// Can be used to call values based on the Counter value
        /// </summary>
        public IntEventHandlers counterEvent = new IntEventHandlers() {
            label = "Value Change Event",
            tooltip =
                "Call functions using counter values\n" +
                "Parameter: the counter value",
            eventTypeLabels = new string[] {
                "Never",
                "On Min",
                "On Max",
                "While Min",
                "While Max",
                "When Changed",
                "Always"
            }
        };

        #endregion
    }

}