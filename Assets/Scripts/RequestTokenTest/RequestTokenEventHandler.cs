using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestTokenEventHandler : MonoBehaviour {


    string twitterConsumerKey = "AtlfZeOEwVNmlE4b3alSuiZcD";

    // Use this for initialization
    void Start () {
        Twity.Oauth.consumerKey = twitterConsumerKey;
        StartCoroutine(Twity.Oauth.GenerateRequestToken(OnGenerateRequestToken));
	}
	
	void OnGenerateRequestToken(bool success, string response)
    {
        Debug.Log(response);
    }

}
