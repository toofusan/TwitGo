using System.Collections;
using System.Collections.Generic;
using OculusGo;
using UnityEngine;

public class TweetPanelWebLink : Touchable
{

	public string Url { get; set; }

	TweetPanel tweetPanel;
	
	[SerializeField] private GameObject _browserPanelPrefab;
	
	public void Init()
	{
		tweetPanel = GetComponentInParent<TweetPanel>();
	}

	public override void TouchBegin(LaserPointer pointer)
	{
//		GetComponent<Image>().color = TouchedColor;
	}

	public override void TouchTrigger(LaserPointer pointer)
	{
		GenerateBrowserPanel();
	}
	public override void TouchEnd(LaserPointer pointer)
	{
//		InitializeColor();
	}

	private void GenerateBrowserPanel()
	{
		if (GetComponentInParent<TweetPanel>().HasBrowserPanelInstance) return;

		GameObject browserPanel = Instantiate(
			_browserPanelPrefab,
			transform.position,
			transform.rotation
		);
		
		browserPanel.transform.SetParent(transform.GetComponentInParent<TweetPanel>().gameObject.transform);
		browserPanel.GetComponent<BrowserPanel>().Url = Url;
		Debug.Log("selfLog: BrowserPanel.Url " + Url);
		browserPanel.GetComponent<BrowserPanel>().Init();

		GetComponentInParent<TweetPanel>().HasBrowserPanelInstance = true;

	}
	
}
