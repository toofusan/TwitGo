using System.Collections;
using OculusGo;
using UnityEngine;
using UnityEngine.UI;

public class RefreshPanelHandler : Touchable {
    
    private readonly Color _defaultColor = new Color(0f, 1.0f, 0.4f, 0.4f);
    private readonly Color _touchedColor = new Color(0f, 1.0f, 0.4f, 0.8f);

    private Transform _panel;
    
    public override void TouchBegin(LaserPointer pointer)
    {
        _panel.GetComponent<Image>().color = _touchedColor;
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        RefreshTweetPanels();
    }

    public override void TouchEnd(LaserPointer pointer)
    {
        _panel.GetComponent<Image>().color = _defaultColor;
    }

    private void Start()
    {
        _panel = transform.Find("Panel");
    }

    private void RefreshTweetPanels()
    {
        GameObject tweetEventHandler = GameObject.Find("TweetEventHandler");
        foreach (Transform tweetPanel in tweetEventHandler.transform)
        {
            StartCoroutine(DestroyTweetPanel(Random.Range(0f, 0.2f), tweetPanel));
        }
        tweetEventHandler.GetComponent<TweetEventHandler>().GetHomeTimeLine();
    }

    IEnumerator DestroyTweetPanel(float delay, Transform tweetPanel)
    {
        yield return new WaitForSeconds(delay);
        Destroy(tweetPanel.gameObject);
    }
    
    
}
