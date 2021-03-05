using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabGarbageCollector : MonoBehaviour
{
    private static PrefabGarbageCollector instance = null;

    public void UpdatePrefabGameObject(UniqueImageTargetID target)
    {
        Debug.Log("Unloading Assets for " + target.name);
        foreach (Object o in target.unload)
        {
            Resources.UnloadAsset(o);
        }
    }
    
    public static PrefabGarbageCollector Instance
    {
        get
        {
            return instance; 
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
        }

        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start ()
    {
        // StartCoroutine(trackAnchorPrefabs());
	}
	
    IEnumerator trackAnchorPrefabs()
    {
        while(Application.isPlaying)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("Anchor Prefabs: " + GameObject.FindGameObjectsWithTag("AnchorPrefab").Length);
        }
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
