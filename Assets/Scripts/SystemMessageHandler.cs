using UnityEngine;

public class SystemMessageHandler : MonoBehaviour {

    public GameObject systemMessagePanelPrefab;

	public void GenerateSystemMessage(string message)
    {
        GameObject systemMessagePanel = Instantiate(
            systemMessagePanelPrefab,
            transform.position,
            transform.localRotation
        );

        systemMessagePanel.GetComponent<SystemMessagePanel>().message = message;
        systemMessagePanel.GetComponent<SystemMessagePanel>().Init();

    }
}
