using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ActivateAudio : MonoBehaviour {

	private Transform camera;
	public List<MediaPlayer> Audios;
	
	private void Awake()
	{
		camera = Camera.main.transform;
	}

	private void OnEnable()
	{
		MuteAudio.TargetAudio += SwitchAudio;
    }

	private void OnDisable()
	{
		MuteAudio.TargetAudio -= SwitchAudio;
	}

    MediaPlayer prevMedia = null;
    MediaPlayer closestMedia = null;
    private void SwitchAudio()
    {
        //Transform camera = Camera.main.transform;
        float distance = float.MaxValue;
	    
	    if(Audios != null)
		    foreach (MediaPlayer audio in Audios)
		    {
				audio.Control.MuteAudio(true);
                //audio.transform.parent.GetChild(2).gameObject.SetActive(false);
                float d = Vector3.Distance(camera.position, audio.gameObject.transform.position);

				if (d < distance)
				{
					distance = d;
					closestMedia = audio;
				}
		    }

	    if (closestMedia != null)
	    {
		    closestMedia.Control.MuteAudio(false);
            //closestMedia.transform.parent.GetChild(2).gameObject.SetActive(true);
	    }
        if (prevMedia != closestMedia)
        {
            foreach (MediaPlayer audio in Audios)
            {
                if (audio != closestMedia)
                    audio.Pause();
                else
                    audio.Play();
            }
        }
    }
}
