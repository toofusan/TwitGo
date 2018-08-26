using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Windows.Speech;
using Twity.DataModels.Core;

public class PostTweetPanel : Grabbable {

    AudioSource audioSource;

    float[] waveData = new float[1024];
    GameObject recordButton;
    Image recordButtonImage;
    Sprite recordOnButtonSprite;
    Sprite recordOffButtonSprite;
    GameObject statusText;
    GameObject statusTextCount;
    GameObject postMediaImage;

    //private DictationRecognizer m_DictationRecognizer;
    private string dictationResult;
    private string dictationHypotheses;
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

	private void Start () {
        audioSource = GetComponent<AudioSource>();
        statusText = transform.Find("Panel/Content/StatusText").gameObject;
        statusTextCount = transform.Find("Panel/Content/StatusTextCount").gameObject;
        postMediaImage = transform.Find("Panel/Content/PostMediaImage").gameObject;
        recordButton = transform.Find("Panel/Content/RecordButton").gameObject;
        recordButtonImage = transform.Find("Panel/Content/RecordButton/Image").GetComponent<Image>();
        recordOnButtonSprite = Resources.Load<Sprite>("Icon/icon_mic_on");
        recordOffButtonSprite = Resources.Load<Sprite>("Icon/icon_mic_off");
        

        dictationResult = "";
        dictationHypotheses = "";

        recordButtonImage.sprite = recordOffButtonSprite;
    }
	
	private void Update () {
        string resultString = userScreenName + dictationResult + dictationHypotheses;
        statusText.GetComponent<Text>().text = resultString;
        statusTextCount.GetComponent<Text>().text = resultString.Length.ToString();

	}

    

    public void Init()
    {
        if (quoteTweet != null) isQuote = true;

        if (replyTweet.user.screen_name != "")
        {
            userScreenName = "@" + replyTweet.user.screen_name + " ";
        }

        if (_mediaBinary.Length != 0)
        //if (hasMedia)
        {
            Debug.Log(_mediaBinary.Length);
            Debug.Log(System.Convert.ToBase64String(_mediaBinary));
            SetMedia();
        } else
        {
            Destroy(postMediaImage);
        }

        StartDictation();
    }

    public void TouchBeginChild(string childName)
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
            Destroy(gameObject);
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


    #region DictationRecognizer

    private void StartDictation()
    {
        //m_DictationRecognizer = new DictationRecognizer();

        //m_DictationRecognizer.DictationResult += (text, confidence) =>
        //{
        //    dictationHypotheses = "";
        //    dictationResult += text;
        //};
        //m_DictationRecognizer.DictationHypothesis += (text) =>
        //{
        //    dictationHypotheses = text;
        //};
        //m_DictationRecognizer.DictationComplete += (completionCause) =>
        //{
        //    if (completionCause != DictationCompletionCause.Complete)
        //    {
        //        GameObject.Find("SystemMessageHandler").
        //        GetComponent<SystemMessageHandler>().
        //        GenerateSystemMessage("Dication completed unsuccessfully: " + completionCause);
        //    }
        //};
        //m_DictationRecognizer.DictationError += (error, hresult) =>
        //{
        //    GameObject.Find("SystemMessageHandler").
        //        GetComponent<SystemMessageHandler>().
        //        GenerateSystemMessage("Dictation error:" + error + ", HResult = " + hresult);
        //};

        //m_DictationRecognizer.Start();

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





    #region About Recording
    //private void StartRecord()
    //{
    //    recordButtonImage.sprite = recordOnButtonSprite;

    //    audioSource.clip = Microphone.Start(
    //        Microphone.devices[0],
    //        false,
    //        GlobalConfig.googleSpeechLengthSec,
    //        GlobalConfig.googleSpeechFrequency
    //    );
    //    while (Microphone.GetPosition(null) <= 0) { }
    //    audioSource.Play();
    //}

    //private void StopRecord()
    //{
    //    Microphone.End(Microphone.devices[0]);
    //}
    //private void ToggleRecord()
    //{
    //    if (Microphone.IsRecording(Microphone.devices[0]))
    //    {
    //        StopRecord();
    //    }
    //    else
    //    {
    //        StartRecord();
    //    }
    //}

    #endregion

    #region About Volume
    float volumeSensitivity = 100f;
    float prevVolumeInfluence = 0.9f;
    float prevVolume;
    float volume;
    private void CalcVolume()
    {
        prevVolume = volume;
        volume = GetVolume() * volumeSensitivity * (1 - prevVolumeInfluence) + prevVolume * prevVolumeInfluence;
    }
    private float GetVolume()
    {


        float[] data = new float[64];
        float a = 0;
        audioSource.GetOutputData(data, 1);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;

    }

    #endregion

    #region About GoogleAPI
    //private void SpeechToText()
    //{
    //    StartCoroutine(googleAPI.Client.SpeechToText(
    //        audioSource.clip,
    //        new googleAPI.GoogleCallback(OnSpeechToText)
    //    ));
    //}
    //private void OnSpeechToText(bool success, string response)
    //{
    //    if (success)
    //    {
    //        googleAPI.ResponseBody Response = JsonUtility.FromJson<googleAPI.ResponseBody>(response);
    //        tweetText = Response.results[0].alternatives[0].transcript;
    //        statusText.GetComponent<Text>().text += tweetText;
    //    }
    //    else
    //    {
    //        Debug.Log(response);
    //    }
    //}
    #endregion
}
