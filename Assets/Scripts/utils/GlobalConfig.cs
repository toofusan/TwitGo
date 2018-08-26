using UnityEngine;
using Twity;
using Twity.DataModels.Core;

public class GlobalConfig : MonoBehaviour {

	// ==============================
	// Oculus Config
	// ==============================
	public const float OculusTouchThreshold = 0.5f;



	// ==============================
	// Twitter Config
	// ==============================
	public const int twitterNumOfTweets = 30;
	public const float twitterPositionRadius = 2.0f;
	public const bool twitterExcludeReplies = true;

	public const int twitterNumOfLoadTweets = 30;

	public const float UIScale = 0.003f;

	public string myTwitterScreenName = UserConfig.twitterScreenName;
	public static TweetUser myTwitterInfo { get; set;}

	// ==============================
	// Google Speech Config
	// ==============================
	public const int googleSpeechFrequency = 16000;
	public const int googleSpeechLengthSec = 10;

	// ==============================
	// iTween Config
	// ==============================
	public const float moveTime = 0.4f;

	// ==============================
	// Media Config
	// ==============================
	public static Vector2 mediaSmallSize = new Vector2 (160, 120);
	public static Vector2 mediaLargeSize = new Vector2 (480, 360);

	public const float mediaShowPositionY = 1.8f;

	public const float backgroundImagePositionRadius = 6.0f;

	// ==============================
	// UserCard Config
	// ==============================
	public const float userCardTranslateZ = 0.1f;


	// Use this for initialization
	void Start () {

        // Oculus Go Conf
        //OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.Off / LMSLow / LMSMedium / LMSHigh;
        OVRManager.display.displayFrequency = 72f;


        Oauth.consumerKey = UserConfig.twitterConsumerKey;
		Oauth.consumerSecret = UserConfig.twitterConsumerSecret;
		Oauth.accessToken = UserConfig.twitterAccessToken;
		Oauth.accessTokenSecret = UserConfig.twitterAccessTokenSecret;

		//googleAPI.Client.APIKey = googleAPIkey;

		GetMyTwitterInfo ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetMyTwitterInfo() {
		TwitterAPI.GetUsersShow(myTwitterScreenName, this, this.OnGetMyTwitterInfo);
	}
	void OnGetMyTwitterInfo(bool success, string response) {
		myTwitterInfo = JsonUtility.FromJson<TweetUser> (response);

	}
}
