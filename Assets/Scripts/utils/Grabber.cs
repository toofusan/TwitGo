using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour {

    public bool isRightHand;

    bool isGrabbing;
    bool canGrabNow;
    private List<Grabbable> grabCandidate = new List<Grabbable>();
    Grabbable grabbedObject;
    Vector3 distanceFromGrabbedObject;
    Vector3 positionPrevFrame;
    Quaternion rotateionPrevFrame;

    // Use this for initialization
    private void Start () {
        isGrabbing = false;
    }

    // Update is called once per frame
    private void Update () {
        bool canGrabPrevFrame = canGrabNow;
        canGrabNow = isReadyToGrab;
        CheckForGrabOrRelease(canGrabPrevFrame);

        if (grabbedObject != null)
        {
            MoveGrabbedObject();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Grabbable grabbable = other.GetComponent<Grabbable>();

        if (grabbable != null) grabCandidate.Add(grabbable);

    }

    private void OnTriggerExit(Collider other)
    {
        Grabbable grabbable = other.GetComponent<Grabbable>();

        if (grabbable != null) grabCandidate.Remove(grabbable);

    }


    private void CheckForGrabOrRelease(bool canGrabPrevFrame)
    {
        if (canGrabNow && !canGrabPrevFrame)
        {
            GrabBegin();
        }
        else if (!canGrabNow && canGrabPrevFrame)
        {
            GrabEnd();
        }
    }

    private void GrabBegin()
    {
        if (grabCandidate.Count == 0) return;

        grabbedObject = grabCandidate[0];

        positionPrevFrame = transform.position;
        rotateionPrevFrame = transform.rotation;

        distanceFromGrabbedObject = positionPrevFrame - grabbedObject.transform.position;

    }

    private void MoveGrabbedObject()
    {
        grabbedObject.transform.position = transform.position - distanceFromGrabbedObject;
    }


    private void GrabEnd()
    {
        grabbedObject = null;
    }



    private bool isReadyToGrab
    {
        get
        {
            if (isRightHand)
            {
                return
                OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) >= 0.5f &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) >= 0.5f &&
                !isGrabbing;
            } else
            {
                return
                OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) >= 0.5f &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) >= 0.5f &&
                !isGrabbing;
            }
        }
    }
}
