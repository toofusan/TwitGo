using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Twity;
using Twity.DataModels.Core;
using Twity.DataModels.Responses;
using Twity.DataModels.StreamMessages;

public class TweetEventHandler : MonoBehaviour {

    private TweetPanelManager _TweetPanelManager;
    private SystemMessageHandler _SystemMessageHandler;
    private NotificationHandler _NotificationHandler;
	public GameObject TweetCardPrefab;
    public GameObject BrowserPanelPrefab;
	public float radius = GlobalConfig.twitterPositionRadius;
	public int NumOfTweets = GlobalConfig.twitterNumOfTweets;

    public GameObject PostTweetPanelPrefab;

	Tweet[] TweetsNowLoaded;
    public List<long> IdsOfTweetDisplayed;

	// Use this for initialization
	void Start () {

	    _TweetPanelManager    = GameObject.Find("TweetPanelManager").GetComponent<TweetPanelManager>();
        _SystemMessageHandler = GameObject.Find("SystemMessageHandler").GetComponent<SystemMessageHandler>();
        _NotificationHandler = GameObject.Find("NotificationHandler").GetComponent<NotificationHandler>();
        Invoke("StartMyStream", 1f);
    }


    #region Public Methods
    public void GetHomeTimeLine() {
        _TweetPanelManager.ClearCurrentTweets();
        TwitterAPI.StopStream();
        TwitterAPI.GetStatusesHomeTimeline (this, OnGetStatusesHomeTimeline);
	}


    public void GetStatusesUserTimeline(long user_id)
    {
        _TweetPanelManager.ClearCurrentTweets();
        TwitterAPI.StopStream();

        TwitterAPI.GetStatusesUserTimeline(user_id, this, OnGetStatusesUserTimeline);
    }
    public void GetStatusesUserTimeline(string screen_name) {
        _TweetPanelManager.ClearCurrentTweets();
        TwitterAPI.StopStream();
        TwitterAPI.GetStatusesUserTimeline(screen_name, this, OnGetStatusesUserTimeline);
    }
    public void GetFavoritesList() {
        _TweetPanelManager.ClearCurrentTweets();
        TwitterAPI.StopStream();
        TwitterAPI.GetFavoritesList(this, OnGetFavoriteList);
    }



	public void PostStatusesUpdate(string status) {
        TwitterAPI.PostStatusesUpdate(status, this, OnPostStatusesUpdate);
	}
    public void PostStatusesUpdate(string status, long in_reply_to_status_id)
    {
        TwitterAPI.PostStatusesUpdate(status, in_reply_to_status_id, this, OnPostStatusesUpdate);
    }

    private string statusWithMedia;
    public void PostStatusesUpdate(string status, byte[] media)
    {
        statusWithMedia = status;
        string imgbase64 = System.Convert.ToBase64String(media);
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["media_data"] = imgbase64;

        StartCoroutine(Client.Post("media/upload", parameters, MediaUploadCallback));
    }
    #endregion


    #region Opening Process    
    public void StartUserFriendsStream(long user_id)
    {
        TwitterAPI.GetFriendsids(user_id, this, OnGetFriends);
    }
    void OnGetFriends(bool success, string response)
    {
        if(!success) _SystemMessageHandler.GenerateSystemMessage(response);
        Dictionary<string, string> streamParameters = new Dictionary<string, string>();
        
        IdsOfTweetDisplayed = new List<long>();
        FriendsidsResponse Response = JsonUtility.FromJson<FriendsidsResponse>(response);
        StartFriendsStream(Response);
    }
    void StartFriendsStream(FriendsidsResponse response)
    {
        Dictionary<string, string> streamParameters = new Dictionary<string, string>();
        List<long> ids = new List<long>();
        foreach (long id in response.ids)
        {
            ids.Add(id);
        }
        ids.Add(GlobalConfig.myTwitterInfo.id);
        FilterFollow myFriendsids = new FilterFollow(ids);
        streamParameters.Add(myFriendsids.GetKey(), myFriendsids.GetValue());

        TwitterAPI.SetStream(StreamType.PublicFilter, streamParameters, OnStream);
        TwitterAPI.StartStream(this);
    }
    #endregion

