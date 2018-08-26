using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using Twity.DataModels.Entities;

public class MediaImageHelper {

    public delegate void MediaPlayCallback();

    #region About MediaType
    public static IEnumerator PlayPhoto(Transform target, string imageURL, Vector2 size, MediaPlayCallback callback = null)
    {
        yield return SetTexture(
            target.Find("RawImage"),
            imageURL,
            true
        );
        yield return SetTextureSize(target, size);
        if (callback != null) callback();
    }

    public static IEnumerator PlayVideo(Media media, VideoPlayer videoPlayer, Vector2 size, bool enableAudio, MediaPlayCallback callback = null)
    {
        RawImage rawImage = videoPlayer.transform.Find("RawImage").GetComponent<RawImage>();

        if (enableAudio) videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = media.video_info.variants.OrderByDescending(variant => variant.bitrate).First().url;
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(1);
        }

        rawImage.texture = videoPlayer.texture;
        rawImage.SetNativeSize();
        yield return SetTextureSize(videoPlayer.transform, size);
        videoPlayer.Play();

        if (callback != null) callback();
    }

    static IEnumerator PlayAnimationGif(Transform target, string imageURL, Vector2 size)
    {
        RawImage rawImage = target.Find("RawImage").GetComponent<RawImage>();

        return null;
    }
    #endregion


    #region Helper Methods
    static IEnumerator SetTextureSize(Transform target, Vector2 size)
    {
        RawImage rawImage = target.Find("RawImage").GetComponent<RawImage>();
        Vector2 rawImageNativeSize = rawImage.rectTransform.sizeDelta;
        float rawImageNativeAspect = rawImageNativeSize.y / rawImageNativeSize.x;

        rawImage.rectTransform.sizeDelta =
            rawImageNativeAspect <= 1f ?
            new Vector2(size.x, size.x * rawImageNativeAspect) :
            new Vector2(size.y / rawImageNativeAspect, size.y);

        if (target.GetComponent<BoxCollider>())
        {
            target.GetComponent<BoxCollider>().size = new Vector3(
            rawImage.rectTransform.sizeDelta.x,
            rawImage.rectTransform.sizeDelta.y,
            1f);
        }

        yield return new WaitForEndOfFrame();
    }

    static IEnumerator SetTexture(Transform target, string imageURL, bool isNativeSize = false)
    {
        RawImage rawimage = target.GetComponent<RawImage>();
        Rect rect = target.GetComponent<RectTransform>().rect;
        UnityWebRequest request = UnityWebRequest.Get(imageURL);
        #if UNITY_2017_2
            yield return request.Send();
        #endif
        #if UNITY_2017_3_OR_NEWER
            yield return request.SendWebRequest();
        #endif
        if (request.responseCode == 200)
        {
            Texture2D texture = new Texture2D((int)rect.width, (int)rect.height);
            texture.LoadImage(request.downloadHandler.data);
            rawimage.texture = texture;
            if (isNativeSize) rawimage.SetNativeSize();
        }
    }

#endregion


}
