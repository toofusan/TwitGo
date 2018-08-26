using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using twitter;

namespace twitter {
	
	public class TwitterEvents: MonoBehaviour{

		public float radius = GlobalConfig.twitterPositionRadius;
		public int NumOfTweets = GlobalConfig.twitterNumOfTweets;

//		public static IEnumerator GetHomeTimeline() {
//			Dictionary<string, string> parameters = new Dictionary<string, string>();
//			parameters ["count"] = NumOfTweets.ToString ();
//			parameters ["exclude_replies"] = GlobalConfig.twitterExcludeReplies.ToString ();
//			StartCoroutine (twitter.Client.Get ("statuses/home_timeline", parameters, new twitter.TwitterCallback (this.OnGetStatusesHomeTimeline)));
//		}
//
//		public static IEnumerator GetUserTimeline(int id) {
//			Dictionary<string, string> parameters = new Dictionary<string, string>();
//			parameters ["id"] = id.ToString ();
//			parameters ["count"] = NumOfTweets.ToString ();
//			StartCoroutine (twitter.Client.Get ("statuses/user_timeline", parameters, new twitter.TwitterCallback (this.OnGetStatusesUserTimeline)));
//		}
//
//		public static IEnumerator PostStatusesUpdate(string status) {
//			Dictionary<string, string> parameters = new Dictionary<string, string>();
//			parameters ["status"] = status;
//			StartCoroutine (twitter.Client.Post ("statuses/update", parameters, new twitter.TwitterCallback (this.OnPostStatusesUpdate)));
//		}


	}
}
