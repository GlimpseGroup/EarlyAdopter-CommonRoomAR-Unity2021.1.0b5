using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;

public class DioramaVideo : MonoBehaviour
{
    public MediaPlayer Media;
    private MeshRenderer Renderer;
    private ApplyToMaterial ApplyMaterial;

    public Material ImageMaterial;

    public GameObject PlayUI;

    public bool autoplay = false;

    public void PlayOrStop()
    {
        if (Media.Control.IsPlaying())
        {
            Media.Stop();
            Media.Rewind(true);

            if (ApplyMaterial._defaultTexture)
            {
                Renderer.material = new Material(ImageMaterial);
                Renderer.material.mainTexture = ApplyMaterial._defaultTexture;
            }
        }
        else
        {
            Media.Play();

            Renderer.material = ApplyMaterial._material;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ApplyMaterial = Media.GetComponent<ApplyToMaterial>();
        Renderer = Media.GetComponent<MeshRenderer>();

        if (ApplyMaterial._defaultTexture)
        {
            Renderer.material = new Material(ImageMaterial);
            Renderer.material.mainTexture = ApplyMaterial._defaultTexture;
        }

        if (autoplay)
            StartCoroutine(Autoplay());
    }

    IEnumerator Autoplay()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Button>().onClick.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        PlayUI.SetActive(!Media.Control.IsPlaying());

        if (Media.Control.IsFinished() && !Media.m_Loop && !PlayUI.activeSelf)
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
