using System.Collections;
using UnityEngine;

public class Toucher : MonoBehaviour {

    private ToucherManager toucherManager;

    public bool isRightHand;

    private OVRHapticsClip hapticsClip;

    private float intervalTime = 0.5f;

    private GameObject MenuPanelPrefab;
    private GameObject MenuPanel;
    private bool isShowingMenu;

    public void TouchBreak()
    {
        toucherManager.InitializeTouchParams();
    }

    private void Start()
    {
        toucherManager = GetComponentInParent<ToucherManager>();
        hapticsClip = defaultHapticsClip();
        MenuPanelPrefab = (GameObject)Resources.Load("Prefabs/MenuPanel");

        toucherManager.InitializeTouchParams();

    }

    private void Update()
    {
        if (isReadyToMenu)
        {
            ToggleMenu();
        }
    }

//    private void TouchBegin(Touchable touchable)
//    {
//        if (touchable == null) return;
//        if (isRightHand)
//        {
//            OVRHaptics.RightChannel.Mix(hapticsClip);
//        }
//        else
//        {
//            OVRHaptics.LeftChannel.Mix(hapticsClip);
//        }
//
//        toucherManager.SetIsTouching(true);
//        touchable.TouchBegin(this);
//    }
//
//    private void TouchEnd(Touchable touchable)
//    {
//        toucherManager.SetIsTouching(false);
//        StartCoroutine(TakeInterval());
//        touchable.TouchEnd(this);
//    }
    

//    private void OnTriggerEnter(Collider other)
//    {
//        Touchable touchable = other.GetComponent<Touchable>();
//        if (touchable == null) return;
//
//        if (isReadyToTouch) TouchBegin(touchable);
//
//    }
//
//    private void OnTriggerExit(Collider other)
//    {
//        Touchable touchable = other.GetComponent<Touchable>();
//        if (touchable == null) return;
//
//        if (toucherManager.isTouching) TouchEnd(touchable);
//    }


    #region Helper
    private void ToggleMenu()
    {
        if (toucherManager.isShowingMenu)
        {
            toucherManager.SetIsShowingMenu(false);
            Destroy(MenuPanel);
        }
        else
        {
            toucherManager.SetIsShowingMenu(true);
            MenuPanel = Instantiate(MenuPanelPrefab, transform.position, Quaternion.identity);
        }
    }

    private OVRHapticsClip defaultHapticsClip()
    {
        byte[] samples = new byte[8];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = 128;
        }
        return new OVRHapticsClip(samples, samples.Length);
    }

    private IEnumerator TakeInterval()
    {
        toucherManager.SetIsInInterval(true);
        yield return new WaitForSeconds(intervalTime);
        toucherManager.SetIsInInterval(false);
    }

    private bool isReadyToTouch
    {
        get
        {
            if (isRightHand)
            {
                return
                OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) >= 0.5f &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) == 0f &&
                !toucherManager.isTouching && !toucherManager.isInInterval;
            }
            else
            {
                return
                OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) >= 0.5f &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) == 0f &&
                !toucherManager.isTouching && !toucherManager.isInInterval;
            }
        }
    }

    private bool isReadyToMenu
    {
        get
        {
            return 
                !isRightHand &&
                OVRInput.GetDown(OVRInput.Button.Three) &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) == 0f &&
                OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) == 0f;
        }
    }

    #endregion
}
