using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureMove : MonoBehaviour
{
	//public Transform rotatingObject;
	
	private Vector2 lastPos;
	private float lastPosition;
	private bool isHidedUi;
    public float m_rotationalSpeed = 360;
    private float m_anchorPosition;
    public float m_maxRotation = 720;
    private float m_startingRotation;

	public TrackableEventHandler imageTarget;

	private void Awake()
	{
        Debug.Log("I'm here before imageTarget instantiate");
		imageTarget = gameObject.GetComponentInParent<TrackableEventHandler>();
        Debug.Log("Awake: imageTarget \"" + (imageTarget == null ? "null" : imageTarget.name) + "\"");
        Debug.Log("I'm here after imageTarget instantiate");
    }

    void ShowInfo()
    {
        //Debug.Log("My Obj \"" + gameObject.name + "\"");
        Transform thisTransform = transform;
        Transform parent = thisTransform.parent;
        while (parent != null)
        {
            //Debug.Log("Parent of " + thisTransform.gameObject.name + " is " + parent.gameObject.name);
            thisTransform = parent;
            parent = thisTransform.parent;
        }
    }

	void Update () 
	{
        ShowInfo();

        //Debug.Log("0: imageTarget \"" + (imageTarget == null ? "null" : imageTarget.name) + "\"");
        if(imageTarget == null)
        {
            
            imageTarget = gameObject.GetComponentInParent<TrackableEventHandler>();
           

        }
        if (imageTarget != null)
        {
            if (Input.touchCount == 1 && imageTarget.isActiveTarget)
            {

                
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    //position x starts at the anchor
                    m_anchorPosition = Input.touches[0].position.x;
                    m_startingRotation = transform.rotation.eulerAngles.y;

                    //lastPosition = Input.touches[0].position.x;
                }
                
                if (Input.touches[0].phase == TouchPhase.Moved)
                {   
                    //the distance is based on how far the we have moved from the anchor
                    float distance = Input.touches[0].position.x - m_anchorPosition;
                    
                    //swiping finger all the way across causes rotation to be 360: (720 / half the screen width)
                    float screen_width = (distance / (float)Screen.width);
                    float angle = screen_width * m_maxRotation;

                    //UpdateObjectRotation((Input.touches[0].position.x - lastPosition) / Screen.dpi);

                    //only rotating about the Y axis
                    //keep the rotation of last position and start from there 
                    //minus sign to have the rotation in the correct direction
                    transform.rotation = Quaternion.Euler(new Vector3(0, m_startingRotation - angle, 0));
                    
                    //lastPosition = Input.touches[0].position.x;

                }
                

            }
            
        }
    }

    void UpdateObjectRotation (float difference)
	{
		transform.rotation *= Quaternion.Euler(new Vector3(0,-(difference * m_rotationalSpeed / 5),0));
		//rotatingObject.Rotate(rotatingObject.up,- difference * 360 / 5);
		if (!isHidedUi)
		{
			if(imageTarget != null)
				imageTarget.isShowedInfo = true;
		}
	}
}
