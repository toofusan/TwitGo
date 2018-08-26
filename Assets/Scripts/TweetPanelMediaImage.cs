using System.Collections;
using System.Linq;
using OculusGo;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Twity;
using Twity.DataModels.Entities;

public class TweetPanelMediaImage : Touchable {


    public GameObject MediaImagePanelPrefab;

    public Extended_Entities extendedEntities { get; set; }

    Transform rawImageTransform;
    RawImage rawImage;

    Vector2 rawImageRegularSize = new Vector2(320.0f, 180.0f);

    private VideoPlayer _VideoPlayer;


    public void Init()
    {
        rawImageTransform = transform.Find("RawImage");
        rawImage = rawImageTransform.GetComponent<RawImage>();

        ShowFirstMedia();
        
    }

    private void OnBecameVisible() {
        PlayVideo();
    }
    private void OnBecameInvisible() {
        PauseVideo();
    }

    public void PlayVideo() {
        if (_VideoPlayer != null) _VideoPlayer.Play();
    }
    public void PauseVideo() {
        if (_VideoPlayer != null) _VideoPlayer.Pause();
    }


    void ShowFirstMedia()
    {
        Media media = extendedEntities.media[0];

        switch(media.type)
        {
            case "photo":
                StartCoroutine(MediaImageHelper.PlayPhoto(transform, media.media_url, rawImageRegularSize, MediaCallback));
                break;
            case "video":
                _VideoPlayer = gameObject.AddComponent<VideoPlayer>();
                StartCoroutine(MediaImageHelper.PlayVideo(media, _VideoPlayer, rawImageRegularSize, false, MediaCallback));
                break;
        }
    }

    private void MediaCallback()
    {
        GetComponentInParent<TweetPanel>().SetBoxCollider();
    }



    public override void TouchBegin(LaserPointer pointer)
    {

    }

    public override void TouchTrigger(LaserPointer pointer)
    {
        PauseVideo();
        GetComponentInParent<TweetPanel>().GenerateMediaImagePanel(extendedEntities);

    }


}

