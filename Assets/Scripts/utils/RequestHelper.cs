using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RequestHelper {


	public static IEnumerator SetTextureRectSize(Transform target, string imageURL) {
		RawImage rawimage = target.GetComponent<RawImage> ();
		Rect rect = target.GetComponent<RectTransform> ().rect;
		UnityWebRequest request = UnityWebRequest.Get (imageURL);
		yield return request.Send ();
		if (request.responseCode == 200) {
			Texture2D texture = new Texture2D ((int)rect.width, (int)rect.height);
			texture.LoadImage (request.downloadHandler.data);
			rawimage.texture = texture;
		}
	}

//	public delegate void SetTextureNativeSizeCallback(Transform target);
//	public static IEnumerator SetTextureNativeSize(Transform target, string imageURL, SetTextureNativeSizeCallback callback) {
//		RawImage rawimage = target.GetComponent<RawImage> ();
//		Rect rect = target.GetComponent<RectTransform> ().rect;
//		UnityWebRequest request = UnityWebRequest.Get (imageURL);
//		yield return request.Send ();
//		if (request.responseCode == 200) {
//			Texture2D texture = new Texture2D ((int)rect.width, (int)rect.height);
//			texture.LoadImage (request.downloadHandler.data);
//			rawimage.texture = texture;
//			rawimage.SetNativeSize ();
//			callback (target);
//		}
//	}
	public static IEnumerator SetTextureNativeSize(Transform target, string imageURL) {
		RawImage rawimage = target.GetComponent<RawImage> ();
		Rect rect = target.GetComponent<RectTransform> ().rect;
		UnityWebRequest request = UnityWebRequest.Get (imageURL);
		yield return request.Send ();
		if (request.responseCode == 200) {
			Texture2D texture = new Texture2D ((int)rect.width, (int)rect.height);
			texture.LoadImage (request.downloadHandler.data);
			rawimage.texture = texture;
			rawimage.SetNativeSize ();
		}
	}

    //	public static IEnumerator SetVideoTexture(Texture2D texture, string ImageURL) {
    //		UnityWebRequest request = UnityWebRequest.Get (ImageURL);
    //		yield return request.Send ();
    //		if (request.responseCode == 200) {
    //			texture.LoadImage (request.downloadHandler.data);
    //			RawImage.texture = texture;
    //			RawImage.SetNativeSize ();
    //		}
    //	}

    public static System.DateTime CreateDateTime(string created_at) {
		return System.DateTime.ParseExact (
			created_at,
			"ddd MMM dd HH:mm:ss zzz yyyy",
			System.Globalization.DateTimeFormatInfo.InvariantInfo
		);
	}

	public static string CreateAtFormat(System.DateTime createDateTime) {
		System.DateTime now = System.DateTime.Now;
		System.TimeSpan dt = now - createDateTime;
		string dtString;
		if (dt.Days > 0) {
			dtString = "";
		} else if (dt.Hours > 0) {
			dtString = dt.Hours.ToString("0") + "時間前";
		} else if(dt.Minutes > 0) {
			dtString = dt.Minutes.ToString("0") + "分前";
		} else {
			dtString = dt.Seconds.ToString("0") + "秒前";
		}
		return System.String.Format(
			"{0}　{1}",
			dtString,
			createDateTime.ToString("yyyy年M月d日 H:mm")
		);
	}

}
