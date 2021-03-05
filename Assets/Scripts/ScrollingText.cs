using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{

    public GameObject textObject;
    public float scrollSpeed = 5.0f;

    public TextMeshProUGUI text;
    private GameObject clone;
    private TextMeshProUGUI cloneText;
    private RectTransform textRectTransform;

    // Use this for initialization
   public  void LoadScrollText(string videoName)
    {
        if (text == null)
        {
            text = textObject.GetComponent<TextMeshProUGUI>();
        }
        textRectTransform = text.gameObject.GetComponent<RectTransform>();
        if (text.preferredWidth < textRectTransform.rect.width)
        {

            DestroyImmediate(this);
        }
        else
        {
            cloneText = Instantiate(text, text.gameObject.transform);
            cloneText.text = videoName;
            /* clone = new GameObject();
              clone.AddComponent<TextMeshProUGUI>();
              clone.AddComponent<ContentSizeFitter>();
              cloneText = clone.GetComponent<TextMeshProUGUI>();
              cloneText.enableWordWrapping = false;
              cloneText.text = text.text;
              cloneText.font = text.font;
              cloneText.fontSize = text.fontSize;
              cloneText.alignment = text.alignment;*/
            RectTransform cloneRectTransform = cloneText.gameObject.GetComponent<RectTransform>();
            cloneRectTransform.anchorMax = new Vector2(1, 1);
            cloneRectTransform.SetParent(textRectTransform);
            cloneRectTransform.anchorMin = new Vector2(0, 0);
            cloneRectTransform.localPosition = new Vector3(text.preferredWidth, 0, cloneRectTransform.position.z);
            cloneRectTransform.localScale = new Vector3(1, 1, 1);
            //text.isTextOverflowing();
            StartCoroutine(StartScroll());
        }
    }

    private IEnumerator StartScroll()
    {
        yield return new WaitForSeconds(5);
  

            float width = text.preferredWidth;
            Vector3 startPosition = textRectTransform.localPosition;

            float scrollPosition = 0;
            textRectTransform.localPosition = Vector3.Lerp(textRectTransform.localPosition, new Vector3(-scrollPosition % width, startPosition.y, startPosition.z), .03f);
            while (true)
            {
                textRectTransform.localPosition = new Vector3(-scrollPosition % width, startPosition.y, startPosition.z);
                scrollPosition += scrollSpeed * 20 * Time.deltaTime;
                yield return null;
            }
        
    }
}
