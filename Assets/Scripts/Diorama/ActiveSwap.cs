using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSwap : MonoBehaviour
{
    [SerializeField]
    float cooldown = 1f;
    float _cooldown;

    public GameObject[] objects;
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        _cooldown = cooldown;
        foreach (GameObject g in objects)
            g.SetActive(false);
        objects[index].SetActive(true);
    }

    public void Select(int num)
    {
        if (cooldown > 0)
            return;
        cooldown = _cooldown;

        index = 0;
        foreach (GameObject g in objects)
            g.SetActive(false);
        objects[index].SetActive(true);
    }

    public void Next()
    {
        if (cooldown > 0)
            return;
        cooldown = _cooldown;

        index = (index + 1) % objects.Length;

        foreach (GameObject g in objects)
            g.SetActive(false);
        objects[index].SetActive(true);
    }
    public void Prev()
    {
        if (cooldown > 0)
            return;
        cooldown = _cooldown;

        index = (index - 1) % objects.Length;

        foreach (GameObject g in objects)
            g.SetActive(false);
        objects[index].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
    }
}
