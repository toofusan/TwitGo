using System.Collections;
using OculusGo;
using UnityEngine;
using UnityEngine.UI;
using Twity;
using Twity.DataModels.Core;

public class TweetPanelLikeButton : Touchable {

    TweetPanel tweetPanel;
    public TweetObjectWithUser tweet { get; set; }
    public TweetObjectWithUser retweet { get; set; }

    Color DefaultColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
    Color favoritedColor = new Color(1.0f, 0.2f, 0.2f, 0.8f);
    Color TouchedColor = new Color(1.0f, 0.8f, 0.8f, 0.8f);

    public void Init()
    {
        tweetPanel = GetComponentInParent<TweetPanel>();
        tweet = tweetPanel.tweet;
        InitializeColor();
    }

    public override void TouchBegin(LaserPointer pointer)
    {
        GetComponent<Image>().color = TouchedColor;
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        ToggleLike();
    }
    public override void TouchEnd(LaserPointer pointer)
    {
        InitializeColor();
    }

    void ToggleLike()
    {
        if (!tweet.favorited)
        {
            tweetPanel.PostLike();
        }
        else
        {
            tweetPanel.PostUnlike();
        }
    }

    public void InitializeColor()
    {
        if (tweet.favorited)
        {
            GetComponent<Image>().color = favoritedColor;
            // GetComponent<ParticleSystem>().Play();
        }
        else
        {
            GetComponent<Image>().color = DefaultColor;
        }
    }
}
