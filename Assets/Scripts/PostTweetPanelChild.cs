

using OculusGo;

public class PostTweetPanelChild : Touchable {

    public override void TouchBegin(LaserPointer pointer)
    {
//        base.TouchBegin(pointer);
//        GetComponentInParent<PostTweetPanel>().TouchBeginChild(gameObject.name);
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        base.TouchBegin(pointer);
        GetComponentInParent<PostTweetPanel>().TouchBeginChild(gameObject.name);
    }
    
}
