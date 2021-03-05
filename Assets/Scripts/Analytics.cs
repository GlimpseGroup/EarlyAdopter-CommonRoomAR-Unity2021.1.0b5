using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Observer;

public class Analytics : MonoBehaviour
{
    #region Observer custom events
    //user view exp
 //   private Observer_CustomEvent timeBackgrounding = new Observer_CustomEvent("Test_custom_events");
	//private Observer_CustomEvent usersTimeStamp = new Observer_CustomEvent("Open and closing app");

	#endregion
	
	#region userActions variable
	
	private bool isPause;
	private string timeOpeningApp;
	private string timeClosimgApp;
	
	private string timeHideToBackround;
	private string timeOpenFromBackround;
	
	#endregion
	
	private void Awake()
	{
		timeOpeningApp = DateTime.Now.ToString();
	}


	private void SendOnOffApp()
	{
		//usersTimeStamp.AddParameter("Time opnening app " ,timeOpeningApp)
		//			  .AddParameter("Time closing app ",timeClosimgApp)
		//			  .EndParameters();
		
		Debug.Log("time start app - " + timeOpeningApp);
		Debug.Log("time close app - " + timeClosimgApp);
	}

	private void SendUserTimeStamp()
	{
		//usersTimestamp.AddParameter("time starting application ",)
	}

	#region  Unity runtime events

	private void OnApplicationQuit()
	{
		timeClosimgApp = DateTime.Now.ToString();
		SendOnOffApp();
	}

	private void OnApplicationFocus(bool hasFocus)
	{	
		isPause = !hasFocus;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		isPause = pauseStatus;
		timeHideToBackround = DateTime.Now.ToString();
		Debug.Log("on pause - " + pauseStatus);
	}
	
	#endregion
}
