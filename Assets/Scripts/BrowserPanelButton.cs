

using OculusGo;

public class BrowserPanelButton : Touchable {

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
		GetComponentInParent<BrowserPanel>().Move(false);

		GetComponentInParent<TweetPanel>().hasMediaImageInstance = false;
	}
}