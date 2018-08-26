package jp.ne.pickle.libwebtexture;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.net.Uri;
import android.opengl.GLSurfaceView;
import android.os.Build;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.webkit.CookieManager;
import android.webkit.CookieSyncManager;
import android.webkit.HttpAuthHandler;
import android.webkit.JavascriptInterface;
import android.webkit.WebChromeClient;
import android.webkit.WebResourceResponse;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;

import com.self.viewtoglrendering.GLLinearLayout;
import com.self.viewtoglrendering.GLWebView;
import com.self.viewtoglrendering.ViewToGLRenderer;
import com.unity3d.player.UnityPlayer;

import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.ByteBuffer;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.Random;

import jp.ne.pickle.libpixelreads.TextureCapture;

class CWebViewPluginInterface {
    private UnityConnect mPlugin;
    private String mGameObject;

    public CWebViewPluginInterface(UnityConnect plugin, String gameObject) {
        mPlugin = plugin;
        mGameObject = gameObject;
    }

    @JavascriptInterface
    public void call(final String message) {
        call("CallFromJS", message);
    }

    public void call(final String method, final String message) {
        if (mGameObject == null || mGameObject == "" || method == null || method == "")
        {
            return;
        }
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mPlugin.IsInitialized()) {
                String tmpMessage = (message == null)? "" : message;
                UnityPlayer.UnitySendMessage(mGameObject, method, tmpMessage);
            }
        }});
    }
}

public class UnityConnect {
    private static UnityConnect m_Instance;

    private RelativeLayout mLayout;
    private GLSurfaceView mGLSurfaceView;
    private ViewToGLRenderer mViewToGlRenderer;
    private GLLinearLayout mGlLayout;
    private WebView mWebView;
    private String mLoadUrl;
    private int mPosX, mPosY;
    //private GLWebView mWebView;

    private CWebViewPluginInterface mWebViewPlugin;
    private boolean canGoBack;
    private boolean canGoForward;
    private Hashtable<String, String> mCustomHeaders;
    private String gameObject;
    private String userAgent;
    private boolean transparent;

    public static void initialize(int posX, int posY, int width, int height, int texWidth, int texHeight) {
        if (m_Instance == null) {
            m_Instance = new UnityConnect();
            m_Instance.initWebView(posX, posY, width, height, texWidth, texHeight);
        }
    }

    public static int getTetxure() {
        return (m_Instance != null) ? m_Instance.mViewToGlRenderer.getGLSurfaceTexture() : 0;
    }

    public static byte[] getPixel() {
        byte[] data;
        if (m_Instance != null) {
            data = m_Instance.mViewToGlRenderer.getTexturePixels();
            m_Instance.mGlLayout.postInvalidate();
        } else {
            data = null;
        }
        return data;
    }

    public static void setUserAgent(String ua) {
        if (m_Instance == null) {
            return;
        }

        m_Instance.userAgent = ua;
    }

    public static void setGameObjectString(String gos) {
        if (m_Instance == null) {
            return;
        }

        m_Instance.gameObject = gos;
    }

    public static void loadUrl(String url) {
        if (m_Instance == null) {
            return;
        }

        m_Instance.LoadURL(url);
    }

    public static void setPosition(int posX, int posY) {
        if (m_Instance == null) {
            return;
        }

        m_Instance.mPosX = posX;
        m_Instance.mPosY = posY;
        if (m_Instance.mLayout != null) {
            m_Instance.mLayout.setX(posX);
            m_Instance.mLayout.setY(posY);
        }
    }

    public static void goBack() {
        if (m_Instance == null) {
            return;
        }

        m_Instance.GoBack();
    }

    public static void goForward() {
        if (m_Instance == null) {
            return;
        }

        m_Instance.GoForward();
    }

    public static void setVisible(boolean visible) {
        if (m_Instance == null) {
            return;
        }

        m_Instance.SetVisibility(visible);
    }

    public boolean IsInitialized() {
        return mWebView != null;
    }

