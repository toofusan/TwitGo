
using UnityEngine;



public class NotificationUnitychan : MonoBehaviour {

    public float animSpeed = 0.5f;
    public float jumpPower = 3.0f;

    private Rigidbody rb;
    private Vector3 velocity;
    private float orgColHeight;
    private Vector3 orgVectorColCenter;
    private Animator anim;
    private AnimatorStateInfo currentBaseState;

    private GameObject cameraObject;

    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int sadState = Animator.StringToHash("Base Layer.Sad");


    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cameraObject = GameObject.Find("CenterEyeAnchor");

        anim.speed = animSpeed;
    }

    private void Update()
    {
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);

        if (currentBaseState.fullPathHash == jumpState)
        {
            anim.SetBool("Jump", false);
        } else if (currentBaseState.fullPathHash == sadState)
        {
            anim.SetBool("Sad", false);
        }
        
    }


    public void Response(string responseType)
    {
        if (!anim.IsInTransition(0))
        {
            //rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            anim.SetBool(responseType, true);
        }
    }

    public void Response(UnitychanResponseType type) {
        if (anim.IsInTransition(0)) return;

        anim.SetBool(type.ToString(), true);
    }

}

public enum UnitychanResponseType {
    Jump,
    Sad
}
