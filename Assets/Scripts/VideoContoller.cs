using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoContoller : MonoBehaviour {
    public MediaPlayer mediaPlayer;
    public Button playButton;

    public Sprite PlayImage;
    public Sprite PauseImage;

    public float playPauseDisplayDuration = 3;

    [Header("NEW")]
    public GameObject VideoSlider;
    public Button FullScreenButton;
    public GameObject FullScreenBG;
    public RectTransform currentRect;
    public RectTransform defaultRect;
    public RectTransform fullscreenRect;
    public Sprite spriteFullScreen;
    public Sprite spriteSmallScreen;

    bool isPaused;
    float fadeTimer;
	// Use this for initialization
	void Start () {
        isPaused = false;
        playButton.gameObject.SetActive(false);
        if (FullScreenButton)
            FullScreenButton.gameObject.SetActive(false);
        fadeTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        FullScreenBG.SetActive(FullScreen);
		if(playButton.gameObject.activeSelf)
        {
            fadeTimer += Time.deltaTime;
        }
        if(fadeTimer >= playPauseDisplayDuration)
        {
            playButton.gameObject.SetActive(false);
            if (FullScreenButton)
                FullScreenButton.gameObject.SetActive(false);
            if (FullScreen)
                VideoSlider.SetActive(false);
            fadeTimer = 0;
        }
        if (!FullScreen)
            VideoSlider.SetActive(true);
	}

    public void showPlay()
    {
        if (FullScreen)
            VideoSlider.SetActive(!VideoSlider.activeSelf);
        playButton.gameObject.SetActive(!playButton.gameObject.activeSelf);
        if (FullScreenButton)
            FullScreenButton.gameObject.SetActive(!FullScreenButton.gameObject.activeSelf);
        fadeTimer = 0;
       
    }
    public void FullScreenSliderRefreshFade()
    {
        if (FullScreen)
            fadeTimer = 0;
    }

    bool FullScreen = false;
    public void GoFullScreen()
    {
        if (!FullScreen)
        {
            FullScreen = true;
            currentRect.anchorMin = fullscreenRect.anchorMin;
            currentRect.anchorMax = fullscreenRect.anchorMax;
            currentRect.anchoredPosition = fullscreenRect.anchoredPosition;
            currentRect.sizeDelta = fullscreenRect.sizeDelta;
            FullScreenButton.GetComponent<Image>().sprite = spriteSmallScreen;
            VideoSlider.SetActive(playButton.gameObject.activeSelf);
        } else
        {
            FullScreen = false;
            currentRect.anchorMin = defaultRect.anchorMin;
            currentRect.anchorMax = defaultRect.anchorMax;
            currentRect.anchoredPosition = defaultRect.anchoredPosition;
            currentRect.sizeDelta = defaultRect.sizeDelta;
            FullScreenButton.GetComponent<Image>().sprite = spriteFullScreen;
            VideoSlider.SetActive(true);
        }
    }

    public void PausePlayVideo()
    {
        if (playButton.gameObject.activeSelf)
        {
            fadeTimer = 0;
            if (isPaused)
            {
                mediaPlayer.Play();
                isPaused = false;
                playButton.image.sprite = PauseImage;
            }
            else
            {
                isPaused = true;
                mediaPlayer.Pause();
                playButton.image.sprite = PlayImage;
            }
        }
    }


}
