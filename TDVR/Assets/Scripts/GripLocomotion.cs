using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GripLocomotion : MonoBehaviour
{
    public SteamVR_Behaviour_Pose leftHand;
    public SteamVR_Behaviour_Pose rightHand;
    public SteamVR_Action_Boolean gripAction;

    private Transform gripping = null;
    private SteamVR_Behaviour_Pose grippingWith = null;
    private Vector3 originalPosition;

    // Start is called before the first frame update
    private void Start() {
        gripAction.AddOnChangeListener(OnGripChange, leftHand.inputSource);
        gripAction.AddOnChangeListener(OnGripChange, rightHand.inputSource);
    }

    private void OnDestroy() {
        gripAction.RemoveOnChangeListener(OnGripChange, leftHand.inputSource);
        gripAction.RemoveOnChangeListener(OnGripChange, rightHand.inputSource);
    }

    private void OnGripChange(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue) {
        if(newValue) {
            bool alreadyGripping = gripping != null;
            grippingWith = (inputSource == leftHand.inputSource ? leftHand : rightHand);
            gripping = grippingWith.gameObject.transform;
            originalPosition = gripping.localPosition;

            if(!alreadyGripping) {
                StartCoroutine(UpdatePosition());
            }
        } else {
            if(inputSource == grippingWith.inputSource)
                gripping = null;
        }
    }

    private IEnumerator UpdatePosition() {
        while(gripping != null) {
            Vector3 modPosition = originalPosition - gripping.localPosition;
            modPosition.y = 0;
            transform.localPosition += modPosition;
            originalPosition = gripping.localPosition;
            yield return null;
        }
    }
}
