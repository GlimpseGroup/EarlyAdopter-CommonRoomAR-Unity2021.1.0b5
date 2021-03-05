using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Observer;

[DisallowMultipleComponent]
public class DioramaAnalytics : MonoBehaviour
{
    public string ProjectName;

    //private static Observer_ProjectSettings settings;
    private string StartTime;

    // Start is called before the first frame update
    void Start()
    {
        //if (Observer_Analytics.Instance == null || !Observer_Analytics.AnalyticsEnabled)
        //{
        //    Debug.LogError("Could not find Observer_Analytics prefab. Make sure it is present and enabled in the scene");
        //    enabled = false;
        //}

        //settings = Resources.Load<Observer_ProjectSettings>("Observer_ProjectSettings");
        //if (settings == null || string.IsNullOrEmpty(settings.applicationID))
        //{
        //    enabled = false;
        //}

        //StartTime = Observer_Functions.GetCurrentTimeString();
    }

    // Update is called once per frame
    void Update()
    {
        //if (string.IsNullOrEmpty(StartTime))
        //    StartTime = Observer_Functions.GetCurrentTimeString();
    }

    private void OnDestroy()
    {
        if (string.IsNullOrEmpty(StartTime))
            return;
        if (string.IsNullOrEmpty(ProjectName))
            return;
        //Observer_Events.CustomEvent("OBJECT_FOV").AddParameter("assetName", "Project-View-" + ProjectName).AddParameter("startTime", StartTime).EndParameters();
    }
    private void OnDisable()
    {
        if (string.IsNullOrEmpty(StartTime))
            return;
        if (string.IsNullOrEmpty(ProjectName))
            return;
        //Observer_Events.CustomEvent("OBJECT_FOV").AddParameter("assetName", "Project-View-" + ProjectName).AddParameter("startTime", StartTime).EndParameters();
    }
}