    private void initWebView(int posX, int posY, final int width, final int height, final int texWidth, final int texHeight) {
        final UnityConnect self = this;

        mPosX = posX;
        mPosY = posY;
        mViewToGlRenderer = new ViewToGLRenderer();
        mViewToGlRenderer.setTextureWidth(width);
        mViewToGlRenderer.setTextureHeight(height);
        mViewToGlRenderer.createTexture();
        if (texWidth != 0 || texHeight != 0) {
            mViewToGlRenderer.createTextureCapture(UnityPlayer.currentActivity, jp.ne.pickle.libpixelreads.R.raw.vertex, jp.ne.pickle.libpixelreads.R.raw.fragment_oes, texWidth, texHeight);
        }

        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mLayout = new RelativeLayout(UnityPlayer.currentActivity);
                mLayout.setGravity(Gravity.TOP);
                mLayout.setX(mPosX);
                mLayout.setY(mPosY);

                mGLSurfaceView = new GLSurfaceView(UnityPlayer.currentActivity);
                mGLSurfaceView.setEGLContextClientVersion(2);
                mGLSurfaceView.setEGLConfigChooser(8, 8, 8, 8, 0, 0);
                mGLSurfaceView.setPreserveEGLContextOnPause(true);
                mGLSurfaceView.setRenderer(mViewToGlRenderer);

                mGlLayout = new GLLinearLayout(UnityPlayer.currentActivity);
                mGlLayout.setBackgroundColor(Color.WHITE);
                mGlLayout.setOrientation(GLLinearLayout.VERTICAL);
                mGlLayout.setGravity(Gravity.START);

                if (texWidth != 0 || texHeight != 0) {
                    mGlLayout.setViewToGLRenderer(mViewToGlRenderer);
                }

                mWebViewPlugin = new CWebViewPluginInterface(self, gameObject);
                final WebView webView = new WebView(UnityPlayer.currentActivity);
                webView.setWebViewClient(new WebViewClient() {
                    @Override
                    public void onReceivedError(WebView view, int errorCode, String description, String failingUrl) {
                        webView.loadUrl("about:blank");
                        canGoBack = webView.canGoBack();
                        canGoForward = webView.canGoForward();
                        mWebViewPlugin.call("CallOnError", errorCode + "\t" + description + "\t" + failingUrl);
                    }

                    @Override
                    public void onReceivedHttpAuthRequest(WebView view, final HttpAuthHandler handler, final String host, final String realm) {

                        mWebViewPlugin.call("CallOnHttpAuth", host + "," + realm);

                        String userName = null;
                        String userPass = null;

                        if (handler.useHttpAuthUsernamePassword() && view != null) {
                            String[] haup = view.getHttpAuthUsernamePassword(host, realm);
                            if (haup != null && haup.length == 2) {
                                userName = haup[0];
                                userPass = haup[1];
                            }
                        }

                        if (userName != null && userPass != null) {
                            handler.proceed(userName, userPass);
                        } else {
                            showHttpAuthDialog(handler, host, realm, null, null, null);
                        }
                    }

                    @Override
                    public void onPageStarted(WebView view, String url, Bitmap favicon) {
                        canGoBack = webView.canGoBack();
                        canGoForward = webView.canGoForward();
                        mWebViewPlugin.call("CallOnStarted", url);
                    }

                    @Override
                    public void onPageFinished(WebView view, String url) {
                        canGoBack = webView.canGoBack();
                        canGoForward = webView.canGoForward();
                        mWebViewPlugin.call("CallOnLoaded", url);
                    }

                    @Override
                    public void onLoadResource(WebView view, String url) {
                        canGoBack = webView.canGoBack();
                        canGoForward = webView.canGoForward();
                    }

                    @Override
                    public WebResourceResponse shouldInterceptRequest(WebView view, String url) {
                        if (mCustomHeaders == null || mCustomHeaders.isEmpty()) {
                            return super.shouldInterceptRequest(view, url);
                        }

                        try {
                            HttpURLConnection urlCon = (HttpURLConnection) (new URL(url)).openConnection();
                            // The following should make HttpURLConnection have a same user-agent of webView)
                            // cf. http://d.hatena.ne.jp/faw/20070903/1188796959 (in Japanese)
                            urlCon.setRequestProperty("User-Agent", userAgent);

                            for (HashMap.Entry<String, String> entry : mCustomHeaders.entrySet()) {
                                urlCon.setRequestProperty(entry.getKey(), entry.getValue());
                            }

                            urlCon.connect();

                            return new WebResourceResponse(
                                    urlCon.getContentType().split(";", 2)[0],
                                    urlCon.getContentEncoding(),
                                    urlCon.getInputStream()
                            );

                        } catch (Exception e) {
                            return super.shouldInterceptRequest(view, url);
                        }
                    }

                    // falseで通常処理trueで中止(多分)
                    @Override
                    public boolean shouldOverrideUrlLoading(WebView view, String url) {
                        canGoBack = webView.canGoBack();
                        canGoForward = webView.canGoForward();
                        if (url.startsWith("http://") || url.startsWith("https://")
                                || url.startsWith("file://") || url.startsWith("javascript:")) {
                            // Let webview handle the URL
                            return false;
                        } else if (url.startsWith("unity:")) {
                            String message = url.substring(6);
                            mWebViewPlugin.call("CallFromJS", message);
                            return true;
                        }
                        Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
                        view.getContext().startActivity(intent);
                        return true;
                    }
                });
                webView.setWebChromeClient(new WebChromeClient());
                webView.setInitialScale(1);
                webView.setScrollBarStyle(View.SCROLLBARS_INSIDE_OVERLAY);
                webView.clearCache(true);
                webView.addJavascriptInterface(mWebViewPlugin, "Unity");

                WebSettings webSettings = webView.getSettings();
                if (userAgent != null && userAgent.length() > 0) {
                    webSettings.setUserAgentString(userAgent);
                }
                webSettings.setLoadWithOverviewMode(true);
                webSettings.setUseWideViewPort(true);
                webSettings.setSupportZoom(true);
                webSettings.setBuiltInZoomControls(true);
                webSettings.setDisplayZoomControls(false);
                webSettings.setJavaScriptEnabled(true);
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN) {
                    // Log.i("CWebViewPlugin", "Build.VERSION.SDK_INT = " + Build.VERSION.SDK_INT);
                    webSettings.setAllowUniversalAccessFromFileURLs(true);
                }
                webSettings.setDatabaseEnabled(true);
                webSettings.setDomStorageEnabled(true);
                String databasePath = webView.getContext().getDir("databases", Context.MODE_PRIVATE).getPath();
                webSettings.setDatabasePath(databasePath);

