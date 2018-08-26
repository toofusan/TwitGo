using System.Collections.Generic;
using UnityEngine;
using Twity;
using Twity.DataModels.Core;

public class TwitterAPI {

    #region REST API

    public static int NumOfTweets = GlobalConfig.twitterNumOfTweets;

	public static void GetStatusesHomeTimeline(MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters ["count"] = NumOfTweets.ToString ();
		parameters ["exclude_replies"] = GlobalConfig.twitterExcludeReplies.ToString ();
		behaviour.StartCoroutine (Client.Get ("statuses/home_timeline", parameters, callback));
	}

	public static void GetStatusesUserTimeline(long user_id, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters ["id"] = user_id.ToString ();
		parameters ["count"] = NumOfTweets.ToString ();
		behaviour.StartCoroutine (Client.Get ("statuses/user_timeline", parameters, callback));
	}
	public static void GetStatusesUserTimeline(string screen_name, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters ["screen_name"] = screen_name;
		parameters ["count"] = NumOfTweets.ToString ();
		behaviour.StartCoroutine (Client.Get ("statuses/user_timeline", parameters, callback));
	}

	public static void GetUsersShow(string screen_name, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters ["screen_name"] = screen_name;
		behaviour.StartCoroutine (Client.Get ("users/show", parameters, callback));
	}
    public static void GetFriendsids(string screen_name, MonoBehaviour behaviour, TwitterCallback callback)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["screen_name"] = screen_name;
        behaviour.StartCoroutine(Client.Get("friends/ids", parameters, callback));
    }
    public static void GetFriendsids(long user_id, MonoBehaviour behaviour, TwitterCallback callback)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["user_id"] = user_id.ToString();
        behaviour.StartCoroutine(Client.Get("friends/ids", parameters, callback));
    }
    public static void GetFavoritesList(MonoBehaviour behaviour, TwitterCallback callback)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["count"] = (NumOfTweets * 2).ToString();
        behaviour.StartCoroutine(Client.Get("favorites/list", parameters, callback));
    }

    public static void PostStatusesUpdate(string status, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>();
		parameters ["status"] = status;
		behaviour.StartCoroutine (Client.Post ("statuses/update", parameters, callback));
	}
    public static void PostStatusesUpdate(string status, long in_reply_to_status_id, MonoBehaviour behaviour, TwitterCallback callback)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["status"] = status;
        parameters["in_reply_to_status_id"] = in_reply_to_status_id.ToString();
        Debug.Log(parameters["in_reply_to_status_id"]);
        behaviour.StartCoroutine(Client.Post("statuses/update", parameters, callback));
    }

	public static void PostStatusesRetweet(string id_str, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string> ();
		parameters ["id"] = id_str;
		behaviour.StartCoroutine (Client.Post ("statuses/retweet/" + id_str, parameters, callback));
	}

	public static void PostStatusesUnretweet(string id_str, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string> ();
		parameters ["id"] = id_str;
		behaviour.StartCoroutine (Client.Post ("statuses/unretweet/" + id_str, parameters, callback));
	}

	public static void PostFavoritesCreate(string id_str, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string> ();
		parameters ["id"] = id_str;
		behaviour.StartCoroutine (Client.Post ("favorites/create", parameters, callback));
	}
	public static void PostFavoritesDestroy(string id_str, MonoBehaviour behaviour, TwitterCallback callback)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string> ();
		parameters ["id"] = id_str;
		behaviour.StartCoroutine (Client.Post ("favorites/destroy", parameters, callback));
	}
    #endregion

    #region MediaUpload
    public static string statusForMediaUpload;
    public static TwitterCallback callbackForMediaUpload;
    public static MonoBehaviour behaviourForMediaUpload;
    public static void PostMediaUpload(string status, string media_data_base64, MonoBehaviour behaviour, TwitterCallback callback)
    {
        statusForMediaUpload = status;
        callbackForMediaUpload = callback;
        behaviourForMediaUpload = behaviour;

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["media_data"] = media_data_base64;
        parameters["additional_owners"] = GlobalConfig.myTwitterInfo.id_str;
        behaviour.StartCoroutine(Client.Post("media/upload", parameters, PostMediaUploadCallback));
    }
    //public static void PostMediaUpload(string status, byte[] media_raw_data, MonoBehaviour behaviour, TwitterCallback callback)
    //{
    //    statusForMediaUpload = status;
    //    callbackForMediaUpload = callback;
    //    behaviourForMediaUpload = behaviour;

    //    //Dictionary<string, string> parameters = new Dictionary<string, string>();
    //    //parameters["media"] = media_raw_data.ToString();
    //    //behaviour.StartCoroutine(Client.MediaUpload(parameters, callback));
    //    behaviour.StartCoroutine(Client.MediaUpload(media_raw_data, PostMediaUploadCallback));
    //}
    static void PostMediaUploadCallback(bool success, string response)
    {
        if (success)
        {
            UploadMedia media = JsonUtility.FromJson<UploadMedia>(response);
            PostStatusesUpdateWithMedia(statusForMediaUpload, media.media_id, behaviourForMediaUpload, callbackForMediaUpload);
        } else
        {
            GameObject.Find("SystemMessageHandler").GetComponent<SystemMessageHandler>().GenerateSystemMessage(response);
        }
        

    }
    static void PostStatusesUpdateWithMedia(string status, long media_id, MonoBehaviour behaviour, TwitterCallback callback)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["media_ids"] = media_id.ToString();
        parameters["status"] = status;
        behaviour.StartCoroutine(Client.Post("statuses/update", parameters, callback));
    }

    #endregion

    #region Streaming API
    public static bool isStreaming = false;
    private static Stream stream;
    private static Dictionary<string, string> streamParameters;

    private static TwitterStreamCallback streamCallback;

    public static void SetStream(StreamType streamType, Dictionary<string, string> parameters, TwitterStreamCallback callback)
    {
        stream = new Stream(streamType);
        streamParameters = new Dictionary<string, string>();
        foreach(KeyValuePair<string, string> parameter in parameters)
        {
            streamParameters.Add(parameter.Key, parameter.Value);
        }
        streamCallback = callback;
    }

    public static void StartStream(MonoBehaviour behaviour)
    {
        behaviour.StartCoroutine(stream.On(streamParameters, streamCallback));
        isStreaming = true;
    }

    public static void StopStream()
    {
        stream.Off();
        isStreaming = false;
    }

    #endregion

}

