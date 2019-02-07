using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyInputController : MonoBehaviour
{
    public SteamVR_TrackedObject rightHand;
    private SteamVR_Controller.Device device;
    
    void Update()
    {
        ButtonTest();
    }

    private void ButtonTest()
    {
        string msg = null;

        // SteamVR
        device = SteamVR_Controller.Input((int)rightHand.index);
        if (device != null && device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            msg = "Trigger press";
            device.triggerHapticPulse(700);
        }

        if (device != null && device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            msg = "Trigger release";
        }

        if (msg != null)
            Debug.Log("Input: " + msg);
    }

    public bool ButtonDown()
    {
        return Input.GetButtonDown("Fire1");
    }

    public bool ButtonUp()
    {
        return Input.GetButtonUp("Fire1");
    }
}
