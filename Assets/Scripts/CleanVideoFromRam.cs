//using System.Collections;

using System;
using System.Collections.Generic;
//using RenderHeads.Media.AVProVideo;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Video;
//using Vuforia;
using UnityEngine.SceneManagement;

public class CleanVideoFromRam : MonoBehaviour {

    public static CleanVideoFromRam Instance;
	private List<GameObject> imageTargetsObj;

	private int maxTargetRecognize = 3;

	private void Awake()
	{
		if(Instance == null)
			Instance = this;
		else 
			Destroy(gameObject);
		
		imageTargetsObj = new List<GameObject>();
		Debug.Log("target count - " + imageTargetsObj.Count);
	}

	/// <summary>
	/// take gameObject which have imageTarget
	/// </summary>
	/// <param name="recognizedImageObj"></param>
	public void AddVideoToQueue(GameObject recognizedImageObj)
	{
		bool isRepeatTarget = false;
		
		if (imageTargetsObj != null)
		{
			//just add imageTarget to list if they less then max count and don't repeate
			if (imageTargetsObj.Count > 0 && imageTargetsObj.Count < maxTargetRecognize) 
			{
				
				foreach (var marker in imageTargetsObj)
				{
					if (marker.name == recognizedImageObj.name)
					{
						isRepeatTarget = true;
						break;
					}

				}
				
				if(!isRepeatTarget)
					imageTargetsObj.Add(recognizedImageObj);

			}
			else if (imageTargetsObj.Count >= maxTargetRecognize)
			{
				//checking for repeating target
				foreach (var marker in imageTargetsObj)
				{
					if (marker.name == recognizedImageObj.name)
						return;

				}

				//remove objetc with video from latest marker		
				Destroy(imageTargetsObj[0]);
				GC.Collect();
				
				for (int i = 0; i < imageTargetsObj.Count; i++)
				{
					if(i < maxTargetRecognize - 1)
						imageTargetsObj[i] = imageTargetsObj[i + 1];
					else				
						imageTargetsObj[i] = recognizedImageObj;
				}

			}
			else // if list is empty
			{
				imageTargetsObj.Add(recognizedImageObj);
			}

		}
		else
		{
			imageTargetsObj = new List<GameObject>();
			imageTargetsObj.Add(recognizedImageObj);
		}


	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (maxTargetRecognize != null)
		{
			/*
			if (imageTargetsObj.Count >= maxTargetRecognize)
			{
				
				for (int i = 0; i < imageTargetsObj.Count; i++)
				{
					if (i == 0)
						continue;
					else
					{
						Destroy(imageTargetsObj[i]);
						//imageTargetsObj.RemoveAt(1);
					}
				}
				
				//imageTargetsObj.RemoveAt(1);
			}
			else
			{
				//imageTargetsObj.RemoveAt(0);
				imageTargetsObj.Clear();
			}
			*/
			foreach (var playerPrefab in imageTargetsObj)
			{
				Destroy(playerPrefab);
			}
			imageTargetsObj.Clear();
			GC.Collect();
		}
	}
		
	

}
