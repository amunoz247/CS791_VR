using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
public class MoveScrollWidget : MonoBehaviour
{
    public SteamVR_Action_Boolean action;
    private SteamVR_Behaviour_Pose pose;
    // Start is called before the first frame update
    void Start()
    {
        if (pose == null)
        {
            pose = GetComponent<SteamVR_Behaviour_Pose>();
        }
        if (action == null)
        {
            Debug.LogError("No action assigned to this.");
            return;
        }

        // action.AddOnChangeListener(OnScroll, pose.inputSource);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0.733f * Mathf.Cos(Time.time / 1000), 0);
    }

    private void OnScroll(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue) {

    }
}
