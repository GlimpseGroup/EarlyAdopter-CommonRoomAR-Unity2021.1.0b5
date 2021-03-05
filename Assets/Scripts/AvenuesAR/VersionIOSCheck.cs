using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
public class VersionIOSCheck : MonoBehaviour {
    
	// Use this for initialization
	void Awake ()
    {
#if UNITY_IOS
        string systemVersion = Device.systemVersion;
        if (systemVersion.Contains("12"))
            return;
        if (systemVersion.Contains("13"))
            return;
        if (systemVersion.Contains("14"))
            return;
        SceneManager.LoadScene("BadVersion");
#endif
    }

}
