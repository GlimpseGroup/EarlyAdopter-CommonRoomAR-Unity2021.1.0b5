using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateObjectController : MonoBehaviour
{
	//public Transform rotatingObjectParent;
	public Transform rotatingObject;
	
	private Camera camera;

	private void Start()
	{
		camera = Camera.main;
	}

	private void Update()
	{
		float angle = camera.transform.eulerAngles.y;
		Vector3 cameraPos = CameraPos(angle);
		Vector3 dir = rotatingObject.position - cameraPos;
		Vector3 rotatingAxis = new Vector3(-dir.z,0,dir.x);
		Debug.Log(rotatingAxis);
		rotatingObject.rotation = Quaternion.AngleAxis(30, rotatingAxis);//Quaternion.Lerp(rotatingObject.rotation,Quaternion.AngleAxis(30, rotatingAxis),Time.deltaTime * 10);
		
	}

	private Vector3 CameraPos(float angle)
	{
		Vector3 cameraPos = rotatingObject.transform.position;
		cameraPos += new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),0,Mathf.Cos(angle * Mathf.Deg2Rad));
		//Debug.Log(cameraPos);
		return cameraPos;
	}
}
