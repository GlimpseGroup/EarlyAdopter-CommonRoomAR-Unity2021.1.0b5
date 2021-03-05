using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebViewButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadAndShowName()
    {
        WebViewManager.Instance.LoadAndShow(transform.name);
    }

    public void LoadAndShowSubText()
    {
        WebViewManager.Instance.LoadAndShow(GetComponentInChildren<Text>().text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
