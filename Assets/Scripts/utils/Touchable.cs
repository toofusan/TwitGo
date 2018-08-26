using System.Collections;
using OculusGo;
using UnityEngine;
using UnityEngine.UI;

public class Touchable : MonoBehaviour
{

    #region TouchEvents
    virtual public void TouchBegin(LaserPointer pointer)
    {
        if (GetComponent<Image>() != null) GetComponent<Image>().color = new Color(0.7f, 0.7f, 1f, 1f);
    }

    virtual public void TouchEnd(LaserPointer pointer)
    {
        if(GetComponent<Image>() != null) GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.8f);
    }

    virtual public void TouchTrigger(LaserPointer pointer)
    {
        if (GetComponent<Image>() != null) StartCoroutine(FeedbackForTouchTrigger());
    }



    #endregion


    private IEnumerator FeedbackForTouchTrigger() {
        GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.8f, 1f);
        yield return new WaitForSeconds(0.2f);
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.8f);
    }
}
