using System.Collections;
using OculusGo;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanelChild : Touchable {

	public Color DefaultColor = new Color(1.0f, 1.0f, 1.0f, 1f);
	public Color TouchedColor = new Color(0.8f, 1.0f, 0.8f, 1f);

	float TweenTime = 1.2f;
	float TweenLength = 0.1f;

	bool isShowUp = false;

	// Use this for initialization
	void Start () {
		
	}

	public void ShowSlide() {
		Hashtable paramsArgs = new Hashtable ();
		paramsArgs.Add ("easetype", "easeInOutQuad");
		paramsArgs.Add ("amount", new Vector3 (TweenLength, 0, 0));
		paramsArgs.Add ("time", TweenTime);
		iTween.MoveBy (gameObject, paramsArgs);
	}
	public void ShowUp() {
		Hashtable paramsArgs = new Hashtable ();
		paramsArgs.Add ("easetype", "easeInOutQuad");
		paramsArgs.Add ("amount", new Vector3 (0, TweenLength, 0));
		paramsArgs.Add ("time", TweenTime);
		iTween.MoveBy (gameObject, paramsArgs);
	}

	public void FadeIn() {
		Hashtable fadeArgs = new Hashtable ();
		fadeArgs.Add ("from", 0.0f);
		fadeArgs.Add ("to", 0.8f);
		fadeArgs.Add ("time", TweenTime);
		fadeArgs.Add ("islocal", true);
		fadeArgs.Add ("easetype", "easeInOutQuad");
		fadeArgs.Add ("onupdate", "FadeUpdate");
		fadeArgs.Add ("onupdatetarget", gameObject);
		fadeArgs.Add ("oncomplete", "OnFadeComplete");
		iTween.ValueTo (gameObject, fadeArgs);
	}

	void FadeUpdate(float fade) {
		if (GetComponent<CanvasGroup>() == null) return;

		GetComponent<CanvasGroup>().alpha = fade;
	}

	void OnFadeComplete() {
		isShowUp = true;
	}

    public override void TouchBegin(LaserPointer pointer)
    {
        base.TouchBegin(pointer);
		if (isShowUp) GetComponent<CanvasGroup>().alpha = 1.0f;
    }

	public override void TouchTrigger(LaserPointer pointer) {
        base.TouchTrigger(pointer);
		GetComponentInParent<MenuPanel>().OnChildButtonPressed(gameObject.name, pointer);

	}
    public override void TouchEnd(LaserPointer pointer)
    {
        base.TouchEnd(pointer);
		if (isShowUp) GetComponent<CanvasGroup>().alpha = 0.8f;
    }

}
