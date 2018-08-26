
using UnityEngine;


public class Background : MonoBehaviour {

	public GameObject BackgroundImagePrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateBackgroundImage(string
		mediaType, string imageURL, string videoURL) {
		GameObject BackgroundImage = (GameObject)Instantiate (
			BackgroundImagePrefab,
			new Vector3 (0, 4f, GlobalConfig.backgroundImagePositionRadius),
			Quaternion.identity
		);
		BackgroundImage.transform.SetParent (transform);
		BackgroundImage.GetComponent<BackgroundImage>().imageURL = imageURL;
		BackgroundImage.GetComponent<BackgroundImage>().mediaType = mediaType;
		BackgroundImage.GetComponent<BackgroundImage>().videoURL = videoURL;
	}
}
