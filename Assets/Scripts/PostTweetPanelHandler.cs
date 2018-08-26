using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostTweetPanelHandler : MonoBehaviour {

	[SerializeField] private GameObject PostTweetPanelPrefab;
	private GameObject PostTweetPanel;
	private bool hasPostTweetPanel;
	

	public void GeneratePostTweetPanel() {
		if (hasPostTweetPanel) return;

        PostTweetPanel = Instantiate(
			PostTweetPanelPrefab,
			transform.position,
			Quaternion.identity
		);
        PostTweetPanel.transform.SetParent(transform);
        PostTweetPanel.GetComponent<PostTweetPanelForGo>().Init();
		hasPostTweetPanel = true;
	}

	public void DestroyPostTweetPanel() {
		if (!hasPostTweetPanel) return;
		Destroy(PostTweetPanel);
		hasPostTweetPanel = false;
	}
}

