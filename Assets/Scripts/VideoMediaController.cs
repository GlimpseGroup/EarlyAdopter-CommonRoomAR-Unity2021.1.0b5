using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class VideoMediaController : MonoBehaviour
{
	[SerializeField]
	private List<MediaPlayer> mediaPlayers;
	//[SerializeField]
	private TrackableEventHandler trackableMarkes;

	private void Awake()
	{
		trackableMarkes = gameObject.GetComponentInParent<TrackableEventHandler>();
	}

	private void Start()
	{
        Debug.Log("Starting play");
		ContinuePlay();
	}

	private void OnEnable()
	{
		if (trackableMarkes != null)
		{
			trackableMarkes.OnFoundLocal += ContinuePlay;
			trackableMarkes.OnLostLocal += StopPlay;
		}
	}

	private void OnDisable()
	{
		if (trackableMarkes != null)
		{
			trackableMarkes.OnFoundLocal -= ContinuePlay;
			trackableMarkes.OnLostLocal -= StopPlay;
		}
	}

	private void StopPlay()
	{
		if (mediaPlayers != null)
		{
			foreach (var player in mediaPlayers)
			{
				player.Pause();
			}
		}
	}

	private void ContinuePlay()
	{
		if (mediaPlayers != null)
		{
			foreach (var player in mediaPlayers)
			{
				player.Play();
			}
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			StopPlay();
			//Debug.Log("pause true  ........  ");
		}
		else
		{
			//ContinuePlay();
		}
	}
}