    #region Stream Methods
    public void StartUserStream(long user_id)
    {
        Dictionary<string, string> streamParameters = new Dictionary<string, string>();

        TwitterAPI.SetStream(StreamType.PublicFilter, streamParameters, OnStream);
        TwitterAPI.StartStream(this);
    }

    public void StartMyStream()
    {
        Dictionary<string, string> streamParameters = new Dictionary<string, string>();
        streamParameters["stall_warnings"] = true.ToString();
        TwitterAPI.SetStream(StreamType.User, streamParameters, OnStream);
        TwitterAPI.StartStream(this);
    }

    #endregion


    #region Callback Methods

    int numOfTweetsPerLoad = GlobalConfig.twitterNumOfLoadTweets;
    int numOfTweetsLoaded = 0;
    void OnGetStatusesHomeTimeline(bool success, string response)
    {
        if (success)
        {
            StatusesHomeTimelineResponse Response = JsonUtility.FromJson<StatusesHomeTimelineResponse>(response);
            //			foreach (Tweet tweet in Response.items) { GenerateTweetCard (tweet); }
            TweetsNowLoaded = Response.items;
            StartCoroutine(_TweetPanelManager.LoadTweets(TweetsNowLoaded));
            StartMyStream();
        }
        else
        {
            _SystemMessageHandler.GenerateSystemMessage(response);
        }
    }

    void OnGetStatusesUserTimeline(bool success, string response)
    {
        if (success)
        {
            StatusesUserTimelineResponse Response = JsonUtility.FromJson<StatusesUserTimelineResponse>(response);
            TweetsNowLoaded = Response.items;
            StartCoroutine(_TweetPanelManager.LoadTweets(TweetsNowLoaded));
        }
        else
        {
            _SystemMessageHandler.GenerateSystemMessage(response);
        }
    }

    void OnGetFavoriteList(bool success, string response) {
        if (success)
        {
            StatusesUserTimelineResponse Response = JsonUtility.FromJson<StatusesUserTimelineResponse>(response);
            TweetsNowLoaded = Response.items;
            StartCoroutine(_TweetPanelManager.LoadTweets(TweetsNowLoaded, true));
        }
        else
        {
            _SystemMessageHandler.GenerateSystemMessage(response);
        }
    }
    void OnPostStatusesUpdate(bool success, string response)
    {
        if (success)
        {
            // _TweetPanelManager.GenerateTweetCardInFront(JsonUtility.FromJson<Tweet>(response));
            _NotificationHandler.ShowNotification("ツイートが投稿されたよ！", UnitychanResponseType.Jump);
        }
        else
        {
            _SystemMessageHandler.GenerateSystemMessage(response);
        }
    }
    void OnGetSearchTweets(bool success, string response) {
		if (success) {
			SearchTweetsResponse Response = JsonUtility.FromJson<SearchTweetsResponse> (response);
			foreach (Tweet tweet in Response.statuses) { _TweetPanelManager.GenerateTweetCardAtRandomPosition(tweet); }
		} else {
            _SystemMessageHandler.GenerateSystemMessage(response);
        }
	}

	void OnGetFollowersList(bool success, string response) {
		if (success) {
			FollowersListResponse Response = JsonUtility.FromJson<FollowersListResponse> (response);
			foreach (TweetUser user in Response.users) {
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters ["user_id"] = user.id.ToString ();
				parameters ["count"] = 1.ToString ();
				StartCoroutine (Client.Get ("statuses/user_timeline", parameters, new TwitterCallback (this.OnGetStatusesUserTimeline)));
			}
		} else {
            _SystemMessageHandler.GenerateSystemMessage(response);
		}
	}

