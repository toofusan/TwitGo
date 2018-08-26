using OculusGo;
using UnityEngine;
using UnityEngine.Video;
using Twity;
using Twity.DataModels.Entities;

public class MediaImagePanelMediaImage: Touchable {

    public Media media;
    private VideoPlayer _VideoPlayer;

    Vector2 rawImageRegularSize = new Vector2(1280.0f, 1280.0f);

    public void Init()
    {

        switch (media.type)
        {
            case "photo":
                StartCoroutine(MediaImageHelper.PlayPhoto(transform, media.media_url, rawImageRegularSize, MediaPlayCallback));
                break;
            case "video":
                _VideoPlayer = gameObject.AddComponent<VideoPlayer>();
                StartCoroutine(MediaImageHelper.PlayVideo(media, _VideoPlayer, rawImageRegularSize, true, MediaPlayCallback));
                break;
        }

    }

    private void OnBecameVisible()
    {
        if (_VideoPlayer != null) _VideoPlayer.Play();
    }
    private void OnBecameInvisible()
    {
        if (_VideoPlayer != null) _VideoPlayer.Pause();
    }

    

    void MediaPlayCallback()
    {
        GetComponentInParent<MediaImagePanel>().SetBoxCollider();
    }

    #region Touch Event
    public override void TouchBegin(LaserPointer pointer)
    {
//        GenerateFrameImagePanel();
    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        GenerateFrameImagePanel();
    }

    void GenerateFrameImagePanel()
    {
        GameObject frameImageHandler = GameObject.Find("FrameImageHandler");
        frameImageHandler.GetComponent<FrameImageHandler>().GenerateFrameImagePanel(media);
    }
    #endregion

}
