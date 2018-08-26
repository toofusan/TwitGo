using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BrowserPanelImage : MonoBehaviour
{

	private RawImage _webview;
	private bool _webviewEnable;
	private Texture2D _webviewTex;
	
	public string url { get; set; }
	
	
#if UNITY_ANDROID
	private AndroidJavaClass _nativePlugin;
	
//	private void Start ()
//	{
//
//		int posX = 100;
//		int posY = 320;
//		int texWidth = 1024;
//		int texHeight = 1024;
//		var aspect = 1136.0f / Screen.width;
//
////		_webviewEnable = true;
//		Init((Screen.width / 2) + (int)(posX / aspect), (Screen.height / 2) - (int)(posY / aspect), texWidth, texWidth, texWidth / 2, texHeight / 2);
//		
//		_webviewTex = new Texture2D(texWidth / 2, texHeight / 2, TextureFormat.ARGB32, false);
//		_webview = transform.Find("RawImage").GetComponent<RawImage>();
//		_webview.texture =  _webviewTex;
//		var rt = _webview.GetComponent<RectTransform>();
//		rt.sizeDelta = new Vector2(texWidth * aspect, texHeight * aspect);
//		rt.localPosition = new Vector3(posX, posY, 0);
//
//	}

	public void Init()
	{
		Debug.Log("selfLog: BrowserPanelImage.Init");
		int posX = 0;
		int posY = 0;
		int texWidth = 1024;
		int texHeight = 768;
		var aspect = 1136.0f / Screen.width;
		Debug.Log("selfLog: Screen.width: " + Screen.width + " aspect: " + aspect);

		_webviewEnable = true;
		Init(Screen.width / 2 + (int)(posX / aspect), Screen.height / 2 - (int)(posY / aspect), texWidth, texWidth, texWidth / 2, texHeight / 2);
		
		_webviewTex = new Texture2D(texWidth / 2, texHeight / 2, TextureFormat.ARGB32, false);
//		_webviewTex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
		_webview = transform.Find("RawImage").GetComponent<RawImage>();
		_webview.texture =  _webviewTex;
		var rt = _webview.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(texWidth * aspect, texHeight * aspect);
//		rt.sizeDelta = new Vector2(texWidth, texHeight);
		rt.localPosition = new Vector3(posX, posY, 0);
		
		LoadUrl(url);
		SetVisible(true);
	}
	
	// Update is called once per frame
	private void Update () {
		Debug.Log("selfLog: AndroidJavaObject: Update");
		if (!_webviewEnable)
		{
			Debug.Log("selfLog: _webviewEnable is null");
			return;
		}


		var data = GetWebTexturePixel();
		if (data != null)
		{
			Debug.Log("selfLog: GetWebTexturePixel is not null");
			_webviewTex.LoadRawTextureData(data);
			_webviewTex.Apply();
		}
	}


	public void Init(int posX, int posY, int width, int height, int texWidth, int texHeight)
	{
		if (!Application.isEditor)
		{
			Debug.Log("selfLog: AndroidJavaObject.Init");
			_nativePlugin = new AndroidJavaClass("jp.ne.pickle.libwebtexture.UnityConnect");
			_nativePlugin.CallStatic("initialize", posX, posY, width, height, texWidth, texHeight);
		}
	}

	public void SetPosition(int posX, int posY)
	{
		if (!Application.isEditor)
		{
			Debug.Log("selfLog: AndroidJavaObject.SetPosition");
			_nativePlugin.CallStatic("setPosition", posX, posY);
		}
	}

	public void SetWebTexture(int textureId, int width, int height)
	{
		if (!Application.isEditor)
		{
			Debug.Log("selfLog: AndroidJavaObject.SetWebTexture");
			_nativePlugin.CallStatic("setTexture", textureId, width, height);
		}
	}

	public int GetWebTexture()
	{
		if (!Application.isEditor)
		{
			Debug.Log("selfLog: AndroidJavaObject.GetWebTexture");
			return _nativePlugin.CallStatic<int>("getTexture");
		}

		return 0;
	}

	public byte[] GetWebTexturePixel()
	{
		if (!Application.isEditor)
		{
			Debug.Log("selfLog: AndroidJavaObject.GetWebTexturePixel");
			return _nativePlugin.CallStatic<byte[]>("getPixel");
		}

		return null;
	}

	public void LoadUrl(string url)
	{
		if (!Application.isEditor)
		{
			Debug.Log("selfLog: AndroidJavaObject.LoadUrl: " + url);
			_nativePlugin.CallStatic("loadUrl", url);
		}
	}

	public void SetVisible(bool visible)
	{
		_webviewEnable = visible;
		if (!Application.isEditor)
		{
			Debug.Log("selfLog: AndroidJavaObject.SetVisible");
			_nativePlugin.CallStatic("setVisible", visible);
		}
	}
#else
[DllImport("__Internal")]
	private static extern IntPtr _WebViewPlugin_Init(string gameObject);
	[DllImport("__Internal")]
	private static extern int _WebViewPlugin_Destroy(IntPtr instance);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_SetMargins(
		IntPtr instance, int left, int top, int right, int bottom);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_SetVisibility(
		IntPtr instance, bool visibility);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_LoadURL(
		IntPtr instance, string url);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_EvaluateJS(
		IntPtr instance, string url);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_SetFrame(
		IntPtr instance, int x, int y, int width, int height);
	[DllImport("__Internal")]
	private static extern void _WebViewPlugin_GetPixel(IntPtr instance, ref IntPtr data);

	private IntPtr webView;
	private int m_width, m_height;

	private void Start()
	{
		string url = "https://www.yahoo.co.jp/";
		int posX = -100;
		int posY = 100;
		int texWidth = 512;
		int texHeight = 512;
		var aspect = 1136.0f / Screen.width;

		Debug.Log("Width:" + Screen.width + ",height:" + Screen.height);
		m_WebViewEnable = true;
		int rw = (int)(texWidth / aspect);
		int rh = (int)(texWidth / aspect);
		rw = (rw + 3) & ~3;
		rh = (rh + 3) & ~3;
		Init((int)(posX / aspect), (int)(posY / aspect), rw, rh, texWidth / 2, texHeight / 2);
		var texture2D = new Texture2D(rw, rh, TextureFormat.ARGB32, false);

		m_WebView = GameObject.Find("RenderCanvas/WebView").GetComponent<RawImage>();
		m_WebView.texture = texture2D;
		var rt = m_WebView.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(texWidth, texHeight);
		rt.localPosition = new Vector3(posX, posY, 0);
		LoadUrl(url);
		SetVisible(true);
	}

	private void Update()
	{
		if (!m_WebViewEnable)
		{
			return;
		}

		var data = GetWebTexturePixel();
		if (data != null)
		{
			var tex = (Texture2D)m_WebView.texture;
			tex.LoadRawTextureData(data);
			tex.Apply();
		}
	}

	public void Init(int posX, int posY, int width, int height, int texWidth, int texHeight)
	{
		m_width = width;
		m_height = height;
		webView = _WebViewPlugin_Init("name");
		SetPosition(posX, posY);
	}

	public void SetPosition(int posX, int posY)
	{
		if (webView == IntPtr.Zero) return;
		_WebViewPlugin_SetFrame(webView, posX, posY, m_width, m_height);
	}

	public void SetWebTexture(int textureId, int width, int height){}

	public int GetWebTexture()
	{
		return 0;
	}

	public byte[] GetWebTexturePixel()
	{
		if (webView == IntPtr.Zero) return null;
		byte[] data = new byte[m_width * m_height * 4];
#if true
		var handle = default(GCHandle);
		try
		{
			handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			var ptr = handle.AddrOfPinnedObject();
			_WebViewPlugin_GetPixel(webView, ref ptr);
		}
		finally
		{
			if (handle != default(GCHandle))
				handle.Free();
		}
#endif
		return data;
	}

	public void LoadUrl(string url)
	{
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_LoadURL(webView, url);
	}

	public void SetVisible(bool visible)
	{
		m_WebViewEnable = visible;
		if (webView == IntPtr.Zero)
			return;
		_WebViewPlugin_SetVisibility(webView, visible);
	}
#endif
}
