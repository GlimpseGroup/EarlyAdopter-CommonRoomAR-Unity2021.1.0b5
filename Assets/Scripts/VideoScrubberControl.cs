using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;

public class VideoScrubberControl : MonoBehaviour {

    public MediaPlayer AVMediaPlayer;
    public Slider videoScrubber;
    private float currentVideoScrubberVal;
    private bool wasSeekingVideo;
    private bool startVideoUpdates;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (startVideoUpdates)
        {
            videoScrubber.maxValue = AVMediaPlayer.Info.GetDurationMs();
            currentVideoScrubberVal = AVMediaPlayer.Control.GetCurrentTimeMs();
            videoScrubber.value = AVMediaPlayer.Control.GetCurrentTimeMs();
        }
	}


    public IEnumerator StartVideoScrubber()
    {
        videoScrubber.minValue = 0;
        yield return null;
        videoScrubber.maxValue = AVMediaPlayer.Info.GetDurationMs();
        startVideoUpdates = true;
        yield return null;
    }


    public IEnumerator ResetVideoScrubber()
    {
        videoScrubber.minValue = 0;
        yield return null;
        videoScrubber.maxValue = 0;
        startVideoUpdates = false;
        yield return null;
    }


    public void OnVideoSeekSlider()
    {

        if (AVMediaPlayer && videoScrubber && videoScrubber.value != currentVideoScrubberVal && wasSeekingVideo)
        {

            AVMediaPlayer.Control.Seek(videoScrubber.value);
            videoScrubber.value = AVMediaPlayer.Control.GetCurrentTimeMs();
            currentVideoScrubberVal = AVMediaPlayer.Control.GetCurrentTimeMs();
        }
    }

    public void OnVideoSliderDown()
    {
        if (AVMediaPlayer)
        {
            wasSeekingVideo = AVMediaPlayer.Control.IsPlaying();
            if (wasSeekingVideo)
            {
                AVMediaPlayer.Control.Pause();
            }
            OnVideoSeekSlider();
        }
    }
    public void OnVideoSliderUp()
    {
        if (AVMediaPlayer && wasSeekingVideo)
        {
            AVMediaPlayer.Control.Play();
            wasSeekingVideo = false;
        }
    }


}
