using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using UnityEngine.UI;

[RequireComponent( typeof( LineRenderer ) )]
public class RaycastFromHand : MonoBehaviour
{
    private GameObject hitObject;
    private RaycastHit raycastHit;
    private LineRenderer lineRenderer;
    private Button hovered;

    public SteamVR_Action_Boolean action;
    private SteamVR_Behaviour_Pose pose;

    void Awake() {
        if (pose == null) {
            pose = GetComponent<SteamVR_Behaviour_Pose>();
        }
        if (action == null) {
            Debug.LogError("No action assigned to this.");
            return;
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void OnEnable() {
        action.AddOnChangeListener(OnClickAction, pose.inputSource);
    }

    void OnDisable() {
        action.RemoveOnChangeListener(OnClickAction, pose.inputSource);
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + transform.TransformDirection(Vector3.forward) * 10);
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out raycastHit)) {
            if (hitObject != null && hitObject == raycastHit.transform.gameObject) {
                RayStay();
            } else {
                if (hitObject != null) {
                    RayExit();
                }

                hitObject = raycastHit.transform.gameObject;
                RayEnter(raycastHit);
            }
        } else {
            if (hitObject != null) {
                RayExit();
                hitObject = null;
            }
        }
    }

    private void OnClickAction(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue) {
        if (hovered == null) {
            Debug.Log("Nothing is being hovered.");
            return;
        }

        hovered.onClick.Invoke();
    }

    void RayEnter(RaycastHit ray) {
        if (ray.transform.tag != "UIElement") {
            return;
        }

        hovered = ray.transform.gameObject.GetComponent<Button>();
        hovered.Select();
        hovered.OnSelect(null);
    }

    void RayStay() {
    }

    void RayExit() {
        if (hovered != null) {
            hovered.OnDeselect(null);
            hovered = null;
        }
    }
}
