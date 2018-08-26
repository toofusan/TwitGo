using UnityEngine;
using Twity;
using Twity.DataModels.Entities;

public class FrameImageHandler : MonoBehaviour {

    public GameObject FrameImagePrefab;

    public void GenerateFrameImagePanel(Media media)
    {
        GameObject frameImagePanel = Instantiate(
                FrameImagePrefab,
                transform.position,
                transform.rotation
            );
        frameImagePanel.transform.SetParent(transform);
        frameImagePanel.transform.Translate(new Vector3(Random.Range(0f, 6f), 0, 0));
        frameImagePanel.GetComponent<FrameImagePanel>().media = media;
        frameImagePanel.GetComponent<FrameImagePanel>().Init();
    }

    public void ClearFrameImagePanels()
    {
        foreach(Transform FrameImagePanel in transform)
        {
            Destroy(FrameImagePanel.gameObject);
        }
    }

}
