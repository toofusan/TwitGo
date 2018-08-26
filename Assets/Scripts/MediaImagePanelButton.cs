

using OculusGo;

public class MediaImagePanelButton : Touchable {

    public override void TouchBegin(LaserPointer pointer)
    {
//        CloseMediaImagePanel();
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        CloseMediaImagePanel();
    }

    void CloseMediaImagePanel()
    {
        GetComponentInParent<TweetPanel>().DestroyMediaImagePanel();

        GetComponentInParent<TweetPanel>().hasMediaImageInstance = false;
    }
}
