using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkActivation
{
    public string Uri { get; private set; }

    public string RawQueryString { get; private set; }

    public Dictionary<string, string> QueryString { get; private set; }
}

/// <summary>
/// This behavior script manages showing and hiding the web view.
/// </summary>
public class WebViewManager : MonoBehaviour
{
    #region Static Properties

    private static WebViewManager _instance;

    public static WebViewManager Instance
    {
        get { return _instance; }
    }

    #endregion

    public RectTransform webViewPanel;
    public GameObject uniWebViewPrefab;
    public Camera ARCamera;
    public GameObject WifiError;

    [SerializeField]
    private UniWebToolbarAndroid androidToolbar;

    private UniWebView webView;
    public static bool webViewEnabled = false;

    #region Monobehavior Callbacks

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        webView = null;
        ImaginationOverflow.UniversalDeepLinking.DeepLinkManager.Instance.LinkActivated += Instance_LinkActivated;
    }

    private void Instance_LinkActivated(ImaginationOverflow.UniversalDeepLinking.LinkActivation linkActivation)
    {
        Debug.Log("ToString ::: " + linkActivation.ToString());

        //var url = linkActivation.Uri;
        //var querystring = linkActivation.RawQueryString;
        //var qParameter = linkActivation.QueryString["q"];

        Debug.Log("url ::: " + linkActivation.Uri);
        Debug.Log("querystring ::: " + linkActivation.RawQueryString);

        AdminController.Instance.SSOAuthTOKEN = linkActivation.RawQueryString.Substring(linkActivation.RawQueryString.LastIndexOf('=') + 1);
        
        AdminController.Instance.Username = linkActivation.RawQueryString.Substring(linkActivation.RawQueryString.IndexOf('=') + 1, linkActivation.RawQueryString.IndexOf('&') - (linkActivation.RawQueryString.IndexOf('=') + 1));
        for (int i = 0; i < RemoteLoginController.GetInstance().Username.Count; i++)
            RemoteLoginController.GetInstance().Username[i].text = AdminController.Instance.Username;
        RemoteLoginController.GetInstance().SetTokenAndLogoutText(linkActivation.RawQueryString.Substring(linkActivation.RawQueryString.LastIndexOf('=') + 1), "Logout");

        StartCoroutine(ChannelManager.instance.ReloadChannelScreen());
    }

    #endregion

    IEnumerator cr_LoadAndShow(string URL)
    {
        WWW www = new WWW(URL);
        yield return www;
        if (www.error != null)
        {
            WifiError.SetActive(true);
        }
        else
        {
            if (webView == null)
            {
                webView = Instantiate(uniWebViewPrefab).GetComponent<UniWebView>();
                webView.transform.SetAsFirstSibling();
                webView.ReferenceRectTransform = webViewPanel;
                webView.OnShouldClose += WebView_OnShouldClose;
#if UNITY_ANDROID
                androidToolbar.gameObject.SetActive(true);
                androidToolbar.DoneButton.onClick.AddListener(delegate { webView.Hide(); });
                androidToolbar.DoneButton.onClick.AddListener(delegate { androidToolbar.gameObject.SetActive(false); });
                androidToolbar.BackButton.onClick.AddListener(webView.GoBack);
                androidToolbar.ForwardButton.onClick.AddListener(webView.GoForward);
#endif
            }

            webViewEnabled = true;
            webView.Load(URL);
            webView.Show();

#if UNITY_ANDROID
            androidToolbar.gameObject.SetActive(true);
#endif

            webView.OnMessageReceived += (view, message) =>
            {
                Debug.Log(message.RawMessage);
            };
        }
    }

    public void NativeLoadAndShow(string url)
    {
        Application.OpenURL(url);
    }

    public void LoadAndShow(string url)
    {
        if (AdminController.GetInstance().isAdmin && url.IndexOf("?ar=") > 0)
            url = url.Substring(0, url.IndexOf("?ar="));

        Debug.Log("Opening Link: " + url);
        StartCoroutine(cr_LoadAndShow(url));

        //if (webView == null)
        //{
        //    webView = Instantiate(uniWebViewPrefab).GetComponent<UniWebView>();
        //    webView.ReferenceRectTransform = webViewPanel;
        //    webView.OnShouldClose += WebView_OnShouldClose;
        //    //VuforiaBehaviour.Instance.enabled = false;
        //    //ARCamera.enabled = false;
        //    webViewEnabled = true;
        //}

        //webView.Load(url);
        //webView.Show();

        //webView.OnMessageReceived += (view, message) => {
        //    Debug.Log(message.RawMessage);

        //};
    }

    /// <summary>
    /// Used to relinquish reference to webView before UniWebView.OnShouldClose completes execution.
    /// </summary>
    /// <param name="webView"></param>
    /// <returns></returns>
    private bool WebView_OnShouldClose(UniWebView webView)
    {
        webView = null;
        androidToolbar.gameObject.SetActive(false);
        webViewEnabled = false;
        return (webView == null) ? true : false;
    }

    public void Hide()
    {
        if (webView == null)
        {
            androidToolbar.gameObject.SetActive(false);
            webViewEnabled = false;
            return;
        }
        webView.Hide();
        androidToolbar.gameObject.SetActive(false);
        webViewEnabled = false;
    }

    public bool CanGoBack()
    {
        if (webView != null)
            return webView.CanGoBack;
        return false;
    }
    public bool CanGoForward()
    {
        if (webView != null)
            return webView.CanGoForward;
        return false;
    }

    public void Update()
    {
        if (!webViewEnabled)
            androidToolbar.gameObject.SetActive(false);
    }

}