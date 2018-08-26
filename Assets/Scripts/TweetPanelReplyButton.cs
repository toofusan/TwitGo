using OculusGo;
using UnityEngine;

public class TweetPanelReplyButton : Touchable {

    TweetPanel tweetPanel;
    public GameObject PostTweetPanelPrefab;

    public void Init()
    {
        tweetPanel = GetComponentInParent<TweetPanel>();
    }

    public override void TouchBegin(LaserPointer pointer)
    {
        
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        base.TouchBegin(pointer);
        GameObject PostTweetPanel = Instantiate(PostTweetPanelPrefab, new Vector3(0, 1.5f, 0.5f), new Quaternion(0, 0, 0, 0));
        PostTweetPanel.GetComponent<PostTweetPanel>().replyTweet = tweetPanel.tweet;
        PostTweetPanel.GetComponent<PostTweetPanel>().Init();
    }
}
