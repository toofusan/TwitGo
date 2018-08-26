using System.Collections;
using UnityEngine;

namespace OculusGo {

    public class MenuHandler : MonoBehaviour {

        [SerializeField] private GameObject MenuPanelPrefab;
        private GameObject MenuPanel;
        private bool _hasMenuPanel;

        private LaserPointer laserPointer;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {

            laserPointer = gameObject.GetComponent<LaserPointer>();
            _hasMenuPanel = false;

        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (laserPointer.isGrabbing()) return;

            CheckInput();
        }

        private void CheckInput() {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                Vector2 touchPadPt = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                if (touchPadPt.x > -0.5f && touchPadPt.x < 0.5f && touchPadPt.y > 0.5f)
                {
                    GameObject.Find("PostTweetPanelHandler").GetComponent<PostTweetPanelHandler>().GeneratePostTweetPanel();
                }
                else if (touchPadPt.x > -0.5f && touchPadPt.x < 0.5f && touchPadPt.y > -0.5f && touchPadPt.y < 0.5f)
                {
                    ToggleMenu();
                }
                
                
            }
        }


        private void ToggleMenu() {
            if (_hasMenuPanel) {
                GameObject.Destroy(MenuPanel, 0.2f);
                _hasMenuPanel = false;
            } else {
                MenuPanel = Instantiate(MenuPanelPrefab, new Vector3(0f, 1f, 1f), Quaternion.identity);
                _hasMenuPanel = true;
            }
        }
    }


}