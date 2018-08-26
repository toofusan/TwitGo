using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using Twity;
using Twity.DataModels.Entities;

public class MediaImagePanel : Grabbable {

    private VideoPlayer videoPlayer;
    private VideoSource videoSource;
    private AudioSource audioSouce;

    Vector2 rawImageRegularSize = new Vector2(640.0f, 360.0f);

    public Extended_Entities extendedEntities { get; set; }
    Media[] mediaArray;
    readonly int maxMediaArrayLength = 4;

    private readonly float _positionY = 1.5f;
    private float offsetY;
    private readonly float _offsetZ = -0.1f;


    public void Init()
    {
        offsetY = _positionY - transform.position.y;

        mediaArray = extendedEntities.media;
        for (int i = 0; i < maxMediaArrayLength; i++)
        {
            if (i >= mediaArray.Length)
            {
                Destroy(transform.Find("Panel/MediaImages/MediaImage" + i).gameObject);
            }
            else
            {
                Transform rawImageTransform = transform.Find("Panel/MediaImages/MediaImage" + i);
                rawImageTransform.GetComponent<MediaImagePanelMediaImage>().media = mediaArray[i];
                rawImageTransform.GetComponent<MediaImagePanelMediaImage>().Init();
            }
        }
        SetPosition(_positionY);
        Move(true);


    }

    void SetPosition(float position)
    {
        transform.Translate(new Vector3(0, 0, _offsetZ));
    }

    public void Move(bool isAppearance)
    {
        Hashtable moveArgs = this.moveArgs(isAppearance);
        iTween.MoveBy(gameObject, moveArgs);

        Hashtable scaleArgs = this.scaleArgs(isAppearance);
        iTween.ValueTo(gameObject, scaleArgs);
    }

    private Hashtable moveArgs(bool isAppearance)
    {
        float amountZ = isAppearance ? _offsetZ : -_offsetZ;
        float amountY = isAppearance ? offsetY : -offsetY;

        Hashtable args = new Hashtable();
        args.Add("easetype", "easeInOutQuad");
        args.Add("time", 0.2f);
        args.Add("oncomplete", "SetBoxCollider");
        args.Add("islocal", true);
        args.Add("amount", new Vector3(0, amountY, amountZ));

        return args;
    }
    private Hashtable scaleArgs(bool isAppearance)
    {
        float from = isAppearance ? 0.0f : 1.0f;
        float to   = isAppearance ? 1.0f : 0.0f;

        Hashtable args = new Hashtable();
        args.Add("time", 0.2f);
        
        args.Add("easetype", "easeInOutQuad");
        args.Add("onupdate", "FadeUpdate");
        args.Add("from", from);
        args.Add("to", to);
        return args;
    }
    void FadeUpdate(float value)
    {
        GetComponent<CanvasGroup>().alpha = value;
        GetComponent<RectTransform>().localScale = new Vector3(value, value, value);
    }



    public void SetBoxCollider()
    {
        RectTransform panel = transform.Find("Panel").GetComponent<RectTransform>();
        Vector2 panelSize = panel.sizeDelta;
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(panelSize.x, panelSize.y, 1f);
    }
}
