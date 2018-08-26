using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Twity;
using Twity.DataModels.Core;

public class TweetPanelManager : MonoBehaviour {

    [SerializeField] private GameObject TweetPanelPrefab;

    private int numOfTweetsLoaded = GlobalConfig.twitterNumOfTweets;
    private int numOfTweetsPerLoad = GlobalConfig.twitterNumOfLoadTweets;


    #region Decide Retweet or not
    public bool isRetweeted(Tweet tweet)
    {
        try
        {
            if (tweet.retweeted_status.text == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (System.Exception e)
        {
            return false;
        }
    }

    public TweetObjectWithUser TweetOnPanel(Tweet tweet)
    {
        if (isRetweeted(tweet))
        {
            return tweet.retweeted_status;
        } else
        {
            return tweet;
        }
    }
    #endregion

    #region Decide Current Displayed or not
    private List<long> IdsOfTweetCurrentDisplayed = new List<long>();
    public bool isCurrentDisplayed(Tweet tweet)
    {
        TweetObjectWithUser originTweet = isRetweeted(tweet) ? tweet.retweeted_status : tweet;
        bool result = IdsOfTweetCurrentDisplayed.Contains(originTweet.id) ? true : false;
        return result;
    }

    public void AddToCurrentList(Tweet tweet)
    {
        IdsOfTweetCurrentDisplayed.Add(TweetOnPanel(tweet).id);
    }
    public void ClearCurrentTweets()
    {
        IdsOfTweetCurrentDisplayed = new List<long>();

        foreach(Transform child in transform) {
            StartCoroutine(DestroyTweetPanel(child));
        }
        
    }

    private IEnumerator DestroyTweetPanel(Transform child) {
        yield return new WaitForEndOfFrame();
        Destroy(child.gameObject);
    }

    

    #endregion




    #region Manage TweetResponse
    public IEnumerator LoadTweets(Tweet[] tweets, bool isOnlyWithImage = false)
    {
        numOfTweetsLoaded = 0;


        for (int i = numOfTweetsLoaded; i < numOfTweetsLoaded + numOfTweetsPerLoad; i++)
        {
            if(isOnlyWithImage) {
                if (tweets[i].extended_entities.media == null) continue;
            }
            GenerateTweetCardAtRandomPosition(tweets[i]);
            yield return new WaitForSeconds(0.2f);
        }
        numOfTweetsLoaded += numOfTweetsPerLoad;
    }


    #endregion

    #region Generate TweetPanel
    public void GenerateTweetCardAtRandomPosition(Tweet tweet)
    {
        GameObject tweetPanel = Instantiate(TweetPanelPrefab, new Vector3(0, Random.Range(1.2f, 3f), Random.Range(1.5f, 3.0f)), transform.localRotation);
        // tweetPanel.GetComponent<TweetPanel>().wholeTweet = tweet;
        tweetPanel.GetComponent<TweetPanel>().tweet = TweetOnPanel(tweet);
        tweetPanel.transform.SetParent(transform);
        tweetPanel.transform.RotateAround(transform.position, new Vector3(0, 1.0f, 0), Random.Range(-90.0f, 90.0f));
        tweetPanel.GetComponent<TweetPanel>().Init();
    }

    public void GenerateTweetCardInFront(Tweet tweet)
    {
        GameObject tweetPanel = Instantiate(TweetPanelPrefab, new Vector3(0, 1.8f, 1.0f), transform.localRotation);
        // tweetPanel.GetComponent<TweetPanel>().wholeTweet = tweet;
        tweetPanel.GetComponent<TweetPanel>().tweet = TweetOnPanel(tweet);
        tweetPanel.transform.SetParent(transform);
        tweetPanel.GetComponent<TweetPanel>().Init();
    }
    #endregion

}

