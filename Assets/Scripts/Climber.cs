using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Climber : MonoBehaviour {
    public static XRController climbingHand;

    private CharacterController character;
    private Movement movement;

    void Start() {
        character = GetComponent<CharacterController>();
        movement = GetComponent<Movement>();
    }

    void FixedUpdate() {
        if(climbingHand){
            movement.enabled = false;
            Climb();
        } else {
            movement.enabled = true;
        }
    }

    private void Climb(){
        InputDevices.GetDeviceAtXRNode(
            climbingHand.controllerNode).
            TryGetFeatureValue(
                CommonUsages.deviceVelocity,
                out Vector3 velocity);
        character.Move(transform.rotation * 
            -velocity * Time.fixedDeltaTime);
    }
}