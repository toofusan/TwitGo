using UnityEngine;

public class ToucherManager : MonoBehaviour {

    public Toucher rightToucher { get; set; }
    public Toucher leftToucher { get; set; }

    private bool _isTouching;
    private bool _isInInterval;
    private bool _isShowingMenu;

    public bool isTouching
    {
        get
        {
            return _isTouching;
        }
    }
    public bool isInInterval
    {
        get
        {
            return _isInInterval;
        }
    }
    public bool isShowingMenu
    {
        get
        {
            return _isShowingMenu;
        }
    }

    #region SetTouchParameters
    public void InitializeTouchParams()
    {
        _isTouching = false;
        _isInInterval = false;
        _isShowingMenu = false;
    }

    public void SetIsTouching(bool _isTouching)
    {
        this._isTouching = _isTouching;
    }
    public void SetIsInInterval(bool _isInInterval)
    {
        this._isInInterval = _isInInterval;
    }
    public void SetIsShowingMenu(bool _isShowingMenu)
    {
        this._isShowingMenu = _isShowingMenu;
    }

    #endregion


    private void Start()
    {
        Invoke("GeneratePlayer", 1f);

    }


    #region GenerateToucher
    void GeneratePlayer()
    {
        GameObject rightHand = transform.Find("hand_right/hand_right_renderPart_0/hands:r_hand_world/hands:b_r_hand/hands:b_r_grip").gameObject;
        GameObject leftHand = transform.Find("hand_left/hand_left_renderPart_0/hands:l_hand_world/hands:b_l_hand/hands:b_l_grip").gameObject;
        GameObject rightIndexFinger = transform.Find("hand_right/hand_right_renderPart_0/hands:r_hand_world/hands:b_r_hand/hands:b_r_index1/hands:b_r_index2/hands:b_r_index3").gameObject;
        GameObject leftIndexFinger = transform.Find("hand_left/hand_left_renderPart_0/hands:l_hand_world/hands:b_l_hand/hands:b_l_index1/hands:b_l_index2/hands:b_l_index3").gameObject;

        GenerateToucher(rightIndexFinger, true);
        GenerateToucher(leftIndexFinger, false);
        GenerateHandCollider(rightHand, true);
        GenerateHandCollider(leftHand, false);
    }

    void GenerateToucher(GameObject finger, bool isRightHand)
    {
        finger.tag = "Player";

        BoxCollider fingerCollider = finger.AddComponent<BoxCollider>();
        fingerCollider.isTrigger = true;
        fingerCollider.center = new Vector3(0.015f, 0f, 0f);
        fingerCollider.size = new Vector3(0.03f, 0.015f, 0.015f);

        Rigidbody finderRigidbody = finger.AddComponent<Rigidbody>();
        finderRigidbody.useGravity = false;

        Toucher fingerToucher = finger.AddComponent<Toucher>();
        fingerToucher.isRightHand = isRightHand;

    }

    void GenerateHandCollider(GameObject hand, bool isRightHand)
    {
        hand.tag = "Player";

        BoxCollider handCollider = hand.AddComponent<BoxCollider>();
        handCollider.isTrigger = true;
        handCollider.center = new Vector3(0, 0, 0);
        handCollider.size = new Vector3(0.02f, 0.1f, 0.1f);

        Rigidbody handRigidbody = hand.AddComponent<Rigidbody>();
        handRigidbody.useGravity = false;

        Grabber handGrabber = hand.AddComponent<Grabber>();
        handGrabber.isRightHand = isRightHand;
    }

    void GenerateFingerParticleSystem(GameObject indexFinger)
    {
        ParticleSystem ps = indexFinger.AddComponent<ParticleSystem>();
        var ma = ps.main;
        ma.startLifetime = 1f;
    }

    #endregion

}
