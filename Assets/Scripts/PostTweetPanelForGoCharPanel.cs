using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostTweetPanelForGoCharPanel : Touchable
{

    private string character;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public void Init(float x, float y, string c)
    {
        character = c;
        if (character == "change") {
            transform.Find("Label").GetComponent<Text>().text = "゛゜小";
        } else {
            transform.Find("Label").GetComponent<Text>().text = character;
        }
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.8f);
        GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0f);

    }

    public override void TouchBegin(OculusGo.LaserPointer pointer)
    {
        base.TouchBegin(pointer);
    }
    

    public override void TouchTrigger(OculusGo.LaserPointer pointer)
    {
        transform.GetComponentInParent<PostTweetPanelForGoKeyboard>().ActionFromKeyboard(character);
        base.TouchTrigger(pointer);
    }

    public override void TouchEnd(OculusGo.LaserPointer pointer)
    {
        base.TouchEnd(pointer);
    }

}
