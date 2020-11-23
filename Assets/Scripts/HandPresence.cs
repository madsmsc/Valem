using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour{
	public InputDeviceCharacteristics characteristics;
	public GameObject prefabHand;
	public GameObject spawnedHand;
	public Animator animator;

    private InputDevice targetDevice;
    
    void Start() {
		TryInitialize();
    }

    void Update() {
		if(targetDevice == null || !targetDevice.isValid){
			TryInitialize();
		} else {
			UpdateAnimation(CommonUsages.trigger, "Trigger");
			UpdateAnimation(CommonUsages.grip, "Grip");
		}

        if(targetDevice.TryGetFeatureValue(
                CommonUsages.primaryButton, out bool primaryButtonValue) 
                && primaryButtonValue) {
            Debug.Log("Primary button pressed!");
        }
    }

	void TryInitialize(){
		List<InputDevice> devices = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
		foreach(InputDevice device in devices) {
			Debug.Log(device.name + " - " + device.characteristics);
		}
		if(devices.Count > 0){
			targetDevice = devices[0];
			spawnedHand = Instantiate(prefabHand, transform);
			animator = spawnedHand.GetComponent<Animator>();
			Debug.Log("Device " + targetDevice + " selected for characteristics " + characteristics);
		} else {
			//Debug.Log("No devices for characteristics " + characteristics);
		}
	}

	void UpdateAnimation(InputFeatureUsage<float> feature, string name) {
		bool hasFeature = targetDevice.TryGetFeatureValue(
			feature, out float featureValue);
		if(hasFeature && featureValue > 0.001f) {
			Debug.Log(name + " pressed: " + featureValue);
			animator.SetFloat(name, featureValue);
		} else {
			animator.SetFloat(name, 0);
		}
	}
}
