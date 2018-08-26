using System.Collections;
using OculusGo;
using UnityEngine;

public class TweetPanelProfileImage : Touchable {

    public override void TouchBegin(LaserPointer pointer)
    {
//        Toggle();
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        Toggle();
    }
    

    void Toggle()
    {
        if(!GetComponentInParent<TweetPanel>().hasUserPanelInstance)
        {
            ShowTween();
        } else
        {
            HideTween();
        }
    }

    private void Tween(Hashtable args)
    {
        args.Add("time", GlobalConfig.moveTime);
        args.Add("easetype", "easeInOutQuad");
        iTween.MoveBy(gameObject, args);
    }

    private void ShowTween()
    {
        Hashtable args = new Hashtable();
        args.Add("amount", new Vector3(0, 0, GlobalConfig.userCardTranslateZ * (-1f)));
        args.Add("oncomplete", "GenerateUserPanel");
        Tween(args);
    }

    private void HideTween()
    {
        Hashtable args = new Hashtable();
        args.Add("amount", new Vector3(0, 0, GlobalConfig.userCardTranslateZ));
        args.Add("onstart", "DestroyUserPanel");
        Tween(args);
    }

    void GenerateUserPanel()
    {
        GetComponentInParent<TweetPanel>().GenerateUserPanel();
    }
    void DestroyUserPanel()
    {
        GetComponentInParent<TweetPanel>().DestroyUserPanel();
    }
}
