using System.Collections.Generic;
using UnityEngine;
using Twity;
using Twity.DataModels.StreamMessages;

public class NotificationHandler : MonoBehaviour {

    public GameObject notificationPanelPrefab;

    private GameObject unitychan;

    private void Start()
    {
        unitychan = transform.Find("SD_unitychan_humanoid").gameObject;
    }

    public void ShowNotification(StreamEvent streamEvent)
    {
        if (IsMyAction(streamEvent)) return;

        GenerateNotificationPanel(streamEvent);
        MakeUnitychan(CheckUnitychanResponse(streamEvent));
    }

    public void ShowNotification(string noticeText, UnitychanResponseType type) {
        GenerateNotificationPanel(noticeText);
        MakeUnitychan(type);
    }

    private void GenerateNotificationPanel(StreamEvent streamEvent)
    {
        GameObject notificationPanel = Instantiate(
            notificationPanelPrefab,
            transform.localPosition,
            transform.localRotation);
        notificationPanel.transform.SetParent(transform);
        notificationPanel.transform.Translate(new Vector3(0.16f, 1f, 0));
        notificationPanel.GetComponent<NotificationPanel>().Init(streamEvent);
    }
    private void GenerateNotificationPanel(string noticeText)
    {
        GameObject notificationPanel = Instantiate(
            notificationPanelPrefab,
            transform.localPosition,
            transform.localRotation);
        notificationPanel.transform.SetParent(transform);
        notificationPanel.transform.Translate(new Vector3(0.16f, 1f, 0));
        notificationPanel.GetComponent<NotificationPanel>().Init(noticeText);
    }

    private void MakeUnitychan(string responseType)
    {
        unitychan.GetComponent<NotificationUnitychan>().Response(responseType);
    }
    private void MakeUnitychan(UnitychanResponseType type) {
        unitychan.GetComponent<NotificationUnitychan>().Response(type);
    }

    private bool IsMyAction(StreamEvent streamEvent) {
        return streamEvent.source.id == GlobalConfig.myTwitterInfo.id;
    }




    private string CheckUnitychanResponse(StreamEvent streamEvent)
    {
        List<string> eventNameForUnitychanJump = new List<string>() { "favorite", "follow" };
        List<string> eventNameForUnitychanSad = new List<string>() { "unfavorite" };

        if (eventNameForUnitychanJump.IndexOf(streamEvent.event_name) != -1)
        {
            return "Jump";
        }
        else if (eventNameForUnitychanSad.IndexOf(streamEvent.event_name) != -1)
        {
            return "Sad";
        }
        else
        {
            return null;
        }
    }
}
