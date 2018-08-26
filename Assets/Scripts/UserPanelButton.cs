using OculusGo;
using UnityEngine;

public class UserPanelButton : Touchable {

    public override void TouchBegin(LaserPointer pointer)
    {
//        StartUserTimeline();
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        StartUserTimeline();
    }

    void StartUserTimeline()
    {
        foreach (Transform tweetCard in gameObject.transform.root.transform)
        {
            Destroy(tweetCard.gameObject);
        }
        GetComponentInParent<TweetEventHandler>().GetStatusesUserTimeline(
            GetComponentInParent<UserPanel>().user.id
        );
    }
}
