using OculusGo;
using UnityEngine;

public class Grabbable : MonoBehaviour
{

    private Grabber grabber;

    public bool isGrabbed
    {
        get
        {
            return grabber != null;
        }
    }

    virtual public void GrabBegin(LaserPointer pointer)
    {

    }

    virtual public void GrabEnd(LaserPointer pointer)
    {
    }
}
