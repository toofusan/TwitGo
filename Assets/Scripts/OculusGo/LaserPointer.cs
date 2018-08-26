using UnityEngine;

namespace OculusGo
{
    public class LaserPointer : MonoBehaviour
    {

        [SerializeField] private Transform _rightHandAnchor;
        [SerializeField] private Transform _leftHandAnchor;
        [SerializeField] private Transform _centerEyeAnchor;
        [SerializeField] private float _maxDistance = 100.0f;
        [SerializeField] private LineRenderer _laserPointerRenderer;

        private bool _isGrabbing;
        public bool isGrabbing () {
            return _isGrabbing;
        }

        private GameObject _grabTarget;
        private Transform _parentOfGrabTarget;
        private GameObject _touchTarget;

        private Transform Pointer
        {
            get
            {
                if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
                {
                    return _rightHandAnchor;
                }
                if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
                {
                    return _leftHandAnchor;
                }

                return _centerEyeAnchor;

            }
        }

        private void Start()
        {
            _touchTarget = null;
        }
	
        // Update is called once per frame
        private void Update ()
        {
            Transform pointer = Pointer;
            if (pointer == null || _laserPointerRenderer == null)
            {
                return;
            }

		
            Ray pointerRay = new Ray(pointer.position, pointer.forward);
            _laserPointerRenderer.SetPosition(0, pointerRay.origin);
            RaycastHit hitInfo;
        
            // なにかに当たっているとき
            if (Physics.Raycast(pointerRay, out hitInfo, _maxDistance))
            {
                _laserPointerRenderer.SetPosition(1, hitInfo.point);


                GameObject target = hitInfo.collider.gameObject;
                Touchable touchable = target.GetComponent<Touchable>();
                Grabbable grabbable = target.GetComponent<Grabbable>();
            
                // 何かを掴んでいるとき
                if (_isGrabbing)
                {
                    if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
                    {
                        _grabTarget.transform.SetParent(_parentOfGrabTarget);
                        _parentOfGrabTarget = null;
                        _grabTarget = null;
                        _isGrabbing = false;
                    }
                    if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
                    {
                        Vector3 direction = pointer.forward * OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;
                        if (_grabTarget != null) _grabTarget.transform.Translate(direction * 0.1f, Space.World);
                    }
                }
            
            
                // 何も掴んでいないとき
                else
                {          
                    // Touchableに当たっているとき
                    if (touchable != null)
                    {
                        _touchTarget = touchable.gameObject;
                        touchable.TouchBegin(this);
                    
                        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                        {
                            // if (_touchTarget.transform.parent != null) Debug.Log("self Debug.Log: TouchTrigger : " + _touchTarget.transform.parent.gameObject.name);
                            touchable.TouchTrigger(this);
                        }
                    }
                    else
                    {
                        ReleaseTouchTarget();
                        
                        // Grabbableに当たっているとき
                        if (grabbable != null)
                        {				
                            // ボタンを押すとcontrollerに追従するようにする
                            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                            {
                                _grabTarget = target;
                                _parentOfGrabTarget = _grabTarget.transform.parent;
                                _grabTarget.transform.SetParent(pointer);
                                _isGrabbing = true;
                            }
                        }
                    }
                
                }

            }
        
            // どこにも当たっていないとき
            else
            {
                _laserPointerRenderer.SetPosition(1, pointerRay.origin + pointerRay.direction * _maxDistance);
                ReleaseTouchTarget();
            }

        }
        
        private void ReleaseTouchTarget()
        {
            if (_touchTarget == null) return;
            _touchTarget.GetComponent<Touchable>().TouchEnd(this);
            _touchTarget = null;
        }
    }
}