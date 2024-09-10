using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passer {

    /// <summary>
    /// Implements behaviour using interaction
    /// </summary>
    public class InteractionEventHandler : MonoBehaviour {

        public BoolEventHandlers focusHandlers = new BoolEventHandlers() {
            id = 0,
            label = "Focus Event",
            tooltip =
                "Call functions using the interaction focus\n" +
                "Parameter: the object having focus",
            eventTypeLabels = new string[] {
                "Never",
                "On get focus",
                "On lost focus",
                "While in focus",
                "While not in focus",
                "On focus change",
                "Always"
            },
        };

        public BoolEventHandlers clickHandlers = new BoolEventHandlers() {
            id = 1,
            label = "Click Event",
            tooltip =
                "Call functions using the interaction click\n" +
                "Parameter: the click state",
            eventTypeLabels = new string[] {
                "Never",
                "On click down",
                "On click up",
                "While clicking",
                "While not clicking",
                "On click change",
                "Always"
            },
        };

        #region Update

        protected virtual void Update() {
            focusHandlers.Update();
            clickHandlers.Update();
        }

        #endregion Update
    }
}