using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Twity;
using Twity.DataModels.Core;

public class UserPanel : MonoBehaviour {

    public TweetUser user { get; set; }

    public void Init()
    {
        Debug.Log(user.id);
        transform.Translate(0, 0, GlobalConfig.userCardTranslateZ * (-1f));
        //InitializeParams();
        SetParams();
        ShowPanel();
    }

    private void Tween(Hashtable args)
    {
        args.Add("time", 0.2f);
        args.Add("easetype", "easeInOutQuad");
        args.Add("onupdate", "ShowUpdate");
        args.Add("onupdatetarget", gameObject);
        //if (transform.parent.GetComponent<TweetPanel>().hasUserPanelInstance)
        //{
        //    args.Add("from", 0.0f);
        //    args.Add("to", 160.0f);
        //    args.Add("oncomplete", "InitializeParams");
        //}
        //else
        //{
        //    args.Add("from", 160.0f);
        //    args.Add("to", 0.0f);
        //    args.Add("onstart", "InitializeParams");
        //    args.Add("oncomplete", "DestroySelf");
        //}
        iTween.ValueTo(gameObject, args);
    }

    public void ShowPanel()
    {
        Debug.Log("ShowPanel");
        Hashtable args = new Hashtable();
        args.Add("from", 0.0f);
        args.Add("to", 1.0f);
        args.Add("oncomplete", "ShowContent");
        args.Add("oncompletetarget", gameObject);
        Tween(args);
    }

    //public void DestroyPanel()
    //{
    //    Debug.Log("DestroyPanel");
    //    Hashtable args = new Hashtable();
    //    args.Add("from", 480.0f);
    //    args.Add("to", 0.0f);
    //    args.Add("onstart", "MakeParamsBlank");
    //    args.Add("oncomplete", "DestroySelf");
    //    Tween(args);
    //}

    void ShowUpdate(float value)
    {
        transform.Find("Panel").GetComponent<Image>().fillAmount = value;
    }

    void ShowContent()
    {
        transform.Find("Panel/Content").GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    void SetParams()
    {
        transform.Find("Panel").GetComponent<Image>().color = HTMLStringToColor("#" + user.profile_background_color);
        transform.Find("Panel/Content/NameBox/NameText").GetComponent<Text>().text = user.name;
        transform.Find("Panel/Content/NameBox/ScreenNameText").GetComponent<Text>().text = "@" + user.screen_name;
        transform.Find("Panel/Content/DetailBox/DescriptionText").GetComponent<Text>().text = user.description;
        transform.Find("Panel/Content/DetailBox/NumberInfo/NumOfTweets/Number").GetComponent<Text>().text = user.statuses_count.ToString();
        transform.Find("Panel/Content/DetailBox/NumberInfo/NumOfFriends/Number").GetComponent<Text>().text = user.friends_count.ToString();
        transform.Find("Panel/Content/DetailBox/NumberInfo/NumOfFollowers/Number").GetComponent<Text>().text = user.followers_count.ToString();
    }

    void MakeParamsBlank()
    {
        transform.Find("Panel/NameBox/NameText").GetComponent<Text>().text = "";
        transform.Find("Panel/NameBox/ScreenNameText").GetComponent<Text>().text = "";
        transform.Find("Panel/DetailBox/DescriptionText").GetComponent<Text>().text = "";
        transform.Find("Panel/DetailBox/NumberInfo/NumOfTweets/Number").GetComponent<Text>().text = "";
        transform.Find("Panel/DetailBox/NumberInfo/NumOfFriends/Number").GetComponent<Text>().text = "";
        transform.Find("Panel/DetailBox/NumberInfo/NumOfFollowers/Number").GetComponent<Text>().text = "";
    }

    public void OnTouchAnction(Collider collider, string panelName)
    {
        //if (panelName == "DeletePanel")
        //{
        //    Tween();
        //}
        //else if (panelName == "TimelineCard")
        //{
        //    foreach (Transform tweetCard in gameObject.transform.root.transform)
        //    {
        //        GameObject.Destroy(tweetCard.gameObject);
        //    }
        //    gameObject.transform.root.GetComponent<TweetEventHandler>().GetStatusesUserTimeline(user.id);
        //}
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    Color HTMLStringToColor(string htmlString)
    {
        Color color;
        ColorUtility.TryParseHtmlString(htmlString, out color);
        return color;
    }
}
