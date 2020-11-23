using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandGrabInteractable : XRGrabInteractable {
    public List<XRSimpleInteractable> secondHandGrabPoints =
        new List<XRSimpleInteractable>();
    public enum RotationType { None, First, Second };
    public RotationType rotationType;
    public bool snapToSecondHand = true;

    private Quaternion initialRotationOffset;
    private Quaternion attachInitialRotation;
    private XRBaseInteractor secondInteractor;

    void Start() {
        foreach(XRSimpleInteractable grabPoint in secondHandGrabPoints){
            grabPoint.onSelectEntered.AddListener(OnSecondHandGrab);
            grabPoint.onSelectExited.AddListener(OnSecondHandRelease);
        }
    }

    void Update() {
        
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase){
        if(secondInteractor && selectingInteractor){
            if(snapToSecondHand){
                selectingInteractor.attachTransform.rotation = 
                    GetRotation();
            } else {
                selectingInteractor.attachTransform.rotation = 
                    GetRotation() * initialRotationOffset;
            }
            
        }
        base.ProcessInteractable(updatePhase);
    }

    /* might be a bug here:
    secondInteractor.attachTransform.up
    ->
    secondInteractor.transform.up
    */
    private Quaternion GetRotation(){
        if(rotationType == RotationType.None){
            return Quaternion.LookRotation(
                secondInteractor.attachTransform.position -
                selectingInteractor.attachTransform.position);
        } else if(rotationType == RotationType.First){
            return Quaternion.LookRotation(
                secondInteractor.attachTransform.position -
                selectingInteractor.attachTransform.position,
                selectingInteractor.attachTransform.up);
        } else {
            return Quaternion.LookRotation(
                secondInteractor.attachTransform.position -
                selectingInteractor.attachTransform.position,
                secondInteractor.attachTransform.up);
        }
    }

    public void OnSecondHandGrab(XRBaseInteractor interactor){
        Debug.Log("second hand grab");
        secondInteractor = interactor;
        initialRotationOffset = Quaternion.Inverse(GetRotation()) * 
            selectingInteractor.attachTransform.rotation;
    }

    public void OnSecondHandRelease(XRBaseInteractor interactor){
        Debug.Log("second hand release");
        secondInteractor = null;
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor){
        Debug.Log("first grab enter");
        base.OnSelectEntered(interactor);
        attachInitialRotation = interactor.attachTransform.localRotation;
    }

    protected override void OnSelectExited(XRBaseInteractor interactor){
        Debug.Log("first grab exit");
        base.OnSelectExited(interactor);
        secondInteractor = null;
        interactor.attachTransform.localRotation = attachInitialRotation;
    }

    public override bool IsSelectableBy(XRBaseInteractor interactor){
        bool isAlreadyGrabbed = selectingInteractor && 
            !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && 
            !isAlreadyGrabbed;
    } 
}
