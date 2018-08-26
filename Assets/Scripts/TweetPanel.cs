using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Twity;
using Twity.DataModels.Core;
using Twity.DataModels.Entities;


public class TweetPanel : Grabbable
{

    public Tweet wholeTweet { get; set; }
    public TweetObjectWithUser tweet { get; set; }
    public System.DateTime createDateTime;

    public GameObject MediaImagePanelPrefab;
    private GameObject mediaImagePanelInstance;
    public bool hasMediaImageInstance = false;

    public GameObject BrowserPanelPrefab;
    public bool HasBrowserPanelInstance = false;

    public GameObject userPanelPrefab;
    private GameObject userPanelInstance;
    public bool hasUserPanelInstance = false;

    private Transform _mediaImage;
    private Transform _webLink;
    private Transform _replyButton;
    private Transform _retweetButton;
    private Transform _likeButton;

    private readonly float _upTime     = 0.8f;
    private readonly float _upLength   = 0.1f;
    private readonly float _remainTime = 180f;   // ツイート残存時間
    //public int timeCount = 0;
    private int _limitCount = 20000;

    private bool isStable;


    private void Update()
    {
       if (!isStable) return;

       transform.LookAt(new Vector3(0f, transform.position.y, 0f));
       transform.Rotate(new Vector3(0f, 180f, 0f));
    }

    #region Public Methods

    public void Init()
    {
        _mediaImage    = transform.Find("Panel/TweetInfo/MediaImage");
        _webLink       = transform.Find("Panel/TweetInfo/WebLink");
        _replyButton   = transform.Find("Panel/TweetInfo/Buttons/ReplyButton");
        _retweetButton = transform.Find("Panel/TweetInfo/Buttons/RetweetButton");
        _likeButton    = transform.Find("Panel/TweetInfo/Buttons/LikeButton");

        // tweet = TweetPanelManager.TweetOnPanel(wholeTweet);
        if (tweet.entities.urls != null)
        {
            if (tweet.entities.urls.Length == 0)
            {
                Destroy(_webLink.gameObject);
            }
            else
            {
                _webLink.GetComponent<TweetPanelWebLink>().Url = tweet.entities.urls[0].expanded_url;
                _webLink.GetComponent<TweetPanelWebLink>().Init();
            }
        }
        
        _replyButton.GetComponent<TweetPanelReplyButton>().Init();
        _retweetButton.GetComponent<TweetPanelRetweetButton>().Init();
        _likeButton.GetComponent<TweetPanelLikeButton>().Init();

        isStable = true;

        StartCoroutine(Tween(Random.Range(0.0f, 1.0f), true));
        StartCoroutine(timeCount(_remainTime));
    }

    private IEnumerator timeCount(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);

