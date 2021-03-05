
using UnityEngine;
using UnityEngine.Video;
//using Vuforia;
using System;
using System.Collections;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class TrackableEventHandler : MonoBehaviour //, ITrackableEventHandler
{

    public delegate void ShowUIelements();
    public static event ShowUIelements ShowUI;
    public static event ShowUIelements HideUI;
    public static Action OnFound;
    public static Action OnLost;
    
    public Action OnFoundLocal;
    public Action OnLostLocal;
    
    private GestureMove gestureMove;
    private string videoObjToScip;
    [HideInInspector]
    public bool isShowedInfo;
    
    //public bool isShowMenu;
    
    [HideInInspector]
    public bool isActiveTarget; //for checking in expG

    #region PRIVATE_MEMBER_VARIABLES

    //protected TrackableBehaviour mTrackableBehaviour;

    #endregion // PRIVATE_MEMBER_VARIABLES


    public string VideoToScip
    {
        get { return videoObjToScip; }
        set { videoObjToScip = value; }
    }


    private void OnEnable()
    {
        
    }

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        //mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        //if (mTrackableBehaviour)
        //    mTrackableBehaviour.RegisterTrackableEventHandler(this);

        gestureMove = gameObject.GetComponentInChildren<GestureMove>();

    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged( )
    { 
    //    TrackableBehaviour.Status previousStatus,
    //    TrackableBehaviour.Status newStatus)
    //{
    //    if (newStatus == TrackableBehaviour.Status.DETECTED ||
    //        newStatus == TrackableBehaviour.Status.TRACKED ||
    //        newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
    //    {
    //        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
    //        OnTrackingFound();
    //    }
    //    else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
    //             newStatus == TrackableBehaviour.Status.NOT_FOUND)
    //    {
    //        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
    //        OnTrackingLost();
    //    }
    //    else
    //    {
    //        // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
    //        // Vuforia is starting, but tracking has not been lost or found yet
    //        // Call OnTrackingLost() to hide the augmentations
    //        OnTrackingLost();
    //    }
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
        
        isActiveTarget = true;
        
        //if (ShowUI != null)
        //    ShowUI();

        if (OnFound != null)
            OnFound();

        if (OnFoundLocal != null)
            OnFoundLocal();

    }

    
    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;

        isActiveTarget = false;
        
        if (gestureMove != null)
            gestureMove.enabled = false;

        if (OnLost != null)
            OnLost();
        
        if (OnLostLocal != null)
            OnLostLocal();
        
        //if (HideUI != null)
        //    HideUI();

    }

    #endregion // PRIVATE_METHODS
}
