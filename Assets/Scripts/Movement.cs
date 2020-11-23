using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Movement : MonoBehaviour {
    public XRNode inputSource;
    public LayerMask groundLayer;
    public float speed = 1;
    public float additionalHeight = 0.2f;

    private float gravity = -9.81f;
    private float fallingSpeed;
    private XRRig rig;
    private Vector2 inputAxis;
    private CharacterController character;

    void Start() {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
    }

    // I don't like that I'm handling devices in two places
    // (HandPreecense / Movement)
    // consolidate after going through entire tutorial

    void Update() {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate(){
        CapsuleFollowHeadset();
        DoMove();
        DoFall();
    }

    private void CapsuleFollowHeadset(){
        character.height = rig.cameraInRigSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(
            rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, 
            character.height/2 + character.skinWidth, capsuleCenter.z);
    }

    private void DoMove(){
        float yAngle = rig.cameraGameObject.transform.eulerAngles.y;
        Quaternion yaw = Quaternion.Euler(0, yAngle, 0);
        Vector3 direction = yaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * speed);
    }

    private void DoFall(){
        if(IsGrounded()) {
            fallingSpeed = 0;
        } else {
            fallingSpeed += gravity * Time.fixedDeltaTime;
        }
        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    private bool IsGrounded(){
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit =  Physics.SphereCast(rayStart, character.radius, 
                Vector3.down, out RaycastHit hitInfo, 
                rayLength, groundLayer);
        if(hasHit){
            //Debug.Log("hitInfo: " + hitInfo);
        }
        return hasHit;
    }
}
