using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DioramaBeacon : MonoBehaviour
{
    public static DioramaBeacon instance = null;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