	void OnGetFriendsList(bool success, string response) {
		if (success) {
			FriendsListResponse Response = JsonUtility.FromJson<FriendsListResponse> (response);
			foreach (TweetUser user in Response.users) {
				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters ["user_id"] = user.id.ToString ();
				parameters ["count"] = 1.ToString ();
				StartCoroutine (Client.Get ("statuses/user_timeline", parameters, new TwitterCallback (this.OnGetStatusesUserTimeline)));
			}
		} else {

		}
	}

    void OnStream(string response, StreamMessageType messageType)
    {
        try
        {
            if(messageType == StreamMessageType.Tweet)
            {
                Tweet tweet = JsonUtility.FromJson<Tweet>(response);
                

                if (!_TweetPanelManager.isCurrentDisplayed(tweet))
                {
                    _TweetPanelManager.AddToCurrentList(tweet);
                    _TweetPanelManager.GenerateTweetCardAtRandomPosition(tweet);
                }
            } else if (messageType == StreamMessageType.StreamEvent)
            {
                StreamEvent streamEvent = JsonUtility.FromJson<StreamEvent>(response);
                _NotificationHandler.ShowNotification(streamEvent);
            } else if (messageType == StreamMessageType.FriendsList)
            {
                FriendsList friendsList = JsonUtility.FromJson<FriendsList>(response);
            }
            //Tweet tweet = JsonUtility.FromJson<Tweet>(response);

            //if (!TweetPanelManager.isCurrentDisplayed(tweet))
            //{
            //    TweetPanelManager.AddToCurrentList(tweet);
            //    GenerateTweetCardAtRandomPosition(tweet);
            //}
        }
        catch (System.Exception e)
        {
            //Debug.Log(e.ToString());
        }
    }

    #endregion


    #region Media Upload
    List<long> media_ids = new List<long>();

    public void UploadMedia()
    {
        for(int i = 1; i < 5; i++)
        {
            byte[] img = File.ReadAllBytes(Application.dataPath + "/screenshot" + i + ".png");
            string imgbase64 = System.Convert.ToBase64String(img);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["media_data"] = imgbase64;

            StartCoroutine(Client.Post("media/upload", parameters, MediaUploadCallback));
        }
        //byte[] img = File.ReadAllBytes(Application.dataPath + "/screenshot.png");
        //string imgbase64 = System.Convert.ToBase64String(img);

        //Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters["banner"] = imgbase64;

        //TwitterAPI.PostMediaUpload("テストその5", imgbase64, this, OnPostStatusesUpdate);
        //StartCoroutine(Client.Post("account/update_profile_banner", parameters, ProfileCallback));

        //TwitterAPI.PostMediaUpload("テスト", ReadPngFile(Application.dataPath + "/test.png"), this, OnPostStatusesUpdate);
    }

    private string media_id;
    void MediaUploadCallback(bool success, string response)
    {
        UploadMedia media = JsonUtility.FromJson<UploadMedia>(response);
        media_id = media.media_id.ToString();
        //media_ids.Add(media.media_id);
        //if (media_ids.Count == 4)
        //{
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    StringBuilder media_ids_string = new StringBuilder();
        //    foreach(long media_id in media_ids)
        //    {
        //        media_ids_string.Append(media_id.ToString() + ",");
        //    }
        //    media_ids_string.Length -= 1;
        //    parameters["media_ids"] = media_ids_string.ToString();
        //    parameters["status"] = "スクリーンショット投稿テスト";
        //    StartCoroutine(Client.Post("statuses/update", parameters, OnPostStatusesUpdate));
        //}
        Debug.Log("mediaUploadCallback");
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["media_ids"] = media_id;
        parameters["alt_text"] = "{\"text\":\"" + statusWithMedia + "\"}";
        Debug.Log(parameters["alt_text"]);
        StartCoroutine(Client.Post("media/metadata/create", parameters, OnPostMediaMetadata));
    }

