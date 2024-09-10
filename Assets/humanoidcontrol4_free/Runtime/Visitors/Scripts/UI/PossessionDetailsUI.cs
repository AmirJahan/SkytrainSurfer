using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Passer.Humanoid;

namespace Passer {

    public class PossessionDetailsUI : MonoBehaviour {

        public PossessionsUI pawnPossessionsUI;

        public Text nameUI;
        public Text countUI;
        public Socket socket;

        public HumanoidControl humanoid;
        public VisitorPossessions humanoidPossessions;

        protected VisitorPossessions.Possession possession;

        public void SetPossesion(VisitorPossessions.Possession possession) {
            this.possession = possession;
        }

        public void RetrievePossession(VisitorPossessions.Possession possession) {
            this.possession = possession;
            RetrievePossession();
        }

        public void RetrievePossession() {
            if (possession == null)
                return;

            humanoid = GetComponentInParent<HumanoidControl>();

            if (possession.type == Possessable.Type.Avatar) {
                // make the humanoid change to the avatar directly?
                SiteNavigator.instance.StartCoroutine(VisitorPossessions.RetrievePossessionAsync(possession, ChangeAvatar));
            }
            else {

                nameUI.text = possession.name;
                countUI.text = "";

                SiteNavigator.instance.StartCoroutine(VisitorPossessions.RetrievePossessionAsync(possession, AttachToSocket));
            }
        }

        private void OnDestroy() {
            VisitorPossessions.UnloadPossession();
        }

        private void AttachToSocket(GameObject prefab) {
            humanoidPossessions = humanoid.GetComponentInChildren<VisitorPossessions>();

            humanoidPossessions.DeletePossession(possession);
            GameObject possessionObj = Object.Instantiate(prefab);
            possessionObj.name = prefab.name;

            socket.Attach(possessionObj);
        }

        private void ChangeAvatar(GameObject avatarObj) {
            humanoid.ChangeAvatar(avatarObj);
            //this.gameObject.SetActive(false);
            //pawnPossessionsUI.gameObject.SetActive(false);

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }
    }
}