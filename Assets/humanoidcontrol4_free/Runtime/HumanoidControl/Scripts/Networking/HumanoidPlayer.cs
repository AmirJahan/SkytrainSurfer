using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer.Humanoid {
#pragma warning disable 0618

#if hNW_UNET || hNW_MIRROR || hNW_PHOTON || hNW_BOLT
    public partial class HumanoidPlayer {

        [SerializeField]
        protected bool _syncFingerSwing = false;
        public bool syncFingerSwing {
            get { return _syncFingerSwing; }
        }

        [SerializeField]
        protected bool _syncFace = false;
        public bool syncFace {
            get { return _syncFace; }
        }

        [SerializeField]
        private bool _syncTracking = false;
        public bool syncTracking {
            get { return _syncTracking; }
            set { _syncTracking = value; }
        }

        public bool fuseTracking { get; set; }
#else

    public class HumanoidPlayer : MonoBehaviour, IHumanoidNetworking {

        #region Dummy interface

        public void Send(bool b) { }
        public void Send(byte b) { }
        public void Send(int x) { }
        public void Send(float f) { }
        public void Send(Vector3 v) { }
        public void Send(Quaternion q) { }

        public bool ReceiveBool() {
            return false;
        }
        public byte ReceiveByte() {
            return 0;
        }
        public int ReceiveInt() {
            return 0;
        }
        public float ReceiveFloat() {
            return 0;
        }
        public Vector3 ReceiveVector3() {
            return Vector3.zero;
        }
        public Quaternion ReceiveQuaternion() {
            return Quaternion.identity;
        }

        public ulong GetObjectIdentity(GameObject obj) {
            return 0;
        }
        public GameObject GetGameObject(ulong objIdentity) {
            return this.gameObject;
        }

        public void InstantiateHumanoid(HumanoidControl humanoid) {
            if (debug <= HumanoidNetworking.DebugLevel.Info)
                DebugLog("Send Instantiate Humanoid " + humanoid.humanoidId);

            HumanoidNetworking.InstantiateHumanoid instantiateHumanoid = new HumanoidNetworking.InstantiateHumanoid(humanoid);
            if (createLocalRemotes)
                this.Receive(instantiateHumanoid);
        }
        public void DestroyHumanoid(HumanoidControl humanoid) { }
        public void UpdateHumanoidPose(HumanoidControl humanoid) { }
        public void Grab(HandTarget handTarget, GameObject obj, bool rangeCheck, HandTarget.GrabType grabType = HandTarget.GrabType.HandGrab) { }
        public void LetGo(HandTarget handTarget) { }
        public void ChangeAvatar(HumanoidControl humanoid, string avatarPrefabName, string possessionLocation = null) {
            if (debug <= HumanoidNetworking.DebugLevel.Info)
                Debug.Log(humanoid.nwId + ": Change Avatar: " + avatarPrefabName);

            HumanoidNetworking.ChangeAvatar changeAvatar = new HumanoidNetworking.ChangeAvatar(humanoid, avatarPrefabName, possessionLocation);
            if (createLocalRemotes)
                this.Receive(changeAvatar);
        }

        public void SyncTrackingSpace(HumanoidControl humanoid) { }
        public void ReenableNetworkSync(GameObject obj) { }
        public void DisableNetworkSync(GameObject obj) { }

        public void DebugLog(string s) {
            Debug.Log(s);
        }
        public void DebugWarning(string s) {
            Debug.LogWarning(s);
        }
        public void DebugError(string s) {
            Debug.LogError(s);
        }

        public float sendRate => 0;

        public HumanoidNetworking.Smoothing smoothing => HumanoidNetworking.Smoothing.None;

        public bool createLocalRemotes { get => true; set { return; } }

        private bool _isLocal;
        public bool isLocal => _isLocal; // may need to implement this

        public ulong nwId => 0;

        public bool syncFingerSwing => false;

        public bool syncTracking { get => false; set { return; } }

        public bool fuseTracking => false;

        public HumanoidNetworking.HumanoidPose lastHumanoidPose { get => null; set { return; } }
        #endregion Dummy Interface

        public List<HumanoidControl> humanoids { get; set; }

        protected virtual void Awake() {
            mInstance = this;

            GameObject.DontDestroyOnLoad(this.gameObject);
            humanoids = HumanoidNetworking.FindLocalHumanoids();

            for (int i = 0; i < humanoids.Count; i++) {
                HumanoidControl humanoid = humanoids[i];
                if (humanoid.isRemote)
                    continue;

                humanoid.humanoidNetworking = this;

                //((IHumanoidNetworking)this).InstantiateHumanoid(humanoid);
            }
        }

#endif
        [SerializeField]
        protected HumanoidNetworking.DebugLevel _debug = HumanoidNetworking.DebugLevel.Error;
        public HumanoidNetworking.DebugLevel debug {
            get { return _debug; }
        }

        static HumanoidPlayer mInstance;
        public static HumanoidPlayer instance {
            get {
                if (mInstance == null) {
                    Debug.LogWarning("No HumanoidPlayer instance found, instantiating a new instance...");
                    GameObject go = new GameObject();
                    mInstance = go.AddComponent<HumanoidPlayer>();
                }
                return mInstance;
            }
        }

    }

#pragma warning restore 0618
}
