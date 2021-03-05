using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    private enum RotateTransformType
    {
        x = 0,
        y = 1,
        z = 2
    }
    [SerializeField]
    RotateTransformType type;
    public float multiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (type == RotateTransformType.x)
            transform.Rotate(new Vector3(multiplier * Time.deltaTime, 0, 0), Space.Self);
        if (type == RotateTransformType.y)
            transform.Rotate(new Vector3(0, multiplier * Time.deltaTime, 0), Space.Self);
        if (type == RotateTransformType.z)
            transform.Rotate(new Vector3(0, 0, multiplier * Time.deltaTime), Space.Self);
    }
}
