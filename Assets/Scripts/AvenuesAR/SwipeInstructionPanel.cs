using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInstructionPanel : MonoBehaviour {

    public CanvasGroup FirstSwipe;
    public CanvasGroup Swipe;

    private bool firstSwipeShown = false;
    [HideInInspector]
    public bool showSwipe = false;

    private bool hiding = false;
    private float hideTime = 2f;
    private bool hidden = false;

    public char[] swipeTargets;

	// Use this for initialization
	void Start () {
        Array.Sort(swipeTargets);
        FirstSwipe.gameObject.SetActive(false);
    }

    public void ShowSwipe(char c)
    {
        if (c == '0')
        {
            showSwipe = false;
            hiding = false;
            hidden = false;
            return;
        }
        showSwipe = false;
        if (Array.BinarySearch(swipeTargets, c) >= 0)
        {
            showSwipe = true;
            if (hiding == false)
            {
                hiding = true;
                StartCoroutine(hideOverTime());
            }
        }
    }

    private IEnumerator hideOverTime()
    {
        yield return new WaitForSeconds(hideTime);
        hidden = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (showSwipe && !hidden) {
            //if (Input.touchCount > 0 && FirstSwipe.alpha >= 1)
                //FirstSwipe.gameObject.SetActive(false);
            //FirstSwipe.alpha += Time.deltaTime;
            Swipe.alpha += Time.deltaTime;
        } else
        {
            Swipe.alpha -= Time.deltaTime;
        }
    }
}
