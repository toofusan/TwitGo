#if UNITY_IOS
#define USE_READPIXELS
#endif

using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ReadPixels : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
	[DllImport("glutil")]
	private static extern IntPtr getRenderEventFunc();
	[DllImport("glutil")]
	private static extern void libglutil_SetPixels(int intPtr, int width, int height);

	private AndroidJavaClass m_NativePlugin;
#elif UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern IntPtr _MovieRecorderPlugin_Init();
	[DllImport("__Internal")]
	private static extern void _MovieRecorderPlugin_Destroy(IntPtr instance);
	[DllImport("__Internal")]
	private static extern void _MovieRecorderPlugin_StartRecord(IntPtr instance, string fileName);
	[DllImport("__Internal")]
	private static extern void _MovieRecorderPlugin_StopRecord(IntPtr instance);
	[DllImport("__Internal")]
	private static extern void _MovieRecorderPlugin_PushVideo(IntPtr instance, IntPtr data, int width, int height);
	[DllImport("__Internal")]
	private static extern void _MovieRecorderPlugin_PushAudio(IntPtr instance, IntPtr data, int datasize);

	private IntPtr m_NativePlugin;
#endif

	private const int CaptureTextureWidth = 1024;
	private const int CaptureTextureHeight = 1024;

#if USE_READPIXELS
	private Texture2D m_TargetTexture2D;
#endif
	private RenderTexture m_TargetTexture;
	private bool m_Enable;
	private bool m_Recording;
//	private MicController m_Mic;

	public RenderTexture GetRenderTexture()
	{
		return m_TargetTexture;
	}

	public void SetEnable(bool flag)
	{
		m_Enable = flag;
	}

//	public void SetMic(MicController mic)
//	{
//		m_Mic = mic;
//	}

	public bool IsRecording()
	{
		return m_Recording;
	}

	public void StartRecord(string filePath, string fileName)
	{
		var fullPath = Path.Combine(filePath, fileName);
		if (File.Exists(fullPath))
		{
			File.Delete(fullPath);
		}
#if UNITY_IPHONE && !UNITY_EDITOR
		else if (!Directory.Exists(filePath))
		{
			Directory.CreateDirectory(filePath);
		}
		_MovieRecorderPlugin_StartRecord(m_NativePlugin, fullPath);
#elif UNITY_ANDROID && !UNITY_EDITOR
		m_NativePlugin.CallStatic<bool>("record", fullPath);
#endif
//		if (m_Mic != null)
//		{
//			m_Mic.StartRecord();
//		}
		m_Recording = true;
	}

	public void StopRecord()
	{
#if UNITY_IPHONE && !UNITY_EDITOR
		_MovieRecorderPlugin_StopRecord(m_NativePlugin);
#elif UNITY_ANDROID && !UNITY_EDITOR
		m_NativePlugin.CallStatic("stop");
#endif
//		if (m_Mic != null)
//		{
//			m_Mic.StopRecord();
//		}
		m_Recording = false;
	}

	private void Awake()
	{
		m_TargetTexture = new RenderTexture(CaptureTextureWidth, CaptureTextureHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		m_TargetTexture.Create();
		m_TargetTexture.anisoLevel = 0;
		m_TargetTexture.filterMode = FilterMode.Point;

		var cam = GetComponent<Camera>();
		cam.targetTexture = m_TargetTexture;

		transform.Find("RawImage").GetComponent<RawImage>().texture = m_TargetTexture;

#if USE_READPIXELS
		m_TargetTexture2D = new Texture2D(CaptureTextureWidth, CaptureTextureHeight, TextureFormat.ARGB32, false);
		m_TargetTexture2D.filterMode = FilterMode.Point;
#endif
	}

	private void Start()
	{
		m_Enable = true;
		m_Recording = false;
#if UNITY_ANDROID && !UNITY_EDITOR
		m_NativePlugin = new AndroidJavaClass("jp.ne.pickle.libpixelreads.UnityConnect");
		m_NativePlugin.CallStatic("initialize", m_TargetTexture.width, m_TargetTexture.height, 0, AudioSettings.outputSampleRate);
#endif
	}

	private void Update()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		m_NativePlugin.CallStatic("update");
#endif
	}

	private void OnPostRender()
	{
		if (!m_Enable)
		{
			return;
		}
		if (!m_Recording)
		{
			return;
		}
#if USE_READPIXELS
		RenderTexture.active = m_TargetTexture;
		m_TargetTexture2D.ReadPixels(new Rect(0, 0, m_TargetTexture.width, m_TargetTexture.height), 0, 0);
		m_TargetTexture2D.Apply();

		var col = m_TargetTexture2D.GetPixels32();

		var handle = default(GCHandle);
		try
		{
			handle = GCHandle.Alloc(col, GCHandleType.Pinned);
			var ptr = handle.AddrOfPinnedObject();

#if UNITY_IOS
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
/*
			var length = col.Length * 4;
			byte[] bytes = new byte[length];
			Marshal.Copy(ptr, bytes, 0, length);
			m_NativePlugin.CallStatic("transByteData", bytes);		// too heavy!!
*/
			// C# -> C++ -> Java (very fast)
			libglutil_SetPixels(ptr.ToInt32(), m_TargetTexture.width, m_TargetTexture.height);
#endif
		}
		finally
		{
			if (handle != default(GCHandle))
				handle.Free();
		}
#else
#if UNITY_ANDROID && !UNITY_EDITOR
		if (false)
		{
			GL.IssuePluginEvent(getRenderEventFunc(), m_TargetTexture.GetNativeTexturePtr().ToInt32());
		}
		else
		{
			m_NativePlugin.CallStatic("sendRgbaFrame", m_TargetTexture.GetNativeTexturePtr().ToInt32());
			GL.InvalidateState();
		}
#endif
#endif
//		if (m_Mic != null)
//		{
//			var waveData = m_Mic.GetWaveData();
//			//Debug.Log("_MovieRecorderPlugin_PushAudio1:" + waveData.Length);
//			if (waveData.Length != 0)
//			{
//				SendWaveData(waveData);
//			}
//			else
//			{
//				Debug.Log("No Wave Data");
//			}
//		}
	}

	private void SendWaveData(byte[] waveData)
	{
#if UNITY_IPHONE && !UNITY_EDITOR
		GCHandle dataHandle = GCHandle.Alloc(waveData, GCHandleType.Pinned);
		//_MovieRecorderPlugin_PushAudio(m_NativePlugin, dataHandle.AddrOfPinnedObject(), waveData.Length);
		dataHandle.Free();
#elif UNITY_ANDROID && !UNITY_EDITOR
		m_NativePlugin.CallStatic("sendWaveData", waveData);
#endif
	}
}
