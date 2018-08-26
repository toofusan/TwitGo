using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Twity;
using Twity.DataModels.StreamMessages;
using Twity.DataModels.Core;

public class NotificationPanel : MonoBehaviour {

    // public StreamEvent streamEvent;
    // public string noticeText;
    bool isMyAction;

    Tweet TargetObjectTweet;
    List TargetObjectList;

    // public void Init()
    // {
    //     if (streamEvent != null) {
    //         isMyAction = streamEvent.source.id == GlobalConfig.myTwitterInfo.id ? true : false;

    //         if (streamEvent.event_name == null) return;
    //         if (isMyAction) return;

    //         Tween(true);
    //         transform.Find("Panel/Text").GetComponent<Text>().text = notificationText(streamEvent);

    //     } else if (noticeText != null) {
    //         Tween(true);
    //         transform.Find("Panel/Text").GetComponent<Text>().text = noticeText;
    //     }
  
    // }

    public void Init(StreamEvent streamEvent) {
        if (streamEvent == null) return;

        isMyAction = streamEvent.source.id == GlobalConfig.myTwitterInfo.id ? true : false;

        if (streamEvent.event_name == null) return;
        if (isMyAction) return;

        Tween(true);
        transform.Find("Panel/Text").GetComponent<Text>().text = notificationText(streamEvent);
    }

    public void Init(string noticeText) {
        if (noticeText == null || noticeText == "") return;

        Tween(true);
        transform.Find("Panel/Text").GetComponent<Text>().text = noticeText;
    }


    #region Move
    private void Tween(bool isShowup)
    {
        Hashtable scaleArgs = new Hashtable();
        if (isShowup)
        {
            scaleArgs.Add("from", 0.0f);
            scaleArgs.Add("to", 0.002f);
            scaleArgs.Add("oncomplete", "WaitForDestroy");
            
        }
        else
        {
            scaleArgs.Add("from", 0.002f);
            scaleArgs.Add("to", 0.0f);
            scaleArgs.Add("oncomplete", "Destroy");
        }
        scaleArgs.Add("time", 0.1f);
        scaleArgs.Add("onupdate", "ScaleUpdate");
        scaleArgs.Add("onupdatetarget", gameObject);
        scaleArgs.Add("oncompletetarget", gameObject);

        iTween.ValueTo(gameObject, scaleArgs);
    }

    void ScaleUpdate(float scale)
    {
        GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
    }

    void WaitForDestroy()
    {
        StartCoroutine(Delay(3.0f));
    }
    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Tween(false);
    }
    private void Destroy()
    {
        Destroy(gameObject);
    }

    #endregion

    

    private void CheckType(StreamEvent streamEvent)
    {
        List<string> eventNameForTweetObject = new List<string>() { "favorite", "unfavorite", "quoted_tweet" };
        List<string> eventNameForListObject = new List<string>() { "list_created", "list_destroyed", "list_updated", "list_member_added", "list_member_removed", "list_user_subscribed", "list_user_unsubscribed" };
        List<string> eventNameForNull = new List<string>() { "block", "unblock", "follow", "unfollow", "user_update " };

        if (eventNameForTweetObject.IndexOf(streamEvent.event_name) != -1)
        {
            TargetObjectTweet = JsonUtility.FromJson<Tweet>(streamEvent.target_object);
        }
        else if (eventNameForListObject.IndexOf(streamEvent.event_name) != -1)
        {
            TargetObjectList = JsonUtility.FromJson<List>(streamEvent.target_object);
        }
        else if (eventNameForNull.IndexOf(streamEvent.event_name) != -1)
        {

        } else
        {
            Debug.Log(streamEvent.event_name);
        }

    }

    private string notificationText(StreamEvent streamEvent)
    {
        return String.Format(
            eventNameDictionary[streamEvent.event_name],
            streamEvent.source.name,
            streamEvent.source.screen_name
            );
    }

    #region Dictionary
    private Dictionary<string, string> eventNameDictionary = new Dictionary<string, string>()
    {
        {"favorite", "あなたのツイートが{0}(@{1})さんからいいねされたよ！"},
        {"unfavorite", "あなたのツイートがいいねからはずされちゃった…"},
        {"follow", "{0}(@{1})さんにフォローされたよ！"},
        {"unfollow", "フォローがはずされちゃった…"},
        {"list_member_added", "あなたが{0}(@{1})さんのリストに追加されたよ！"},
        {"list_member_removed", "リストからはずされちゃった…"},
        {"list_user_subscribed", "あなたのリストが{0}(@{1})さんから購読されたよ！" },
        {"list_user_unsubscribed", "あなたのリストが購読からはずされちゃった…" },
        {"quoted_tweet", "あなたのツイートが{0}(@{1})さんに引用されたよ！" },
    };
    #endregion

}
