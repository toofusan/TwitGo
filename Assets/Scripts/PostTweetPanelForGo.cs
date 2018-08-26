using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Windows.Speech;
using Twity.DataModels.Core;

public class PostTweetPanelForGo : Grabbable
{
    

    private string statusTextString;
    public string StatusTextString {
        get { return statusTextString; }
    }

    GameObject statusText;
    GameObject statusTextCount;
    GameObject postMediaImage;
    GameObject keyboard;

    private string userScreenName;

    public TweetObjectWithUser quoteTweet;
    private bool isQuote;

    public TweetObjectWithUser replyTweet;
    public bool hasReply;

    private byte[] _mediaBinary;
    public byte[] mediaBinary
    {
        get
        {
            return _mediaBinary;
        }
    }
    public void SetMediaBinary(byte[] _mediaBinary)
    {
        this._mediaBinary = _mediaBinary;
    }

    private bool _hasMedia = false;
    public bool hasMedia
    {
        get
        {
            return _hasMedia;
        }
    }
    public void SetHasMedia(bool _hasMedia)
    {
        this._hasMedia = _hasMedia;
    }
    Vector2 rawImageRegularSize = new Vector2(432.0f, 243.0f);


    string tweetText;

    private void Update()
    {
        string resultString = userScreenName + statusTextString;
        statusText.GetComponent<Text>().text = resultString;
        statusTextCount.GetComponent<Text>().text = resultString.Length.ToString();

    }



    public void Init()
    {
        statusText = transform.Find("Panel/Content/StatusText").gameObject;
        statusTextCount = transform.Find("Panel/Content/StatusTextCount").gameObject;
        postMediaImage = transform.Find("Panel/Content/PostMediaImage").gameObject;
        keyboard = transform.Find("Keyboard").gameObject;
        
        if (quoteTweet != null) isQuote = true;

        if (replyTweet.user.screen_name != "")
        {
            userScreenName = "@" + replyTweet.user.screen_name + " ";
        }

        if (_mediaBinary != null && _mediaBinary.Length != 0)
        {
            Debug.Log(_mediaBinary.Length);
            Debug.Log(System.Convert.ToBase64String(_mediaBinary));
            SetMedia();
        }
        else
        {
            Destroy(postMediaImage);
        }

        keyboard.GetComponent<PostTweetPanelForGoKeyboard>().SetKeyboard();
    }

    public void ActionFromChild(string childName)
    {
        if (childName == "RecordButton")
        {
            //ToggleRecord();
        }
        else if (childName == "PostTweetButton")
        {
            if (statusText.GetComponent<Text>().text != "")
            {
                PostTweet();
            }
        }
        else if (childName == "CancelButton")
        {
            GetComponentInParent<PostTweetPanelHandler>().DestroyPostTweetPanel();
        }
    }



#region About Media

    private void SetMedia()
    {
        RawImage rawImage = transform.Find("Panel/Content/PostMediaImage/RawImage").GetComponent<RawImage>();
        Rect rect = transform.Find("Panel/Content/PostMediaImage/RawImage").GetComponent<RectTransform>().rect;
        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height);
        texture.LoadImage(_mediaBinary);
        rawImage.texture = texture;
        rawImage.SetNativeSize();

        float aspect = rawImage.rectTransform.sizeDelta.y / rawImage.rectTransform.sizeDelta.x;

        rawImage.rectTransform.sizeDelta =
            aspect <= 1f ?
            new Vector2(rawImageRegularSize.x, rawImageRegularSize.x * aspect) :
            new Vector2(rawImageRegularSize.y / aspect, rawImageRegularSize.y);


    }


    #endregion



    #region Set Keyboard

    public void AddCharacter(string s) {
        statusTextString += s;
    }
    public void ChangeCharacter(string s) {
        if (s == "" || s == null) return;
        statusTextString = statusTextString.Substring(0, statusTextString.Length - 1);
        statusTextString += s;
    }


    #endregion

    #region Post a Tweet

    private void PostTweet()
    {
        TweetEventHandler tweetEventHandler = GameObject.Find("TweetEventHandler").GetComponent<TweetEventHandler>();
        if (hasReply)
        {
            tweetEventHandler.PostStatusesUpdate(statusText.GetComponent<Text>().text, replyTweet.id);
        }
        else if (_hasMedia)
        {
            Debug.Log("media");
            tweetEventHandler.PostStatusesUpdate(statusText.GetComponent<Text>().text, _mediaBinary);
        }
        else
        {
            tweetEventHandler.PostStatusesUpdate(statusText.GetComponent<Text>().text);
        }

    }

    #endregion


}
