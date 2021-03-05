using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public Transform follow;

    public float massScale;
    public bool rotation;

    public bool physics = true;
    private new Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (follow == null)
            return;
        if (physics)
            rigidbody.velocity = (follow.position - transform.position) * massScale;
        else
            transform.position = follow.position;
        if (rotation)
            transform.rotation = follow.rotation;
	}
}
