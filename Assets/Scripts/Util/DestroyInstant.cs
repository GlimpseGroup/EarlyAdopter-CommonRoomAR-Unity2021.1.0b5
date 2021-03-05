using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInstant : MonoBehaviour
{
    [SerializeField]
    float Time = 0;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, Time);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
