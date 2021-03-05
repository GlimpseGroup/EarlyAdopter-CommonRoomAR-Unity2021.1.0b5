using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneChannelColorManager : MonoBehaviour
{
    public static SceneChannelColorManager instance = null;

    [SerializeField]
    Image AutoAdd;
    [SerializeField]
    Image AutoAddContrast;

    [SerializeField]
    public List<GameObject> ToChangeColor;
    [SerializeField]
    public List<GameObject> ToChangeContrastColor;

    private void Awake()
    {
        instance = this;

        AttemptAutoAdd();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!this.isActiveAndEnabled)
            return;
        if (AdminController.Instance.CURRENTCHANNEL == null)
            return;
        if (AdminController.Instance.CURRENTCHANNEL.color == null)
            return;
        if (string.IsNullOrEmpty(AdminController.Instance.CURRENTCHANNEL.color))
            return;
        
        float red = int.Parse("" + AdminController.Instance.CURRENTCHANNEL.color[1] + AdminController.Instance.CURRENTCHANNEL.color[2], System.Globalization.NumberStyles.HexNumber) * 0.004f;
        float green = int.Parse("" + AdminController.Instance.CURRENTCHANNEL.color[3] + AdminController.Instance.CURRENTCHANNEL.color[4], System.Globalization.NumberStyles.HexNumber) * 0.004f;
        float blue = int.Parse("" + AdminController.Instance.CURRENTCHANNEL.color[5] + AdminController.Instance.CURRENTCHANNEL.color[6], System.Globalization.NumberStyles.HexNumber) * 0.004f;

        float redc = int.Parse("" + AdminController.Instance.CURRENTCHANNEL.textColor[1] + AdminController.Instance.CURRENTCHANNEL.textColor[2], System.Globalization.NumberStyles.HexNumber) * 0.004f;
        float greenc = int.Parse("" + AdminController.Instance.CURRENTCHANNEL.textColor[3] + AdminController.Instance.CURRENTCHANNEL.textColor[4], System.Globalization.NumberStyles.HexNumber) * 0.004f;
        float bluec = int.Parse("" + AdminController.Instance.CURRENTCHANNEL.textColor[5] + AdminController.Instance.CURRENTCHANNEL.textColor[6], System.Globalization.NumberStyles.HexNumber) * 0.004f;

        Debug.Log("PARSED COLOR : " + red + " : " + green + " : " + blue);
        foreach (GameObject o in ToChangeColor)
        {
            if (o == null) {
                continue;
            }
            if (o.GetComponent<Image>() != null)
                o.GetComponent<Image>().color = new Color(red, green, blue);
            if (o.GetComponent<Text>() != null)
                o.GetComponent<Text>().color = new Color(red, green, blue);
            if (o.GetComponent<TextMeshPro>() != null)
                o.GetComponent<TextMeshPro>().color = new Color(red, green, blue);
            if (o.GetComponent<TextMeshProUGUI>() != null)
                o.GetComponent<TextMeshProUGUI>().color = new Color(red, green, blue);
        }

        Debug.Log("PARSED CONTRAST COLOR : " + redc + " : " + greenc + " : " + bluec);
        foreach (GameObject o in ToChangeContrastColor)
        {
            if (o == null)
            {
                continue;
            }
            if (o.GetComponent<Image>() != null)
                o.GetComponent<Image>().color = new Color(redc, greenc, bluec);
            if (o.GetComponent<Text>() != null)
                o.GetComponent<Text>().color = new Color(redc, greenc, bluec);
            if (o.GetComponent<TextMeshPro>() != null)
                o.GetComponent<TextMeshPro>().color = new Color(redc, greenc, bluec);
            if (o.GetComponent<TextMeshProUGUI>() != null)
                o.GetComponent<TextMeshProUGUI>().color = new Color(redc, greenc, bluec);
        }
    }

    public void ChangeColors()
    {
        if (!this.isActiveAndEnabled)
            return;
        Start();
    }

    public void AttemptAutoAdd()
    {
        Color c = AutoAdd.color;
        Debug.Log("DEFAULT COLOR: " + c);

        Color cc = AutoAddContrast.color;
        Debug.Log("DEFAULT CONTRAST COLOR: " + cc);

        foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (g.GetComponent<Image>() != null)
                if (g.GetComponent<Image>().color == c)
                    ToChangeColor.Add(g);
            if (g.GetComponent<Text>() != null)
                if (g.GetComponent<Text>().color == c)
                    ToChangeColor.Add(g);
            if (g.GetComponent<TextMeshPro>() != null)
                if (g.GetComponent<TextMeshPro>().color == c)
                    ToChangeColor.Add(g);
            if (g.GetComponent<TextMeshProUGUI>() != null)
                if (g.GetComponent<TextMeshProUGUI>().color == c)
                    ToChangeColor.Add(g);
        }

        foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (ToChangeColor.Contains(g))
                continue;
            //if (g.GetComponent<Image>() != null)
            //    if (g.GetComponent<Image>().color == cc)
            //        ToChangeContrastColor.Add(g);
            if (g.GetComponent<Text>() != null)
                if (g.GetComponent<Text>().color == cc)
                    ToChangeContrastColor.Add(g);
            if (g.GetComponent<TextMeshPro>() != null)
                if (g.GetComponent<TextMeshPro>().color == cc)
                    ToChangeContrastColor.Add(g);
            if (g.GetComponent<TextMeshProUGUI>() != null)
                if (g.GetComponent<TextMeshProUGUI>().color == cc)
                    ToChangeContrastColor.Add(g);
        }
    }

    // Update is called once per frame
    void Update()
    {
        NetworkFailure.alpha = NetworkFailureAlpha;
        NetworkFailureAlpha -= Time.deltaTime;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Network Check")]
    public CanvasGroup NetworkFailure;
    float NetworkFailureAlpha = 0;

    IEnumerator checkInternetConnection(Action<bool> action)
    {
        while (this.isActiveAndEnabled)
        {
            WWW www = new WWW("http://google.com");
            yield return www;
            if (www.error != null)
            {
                //action(false);
            }
            else
            {
                //action(true);
                NetworkFailureAlpha = 4;
            }
            yield return new WaitForSeconds(10f);
        }
    }

}
