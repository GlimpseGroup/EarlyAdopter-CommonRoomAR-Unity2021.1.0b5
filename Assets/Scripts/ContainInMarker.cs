using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainInMarker : MonoBehaviour
{

	private TrackableEventHandler imageTarget;
	[SerializeField]
	private GameObject markersContainPrefab;

	private GameObject contentInScene;
	

	private void Awake()
	{
		imageTarget = gameObject.GetComponent<TrackableEventHandler>();
	}

	private void OnEnable()
	{
		if (imageTarget != null)
		{
			imageTarget.OnFoundLocal += ImageFound;
			imageTarget.OnLostLocal += ImageLost;
		}
	}

	private void ImageFound()
	{
		if (contentInScene == null)
		{		
			contentInScene = Instantiate(markersContainPrefab, imageTarget.gameObject.transform);
			CleanVideoFromRam.Instance.AddVideoToQueue(contentInScene);
		}

	}

	private void ImageLost()
	{
		
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		//true - app on pause
		//false app is comeback
		
	}
}