                if (transparent) {
                    webView.setBackgroundColor(0x00000000);
                }

                UnityPlayer.currentActivity.addContentView(mLayout, new RelativeLayout.LayoutParams(width, height));

                mGlLayout.addView(webView, new GLLinearLayout.LayoutParams(GLLinearLayout.LayoutParams.MATCH_PARENT, GLLinearLayout.LayoutParams.MATCH_PARENT));
                mLayout.addView(mGLSurfaceView, new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MATCH_PARENT, RelativeLayout.LayoutParams.MATCH_PARENT));
                mLayout.addView(mGlLayout, new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MATCH_PARENT, RelativeLayout.LayoutParams.MATCH_PARENT));

                if (mLoadUrl != null) {
                    LoadURL(mLoadUrl);
                }
                mWebView = webView;
            }
        });
        //Log.i("", "initWebView");
    }

    public void Destroy() {
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {
            public void run() {
                if (mWebView == null) {
                    return;
                }
                mWebView.stopLoading();
                mGlLayout.removeView(mWebView);
                mWebView.destroy();
                mWebView = null;
            }
        });
    }

    public void LoadURL(String url) {
        mLoadUrl = url;
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mWebView == null) {
                return;
            }
            if (mCustomHeaders != null &&
                    !mCustomHeaders.isEmpty()) {
                mWebView.loadUrl(mLoadUrl, mCustomHeaders);
            } else {
                mWebView.loadUrl(mLoadUrl);
            }
        }});
    }

    public void LoadHTML(final String html, final String baseURL)
    {
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mWebView == null) {
                return;
            }
            mWebView.loadDataWithBaseURL(baseURL, html, "text/html", "UTF8", null);
        }});
    }

    public void EvaluateJS(final String js) {
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mWebView == null) {
                return;
            }
            mWebView.loadUrl("javascript:" + js);
        }});
    }

    public void GoBack() {
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mWebView == null) {
                return;
            }
            mWebView.goBack();
        }});
    }

    public void GoForward() {
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mWebView == null) {
                return;
            }
            mWebView.goForward();
        }});
    }

    public void SetMargins(int left, int top, int right, int bottom) {
        final FrameLayout.LayoutParams params
                = new FrameLayout.LayoutParams(
                FrameLayout.LayoutParams.MATCH_PARENT,
                FrameLayout.LayoutParams.MATCH_PARENT,
                Gravity.NO_GRAVITY);
        params.setMargins(left, top, right, bottom);
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mWebView == null) {
                return;
            }
            mWebView.setLayoutParams(params);
        }});
    }

    public void SetVisibility(final boolean visibility) {
        final Activity a = UnityPlayer.currentActivity;
        a.runOnUiThread(new Runnable() {public void run() {
            if (mWebView == null) {
                return;
            }
            if (visibility) {
                mWebView.setVisibility(View.VISIBLE);
                mGlLayout.requestFocus();
                mWebView.requestFocus();
            } else {
                mWebView.setVisibility(View.INVISIBLE);
            }
        }});
    }

    public void AddCustomHeader(final String headerKey, final String headerValue)
    {
        if (mCustomHeaders == null) {
            return;
        }
        mCustomHeaders.put(headerKey, headerValue);
    }

    public String GetCustomHeaderValue(final String headerKey)
    {
        if (mCustomHeaders == null) {
            return null;
        }

        if (!mCustomHeaders.containsKey(headerKey)) {
            return null;
        }
        return this.mCustomHeaders.get(headerKey);
    }

    public void RemoveCustomHeader(final String headerKey)
    {
        if (mCustomHeaders == null) {
            return;
        }

        if (this.mCustomHeaders.containsKey(headerKey)) {
            this.mCustomHeaders.remove(headerKey);
        }
    }

    public void ClearCustomHeader()
    {
        if (mCustomHeaders == null) {
            return;
        }

        this.mCustomHeaders.clear();
    }

    public void ClearCookies()
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
        {
            CookieManager.getInstance().removeAllCookies(null);
            CookieManager.getInstance().flush();
        } else {
            final Activity a = UnityPlayer.currentActivity;
            CookieSyncManager cookieSyncManager = CookieSyncManager.createInstance(a);
            cookieSyncManager.startSync();
            CookieManager cookieManager = CookieManager.getInstance();
            cookieManager.removeAllCookie();
            cookieManager.removeSessionCookie();
            cookieSyncManager.stopSync();
            cookieSyncManager.sync();
        }
    }

    private void showHttpAuthDialog(final HttpAuthHandler handler, final String host,
                                    final String realm, final String title,
                                    final String name, final String password) {

        final Activity activity = UnityPlayer.currentActivity;
        final AlertDialog.Builder mHttpAuthDialog = new AlertDialog.Builder(activity);
        LinearLayout layout = new LinearLayout(activity);

        mHttpAuthDialog.setTitle("Enter the password").setCancelable(false);
        final EditText etUserName = new EditText(activity);
        etUserName.setWidth(100);
        layout.addView(etUserName);
        final EditText etUserPass = new EditText(activity);
        etUserPass.setWidth(100);
        layout.addView(etUserPass);
        mHttpAuthDialog.setView(layout);

        mHttpAuthDialog.setPositiveButton("OK", new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int whichButton) {
                String userName = etUserName.getText().toString();
                String userPass = etUserPass.getText().toString();
                mWebView.setHttpAuthUsernamePassword(host, realm, userName, userPass);
                handler.proceed(userName, userPass);
                //mHttpAuthDialog = null;
            }
        });
        mHttpAuthDialog.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int whichButton) {
                handler.cancel();
                //mHttpAuthDialog = null;
            }
        });
        mHttpAuthDialog.create().show();
    }
}
