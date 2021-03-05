using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{

    public delegate void LookToCamera(GameObject obj);
    public  static event  LookToCamera FolowRotate;

    
    // Update is called once per frame
    void Update ()
    {
        if (FolowRotate != null)
            FolowRotate(gameObject);
    }
    

}