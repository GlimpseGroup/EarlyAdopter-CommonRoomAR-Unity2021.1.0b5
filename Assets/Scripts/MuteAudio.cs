using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteAudio : MonoBehaviour
{
	public static MuteAudio Instance;
	
	public delegate void MuteAudionInVideo();

	public static MuteAudionInVideo TargetAudio;
	
	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);

	//	DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (TargetAudio != null)
			TargetAudio();
	}
}
