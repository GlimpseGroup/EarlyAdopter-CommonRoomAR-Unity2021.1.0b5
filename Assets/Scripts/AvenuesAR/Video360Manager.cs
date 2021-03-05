using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;

public class Video360Manager : MonoBehaviour
{
    public static Video360Manager instance = null;

    public Camera mainCamera;

    [SerializeField]
    Transform VideoSphere;
    Transform current;

    [SerializeField]
    GameObject OutVideoButton;

    [SerializeField]
    Button TestButton;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    public void PlayVideo(string s)
    {
        if (current)
            return;
        OutVideoButton.SetActive(true);
        Transform t = Instantiate(VideoSphere);
        t.gameObject.SetActive(true);
        current = t;
        t.position = mainCamera.transform.GetChild(0).position;
        t.rotation = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y + 90, 0);
        t.localScale = Vector3.one;

        MediaPlayer VideoPlayer = t.GetComponentInChildren<MediaPlayer>();

        VideoPlayer.m_VideoPath = s;
        VideoPlayer.Play();
        WebViewManager.webViewEnabled = true;

        //mainCamera.fieldOfView = 120;
    }

    public void StopVideo()
    {
        current.GetComponentInChildren<MediaPlayer>().Stop();
        Destroy(current.gameObject);
        current = null;
        OutVideoButton.SetActive(false);
        WebViewManager.webViewEnabled = false;

        //mainCamera.fieldOfView = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TestButton.onClick.Invoke();
        }
    }
}
