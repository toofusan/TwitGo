using System.Collections;
using UnityEngine;



public class BrowserPanel : Grabbable
{

	private BrowserPanelImage _webViewImage;
	public string Url { get; set; }
	
	private readonly float _positionY = 1.5f;
	private float offsetY;
	private readonly float _offsetZ = -0.1f;

	public void Init()
	{
//		Transform browserGUI = transform.Find("Panel/Browser/Display");
//		browserGUI.GetComponent<Browser>().Url = Url;

		_webViewImage = transform.Find("Panel/Image").GetComponent<BrowserPanelImage>();
		_webViewImage.url = Url;
		_webViewImage.Init();
		
		SetPosition(_positionY);
		Move(this);
	}
	
	
	
	
	void SetPosition(float position)
	{
		transform.Translate(new Vector3(0, 0, _offsetZ));
	}

	public void Move(bool isAppearance)
	{
		Hashtable moveArgs = this.moveArgs(isAppearance);
		iTween.MoveBy(gameObject, moveArgs);

		Hashtable scaleArgs = this.scaleArgs(isAppearance);
		iTween.ValueTo(gameObject, scaleArgs);
	}

	private Hashtable moveArgs(bool isAppearance)
	{
		float amountZ = isAppearance ? _offsetZ : -_offsetZ;
		float amountY = isAppearance ? offsetY : -offsetY;

		Hashtable args = new Hashtable();
		args.Add("easetype", "easeInOutQuad");
		args.Add("time", 0.2f);
		args.Add("oncomplete", "SetBoxCollider");
		args.Add("islocal", true);
		args.Add("amount", new Vector3(0, amountY, amountZ));

		return args;
	}
	private Hashtable scaleArgs(bool isAppearance)
	{
		float from = isAppearance ? 0.0f : 1.0f;
		float to   = isAppearance ? 1.0f : 0.0f;

		Hashtable args = new Hashtable();
		args.Add("time", 0.2f);
        
		args.Add("easetype", "easeInOutQuad");
		args.Add("onupdate", "FadeUpdate");
		args.Add("from", from);
		args.Add("to", to);
		return args;
	}
	void FadeUpdate(float value)
	{
		GetComponent<CanvasGroup>().alpha = value;
		GetComponent<RectTransform>().localScale = new Vector3(value, value, value);
	}



	public void SetBoxCollider()
	{
		RectTransform panel = transform.Find("Panel").GetComponent<RectTransform>();
		Vector2 panelSize = panel.sizeDelta;

		BoxCollider boxCollider = GetComponent<BoxCollider>();
		boxCollider.size = new Vector3(panelSize.x, panelSize.y, 1f);
	}
}
