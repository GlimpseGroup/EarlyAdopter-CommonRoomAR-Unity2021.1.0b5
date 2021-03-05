using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookOnCameraNonDelegate : MonoBehaviour
{
	[SerializeField]
    private Vector3 deltaAngle = Vector3.zero;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void FixedUpdate () {
        
        transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);
        Vector3 rot = transform.eulerAngles;
        
        transform.rotation = Quaternion.AngleAxis(deltaAngle.x, transform.right) * transform.rotation;
        transform.rotation = Quaternion.AngleAxis(deltaAngle.y, transform.up) * transform.rotation;
    }
}