        StartCoroutine(Tween(Random.Range(0.0f, 1.0f), false));
    }
    #endregion

    #region Helper Methods
    private void InitializeParams()
    {
        try
        {
            transform.Find("Panel/UserInfo/NameText").GetComponent<Text>().text = tweet.user.name;
            transform.Find("Panel/UserInfo/ScreenNameText").GetComponent<Text>().text = "@" + tweet.user.screen_name;
            transform.Find("Panel/TweetInfo/TweetText").GetComponent<Text>().text = tweet.text;
            createDateTime = RequestHelper.CreateDateTime(tweet.created_at);
            StartCoroutine(RequestHelper.SetTextureRectSize(transform.Find("ProfileImage"), tweet.user.profile_image_url));
            transform.Find("Panel/TweetInfo/PostTimeText").GetComponent<Text>().text = RequestHelper.CreateAtFormat(createDateTime);
            LoadMedia();

        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            Destroy(gameObject);
        }
    }

    private void LoadMedia()
    {
        if (tweet.extended_entities.media != null)
        {
            _mediaImage.GetComponent<TweetPanelMediaImage>().extendedEntities = tweet.extended_entities;
            _mediaImage.GetComponent<TweetPanelMediaImage>().Init();
            // SetBoxCollider();
        }
    }

    public void SetBoxCollider()
    {
        RectTransform panel = transform.Find("Panel").GetComponent<RectTransform>();
        Vector2 panelSize = panel.sizeDelta;

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(panelSize.x, panelSize.y, 1f);
        boxCollider.center = new Vector3(0f, -panelSize.y / 2, 0f);
    }
    #endregion

    #region Move
    IEnumerator Tween(float delay, bool isShowup)
    {
        yield return new WaitForSeconds(delay);

        Hashtable fadeArgs = new Hashtable();
        fadeArgs.Add("time", _upTime);
        if (isShowup)
        {
            fadeArgs.Add("from", 0.0f);
            fadeArgs.Add("to", 1.0f);
            fadeArgs.Add("oncomplete", "SetBoxCollider");
        }
        else
        {
            fadeArgs.Add("from", 1.0f);
            fadeArgs.Add("to", 0.0f);
            fadeArgs.Add("oncomplete", "DestroySelf");
        }
        fadeArgs.Add("easetype", "easeInOutQuad");
        fadeArgs.Add("onupdate", "FadeUpdate");
        fadeArgs.Add("onupdatetarget", gameObject);
        iTween.ValueTo(gameObject, fadeArgs);

        Hashtable paramsArgs = new Hashtable();
        paramsArgs.Add("time", _upTime);
        paramsArgs.Add("easetype", "easeInOutQuad");
        paramsArgs.Add("amount", new Vector3(0, _upLength, 0));
        if (isShowup)
        {
            paramsArgs.Add("onstart", "InitializeParams");
        }
        iTween.MoveBy(gameObject, paramsArgs);
    }

    private void FadeUpdate(float fade)
    {
        GetComponent<CanvasGroup>().alpha = fade;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    #endregion


    #region Action for MediaImagePanel
    public void GenerateMediaImagePanel(Extended_Entities extendedEntities)
    {
        if (mediaImagePanelInstance != null) return;

        mediaImagePanelInstance = Instantiate(
            MediaImagePanelPrefab,
            transform.position,
            transform.rotation
        );

        mediaImagePanelInstance.transform.SetParent(transform);
        mediaImagePanelInstance.GetComponent<MediaImagePanel>().extendedEntities = extendedEntities;
        mediaImagePanelInstance.GetComponent<MediaImagePanel>().Init();

    }

    public void DestroyMediaImagePanel() {
        mediaImagePanelInstance.GetComponent<MediaImagePanel>().Move(false);
        mediaImagePanelInstance = null;
        transform.Find("Panel/TweetInfo/MediaImage").GetComponent<TweetPanelMediaImage>().PlayVideo();

    }


    #endregion

    

    #region Methods for UserPanel
    public void GenerateUserPanel()
    {
        hasUserPanelInstance = true;
        userPanelInstance = (GameObject)Instantiate(
            userPanelPrefab,
            transform.localPosition,
            transform.localRotation
        );
        userPanelInstance.transform.SetParent(transform);
        userPanelInstance.GetComponent<UserPanel>().user = tweet.user;
        transform.Find("ProfileImage").SetAsLastSibling();
        userPanelInstance.GetComponent<UserPanel>().Init();
        
    }
    public void DestroyUserPanel()
    {
        hasUserPanelInstance = false;
        Destroy(userPanelInstance);
        
    }
    #endregion

    #region Action for Tweet
    public void PostRetweet()
    {
        TwitterAPI.PostStatusesRetweet(tweet.id_str, this, OnPostRetweet);
    }
    private void OnPostRetweet(bool success, string response)
    {
        if (success)
        {
            tweet.retweeted = true;
            _retweetButton.GetComponent<TweetPanelRetweetButton>().InitializeColor();
        }
        else
        {
            Debug.Log(response);
        }
    }
    public void PostUnretweet()
    {
        TwitterAPI.PostStatusesUnretweet(tweet.id_str, this, OnPostUnretweet);
    }
    private void OnPostUnretweet(bool success, string response)
    {
        if (success)
        {
            tweet.retweeted = false;
            _retweetButton.GetComponent<TweetPanelRetweetButton>().InitializeColor();
        }
        else
        {
            Debug.Log(response);
        }
    }

    public void PostLike()
    {
        TwitterAPI.PostFavoritesCreate(tweet.id_str, this, OnPostLike);
    }
    private void OnPostLike(bool success, string response)
    {
        if (success)
        {
            tweet.favorited = true;
            _likeButton.GetComponent<TweetPanelLikeButton>().InitializeColor();
        }
        else
        {
            Debug.Log(response);
        }
    }
    public void PostUnlike()
    {
        TwitterAPI.PostFavoritesDestroy(tweet.id_str, this, OnPostUnlike);
    }
    private void OnPostUnlike(bool success, string response)
    {
        if (success)
        {
            tweet.favorited = false;
            _likeButton.GetComponent<TweetPanelLikeButton>().InitializeColor();
        }
        else
        {
            Debug.Log(response);
        }
    }
    #endregion
}
