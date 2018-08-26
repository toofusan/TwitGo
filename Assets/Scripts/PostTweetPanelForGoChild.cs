using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostTweetPanelForGoChild : Touchable {

	public override void TouchBegin(OculusGo.LaserPointer pointer) {
		base.TouchBegin(pointer);
	}

	public override void TouchTrigger(OculusGo.LaserPointer pointer) {
        
		transform.GetComponentInParent<PostTweetPanelForGo>().ActionFromChild(gameObject.name);
	}

	public override void TouchEnd(OculusGo.LaserPointer pointer) {
        base.TouchEnd(pointer);
	}
	
}