    void OnPostMediaMetadata(bool success, string response)
    {
        Debug.Log("OnPostMediaMetadata");
        if (success)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["media_ids"] = media_id;
            parameters["status"] = statusWithMedia;
            StartCoroutine(Client.Post("statuses/update", parameters, OnPostStatusesUpdate));
        }
        else
        {
            _SystemMessageHandler.GenerateSystemMessage(response);
        }

    }


    void ProfileCallback(bool success, string response)
    {
        Debug.Log(response);
    }


    //byte[] ReadPngFile(string path)
    //{
    //    FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
    //    BinaryReader bin = new BinaryReader(fileStream);
    //    byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

    //    bin.Close();

    //    return values;
    //}

    #endregion

    #region Screen Shot

    public void ShareScreenshot()
    {
        StartCoroutine(TakeScreenShot());
    }

    public IEnumerator TakeScreenShot()
    {
        yield return new WaitForSeconds(0.5f);

        string imagePath = "screenshot.png";

        File.Delete(imagePath);

        ScreenCapture.CaptureScreenshot(imagePath);
        


        float latency = 0, latencyLimit = 2;
        while (latency < latencyLimit)
        {
            if(File.Exists(imagePath))
            {
                break;
            }
            latency += Time.deltaTime;
            yield return null;
        }

        if(latency >= latencyLimit)
        {
            Debug.Log("Screenshot Error");
        }

        byte[] img = File.ReadAllBytes("screenshot.png");
        string imgbase64 = System.Convert.ToBase64String(img);

        GameObject PostTweetPanel = Instantiate(PostTweetPanelPrefab, new Vector3(0, 1.5f, 0.5f), new Quaternion(0, 0, 0, 0));
        PostTweetPanel.GetComponent<PostTweetPanel>().SetMediaBinary(img);
        PostTweetPanel.GetComponent<PostTweetPanel>().SetHasMedia(true);
        PostTweetPanel.GetComponent<PostTweetPanel>().Init();

        //Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters["media_data"] = imgbase64;

        //StartCoroutine(Client.Post("media/upload", parameters, MediaUploadCallback));

    }

    // public IEnumerator TakeScreenShotForGo() {
    //     yield return new WaitForSeconds(0.5f);

    //     string path = "";
    //     using (AndroidJavaClass jcEnvironment = new AndroidJavaClass ("android.os.Environment"))
    //     using (AndroidJavaObject joExDir = jcEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory")) {
    //         path = joExDir.Call<string>("toString")+"/jp.co.cname.app/";
    //     }

    //     if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    //     path += System.DateTime.Now.Ticks.ToString()+".png";

    //     ScreenCapture.CaptureScreenshot(path);

    //     var tex = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);
    //     tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //     using (FileStream BinaryFile = new FileStream(path, FileMode.Create, FileAccess.Write)) {
    //         using (BinaryWriter Writer = new BinaryWriter(BinaryFile)) {
    //             Writer.Write (tex.EncodeToPNG());
                
    //         }
    //     }


    //     string imagePath = "screenshot.png";

    //     File.Delete(imagePath);

    //     ScreenCapture.CaptureScreenshot(imagePath);



    //     float latency = 0, latencyLimit = 2;
    //     while (latency < latencyLimit)
    //     {
    //         if (File.Exists(imagePath))
    //         {
    //             break;
    //         }
    //         latency += Time.deltaTime;
    //         yield return null;
    //     }

    //     if (latency >= latencyLimit)
    //     {
    //         Debug.Log("Screenshot Error");
    //     }

    //     byte[] img = File.ReadAllBytes("screenshot.png");
    //     string imgbase64 = System.Convert.ToBase64String(img);

    //     GameObject PostTweetPanel = Instantiate(PostTweetPanelPrefab, new Vector3(0, 1.5f, 0.5f), new Quaternion(0, 0, 0, 0));
    //     PostTweetPanel.GetComponent<PostTweetPanel>().SetMediaBinary(img);
    //     PostTweetPanel.GetComponent<PostTweetPanel>().SetHasMedia(true);
    //     PostTweetPanel.GetComponent<PostTweetPanel>().Init();
    // }

    #endregion
}