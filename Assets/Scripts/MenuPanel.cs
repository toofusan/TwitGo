using System.Collections;
using System.Reflection;
using OculusGo;
using UnityEngine;

public class MenuPanel : MonoBehaviour {

	float TweenTime = 0.6f;
	float TweenLength = 0.05f;

	public GameObject PostTweetWindowPrefab;

	private GameObject TweetEventHandler;

	// Use this for initialization
	void Start () {
//		ShowUp ();
		TweetEventHandler = GameObject.Find ("TweetEventHandler");
		StartCoroutine(RequestHelper.SetTextureRectSize(
			transform.Find("ProfileImage"),
			GlobalConfig.myTwitterInfo.profile_image_url
		));
		StartCoroutine("ShowChilds");
	}

	public void DestroyMenu() {
		StartCoroutine("ShowChilds");
	}
	
	private IEnumerator ShowChilds() {
		transform.Find ("ProfileImage").GetComponent<MenuPanelChild> ().ShowUp ();
		transform.Find ("ProfileImage").GetComponent<MenuPanelChild> ().FadeIn ();
		yield return new WaitForSeconds (0.05f);
		transform.Find ("PostTweetsButton").GetComponent<MenuPanelChild> ().ShowSlide ();
		transform.Find ("PostTweetsButton").GetComponent<MenuPanelChild> ().FadeIn ();
		yield return new WaitForSeconds (0.05f);
		transform.Find ("RefreshTimelineButton").GetComponent<MenuPanelChild> ().ShowSlide ();
		transform.Find ("RefreshTimelineButton").GetComponent<MenuPanelChild> ().FadeIn ();
		yield return new WaitForSeconds (0.05f);
		transform.Find ("ShowYourTweetsButton").GetComponent<MenuPanelChild> ().ShowSlide ();
		transform.Find ("ShowYourTweetsButton").GetComponent<MenuPanelChild> ().FadeIn ();
		yield return new WaitForSeconds (0.05f);
		transform.Find ("ShowFavoriteImageButton").GetComponent<MenuPanelChild> ().ShowSlide ();
		transform.Find ("ShowFavoriteImageButton").GetComponent<MenuPanelChild> ().FadeIn ();
		yield return new WaitForSeconds (0.05f);
		transform.Find ("TakeScreenshotButton").GetComponent<MenuPanelChild> ().ShowSlide ();
		transform.Find ("TakeScreenshotButton").GetComponent<MenuPanelChild> ().FadeIn ();
		yield return new WaitForSeconds (0.05f);
		transform.Find ("HelpButton").GetComponent<MenuPanelChild> ().ShowSlide ();
		transform.Find ("HelpButton").GetComponent<MenuPanelChild> ().FadeIn ();
	}

	void Tween() {

	}

	public void OnChildButtonPressed(string ChildName, LaserPointer pointer) {
		if (ChildName == "PostTweetsButton") {
            
			// GameObject PostTweetWindow = Instantiate (PostTweetWindowPrefab, new Vector3 (0, 1.5f, 1f), new Quaternion(0, 0, 0, 0));
            // PostTweetWindow.GetComponent<PostTweetPanel>().Init();
			GameObject.Find("PostTweetPanelHandler").GetComponent<PostTweetPanelHandler>().GeneratePostTweetPanel();
            DestroyMenuPanel(pointer);
        } else if (ChildName == "RefreshTimelineButton") {
			foreach(Transform tweetCard in TweetEventHandler.transform) {
				Destroy(tweetCard.gameObject);
			}
			TweetEventHandler.GetComponent<TweetEventHandler> ().GetHomeTimeLine ();
            DestroyMenuPanel(pointer);
        }
        else if (ChildName == "ShowYourTweetsButton") {
			foreach(Transform tweetCard in TweetEventHandler.transform) {
				Destroy(tweetCard.gameObject);
			}
			TweetEventHandler.GetComponent<TweetEventHandler> ().GetStatusesUserTimeline (GlobalConfig.myTwitterInfo.screen_name);
		// } else if (ChildName == "ResetBackgroundButton") {
		// 	foreach(Transform BackgroundImage in GameObject.Find ("BackgroundHandler").transform) {
		// 		Destroy(BackgroundImage.gameObject);
		// 	}
        //     DestroyMenuPanel(pointer);
		} else if (ChildName == "ShowFavoriteImageButton") {
            foreach (Transform tweetCard in TweetEventHandler.transform)
            {
                Destroy(tweetCard.gameObject);
            }
            TweetEventHandler.GetComponent<TweetEventHandler>().GetFavoritesList();
        } else if (ChildName == "TakeScreenshotButton") {
            GameObject.Find("TweetEventHandler").GetComponent<TweetEventHandler>().ShareScreenshot();
            DestroyMenuPanel(pointer);
        }
    }


    private void DestroyMenuPanel(LaserPointer pointer)
    {
//        pointer.TouchBreak();
        Destroy(gameObject);
    }
}
