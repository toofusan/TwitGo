using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BackgroundImage : MonoBehaviour {

	public string mediaType {get; set;}
	public string imageURL { get; set; }
	public string videoURL { get; set; }

	private VideoPlayer videoPlayer;
	private Transform rawImageTransform;

	void Start () {
		transform.RotateAround (transform.parent.transform.position, new Vector3 (0, 1.0f, 0), Random.Range (0.0f, 360.0f));
		rawImageTransform = transform.Find ("RawImage");


		if (mediaType == "photo") {
			StartCoroutine (ShowImage ());
		} else if (mediaType == "video") {
			StartCoroutine (PlayVideo ());
		}

		Vector2 nativeSize = rawImageTransform.GetComponent<RawImage>().rectTransform.sizeDelta;


	}

	IEnumerator ShowImage() {
		yield return StartCoroutine(RequestHelper.SetTextureNativeSize(
			rawImageTransform,
			imageURL
		));
	}

	IEnumerator PlayVideo() {
		videoPlayer = rawImageTransform.gameObject.AddComponent<VideoPlayer> ();
		videoPlayer.playOnAwake = false;
		videoPlayer.isLooping = true;
		videoPlayer.source = VideoSource.Url;
		videoPlayer.url = videoURL;
		videoPlayer.Prepare ();

		WaitForSeconds waitTime = new WaitForSeconds (1);
		while (!videoPlayer.isPrepared) {
			Debug.Log ("Now Preparing...");
			yield return waitTime;
		}
		//		yield return videoPlayer.isPrepared;
		Debug.Log ("Prepared");

		rawImageTransform.GetComponent<RawImage>().texture = videoPlayer.texture;
		rawImageTransform.GetComponent<RawImage> ().SetNativeSize ();
		videoPlayer.Play ();
	}

}
