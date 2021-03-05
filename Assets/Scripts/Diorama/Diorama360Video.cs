using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;

public class Diorama360Video : MonoBehaviour
{
    public GameObject Sphere360;
    public GameObject Video360OffButton;

    public MediaPlayer Media;
    public MeshRenderer Sample;
    private MeshRenderer Renderer;
    private ApplyToMaterial ApplyMaterial;

    public Material ImageMaterial;

    public GameObject PlayUI;

    public List<MediaPlayer> Unpause;

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

            Sphere360.gameObject.SetActive(false);
            Video360OffButton.SetActive(false);
        }
        else
        {
            Sphere360.transform.SetParent(null);
            Sphere360.transform.localScale = Vector3.one;
            Sphere360.gameObject.SetActive(true);
            Video360OffButton.SetActive(true);

            Renderer.material = ApplyMaterial._material;
            Sphere360.GetComponentInChildren<MeshRenderer>().material = Renderer.sharedMaterial;

            Media.Play();
            
            MediaPlayer[] videos = FindObjectsOfType<MediaPlayer>();
            foreach (MediaPlayer v in videos)
            {
                if (v.Control.IsPlaying() && v.gameObject.activeInHierarchy && v != Media)
                {
                    v.Control.Pause();
                    Unpause.Add(v);
                }
            }
        }
    }

    public void UnpauseAfterExit360()
    {
        foreach (MediaPlayer v in Unpause)
        {
            v.Control.Play();
        }
        Unpause.Clear();
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

        Renderer.enabled = false;

        StartCoroutine(Autoplay());
    }

    IEnumerator Autoplay()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        yield return new WaitForSeconds(0.25f);
        GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(0.25f);
        Video360OffButton.GetComponent<Button>().onClick.Invoke();
#endif
        yield return new WaitForSeconds(0.5f);
        if (autoplay)
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Sample.material = Renderer.sharedMaterial;
        PlayUI.SetActive(!Media.Control.IsPlaying());

        if (Media.Control.IsFinished() && !Media.m_Loop && !PlayUI.activeSelf)
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }

    private void OnDestroy()
    {
        Video360OffButton.SetActive(false);
    }
    private void OnDisable()
    {
        Video360OffButton.SetActive(false);
    }
}
