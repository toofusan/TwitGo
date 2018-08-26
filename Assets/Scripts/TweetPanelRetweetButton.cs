using OculusGo;
using UnityEngine;
using UnityEngine.UI;
using Twity;
using Twity.DataModels.Core;

public class TweetPanelRetweetButton : Touchable {

    TweetPanel tweetPanel;
    public TweetObjectWithUser tweet { get; set; }
    public TweetObjectWithUser retweet { get; set; }

    private readonly Color _defaultColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    private readonly Color _favoritedColor = new Color(0.2f, 1.0f, 0.2f, 0.8f);
    private readonly Color _touchedColor = new Color(0.8f, 1.0f, 0.8f, 0.8f);


    public void Init()
    {
        tweetPanel = GetComponentInParent<TweetPanel>();
        tweet = tweetPanel.tweet;
        InitializeColor();
    }

    public override void TouchBegin(LaserPointer pointer)
    {
        GetComponent<Image>().color = _touchedColor;
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        ToggleRetweet();
    }
    public override void TouchEnd(LaserPointer pointer)
    {
        InitializeColor();
    }

    void ToggleRetweet()
    {
        if (!tweet.retweeted)
        {
            tweetPanel.PostRetweet();
        }
        else
        {
            tweetPanel.PostUnretweet();
        }
    }

    public void InitializeColor()
    {
        if (tweet.retweeted)
        {
            GetComponent<Image>().color = _favoritedColor;
        }
        else
        {
            GetComponent<Image>().color = _defaultColor;
        }
    }
}
