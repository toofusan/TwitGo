using UnityEngine;
using UnityEngine.UI;

public class SystemMessagePanel : MonoBehaviour {

    public string message;

    public void Init()
    {
        transform.Find("Panel/DescriptionText").GetComponent<Text>().text = message;
    }

    public void DestroyPanel()
    {
        Destroy(gameObject);
    }
}
