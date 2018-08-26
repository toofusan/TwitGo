using OculusGo;
using UnityEngine;

public class SystemMessagePanelButton : Touchable {

    public override void TouchBegin(LaserPointer pointer)
    {
//        CloseSystemMessagePanel();
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        CloseSystemMessagePanel();
    }

    void CloseSystemMessagePanel()
    {
        GetComponentInParent<SystemMessagePanel>().DestroyPanel();
    }
}
