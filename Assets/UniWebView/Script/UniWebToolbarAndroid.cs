using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniWebToolbarAndroid : MonoBehaviour {

    public Button DoneButton;
    public Button BackButton;
    public Button ForwardButton;

    // Use this for initialization
    void Start () {
		
	}

#if UNITY_ANDROID 
    // Update is called once per frame
    private void Update () {
        BackButton.interactable = WebViewManager.Instance.CanGoBack();
        ForwardButton.interactable = WebViewManager.Instance.CanGoForward();
    }
#endif
}
