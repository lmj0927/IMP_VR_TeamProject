using UnityEngine;
using InputDevice = UnityEngine.XR.InputDevice;
using CommonUsages = UnityEngine.XR.CommonUsages;
using UnityEngine.InputSystem;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.XR;

public class InputTest : MonoBehaviour
{
    private InputDevice targetDevice;




   

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            InitializeInput();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame && targetDevice != null && targetDevice.isValid)
        {
            TestInputs();
        }


    }

    private void InitializeInput()
    {
        List<InputDevice> XRDevices = new List<InputDevice>();

        // get only the right controller
        InputDeviceCharacteristics chars = InputDeviceCharacteristics.Right |
                                            InputDeviceCharacteristics.Controller;



        InputDevices.GetDevicesWithCharacteristics(chars, XRDevices); // get the right controller

        if (XRDevices.Count > 0)
        {
            targetDevice = XRDevices[0];
        }


    }

    private void TestInputs()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryBtnValue) 
            && primaryBtnValue)
        {
            Debug.Log("A was pressed");
        }

        // detect how much we press the Trigger button
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)
            && triggerValue > 0.2f)
        {
            Debug.Log("Trigger was pressed > 20%: "+triggerValue);
        }

        // detect joystick movement
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickValue)
           && joystickValue != Vector2.zero)
        {
            Debug.Log("Joystick used: " + joystickValue);
        }
    }
}
