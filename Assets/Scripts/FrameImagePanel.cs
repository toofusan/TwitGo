
using UnityEngine;
using UnityEngine.Video;
using Twity.DataModels.Entities;

public class FrameImagePanel : MonoBehaviour {

    public Media media;
    private VideoPlayer _VideoPlayer;

    Vector2 rawImageRegularSize = new Vector2(1280.0f, 1280.0f);

    public void Init()
    {

        switch (media.type)
        {
            case "photo":
                StartCoroutine(MediaImageHelper.PlayPhoto(transform, media.media_url, rawImageRegularSize));
                break;
            case "video":
                _VideoPlayer = gameObject.AddComponent<VideoPlayer>();
                StartCoroutine(MediaImageHelper.PlayVideo(media, _VideoPlayer, rawImageRegularSize, false));
                break;
        }

    }


}

