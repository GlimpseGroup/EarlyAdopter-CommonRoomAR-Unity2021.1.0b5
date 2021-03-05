using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;

public class LookOnCamera : MonoBehaviour
{
    [SerializeField]
    private Vector3 deltaAngle = Vector3.zero;

    private void OnEnable()
    {
        MainCam.FolowRotate += FollowCameraRotate;
    }

    private void OnDisable()
    {
        MainCam.FolowRotate -= FollowCameraRotate;
    }


    private void FollowCameraRotate(GameObject camTransform)
    {
        gameObject.transform.LookAt(camTransform.transform);
        //Quaternion rot = gameObject.transform.rotation;
        Vector3 rot = gameObject.transform.eulerAngles;
        gameObject.transform.rotation = Quaternion.Euler(rot.x + deltaAngle.x,gameObject.transform.eulerAngles.y + deltaAngle.y,rot.z);
        
    }
}