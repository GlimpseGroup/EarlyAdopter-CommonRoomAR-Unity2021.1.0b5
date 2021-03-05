using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DioramaMultiImages : MonoBehaviour
{
    MeshRenderer[] imageMRDeck;
    int counter = 1;

    public Canvas canvas;
    public RectTransform Backward;
    public RectTransform Forward;

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextImage();
        }
    }

    public void NextImage()
    {
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        for (int i = 1; i < MRs.Length; i++)
        {
            MRs[i].enabled = false;
        }


        if (counter < MRs.Length - 1)
        {
            counter = counter + 1;
        }
        else
        {
            counter = 1;
        }
        MRs[counter].enabled = true;
        canvas.transform.localScale = MRs[counter].transform.localScale * new Vector2(0.002f, 0.002f);
        Backward.sizeDelta = new Vector3(100 * 0.002f / canvas.transform.localScale.x / DioramaManager.DIORAMASCALE, 100 * 0.002f / canvas.transform.localScale.y / DioramaManager.DIORAMASCALE, 1);
        Forward.sizeDelta = new Vector3(100 * 0.002f / canvas.transform.localScale.x / DioramaManager.DIORAMASCALE, 100 * 0.002f / canvas.transform.localScale.y / DioramaManager.DIORAMASCALE, 1);
    }

    public void PrevImage()
    {
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        for (int i = 1; i < MRs.Length; i++)
            MRs[i].enabled = false;

        if (counter > 1)
            counter = counter - 1;
        else
            counter = MRs.Length - 1;
        MRs[counter].enabled = true;
    }

    public void GoToFirstPicture()
    {
        MeshRenderer[] MRs = GetComponentsInChildren<MeshRenderer>();
        int length = MRs.Length;
        counter = length - 1;

        NextImage();
    }
}
