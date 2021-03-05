using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GridSetUp : MonoBehaviour {
    GridLayoutGroup mGridGroup;
    RectTransform parentRect;
    public bool isEmail;
	// Use this for initialization
	void Start () {
        parentRect = gameObject.GetComponent<RectTransform>();
        mGridGroup = gameObject.GetComponent<GridLayoutGroup>();

        if (isEmail)
        {
            mGridGroup.cellSize = new Vector2(parentRect.rect.width, parentRect.rect.height / mGridGroup.constraintCount);
        }
        else
        {
            int rows = getRowCount();
            mGridGroup.cellSize = new Vector2(parentRect.rect.width / mGridGroup.constraintCount, parentRect.rect.height / rows);
        }

	}

    private int getRowCount()
    {
     int rows = 1;
       RectTransform childObj1 = mGridGroup.transform.GetChild(0).GetComponent<RectTransform>();
       Vector2 pos1 = childObj1.anchoredPosition;
       
       for (int i = 1; i < mGridGroup.transform.childCount; i++)
       {
       
           RectTransform currObj = mGridGroup.transform.GetChild(i).GetComponent<RectTransform>();
           Vector2 currPos = currObj.anchoredPosition;
           if (pos1.x == currPos.x && pos1.y != currPos.y)
           {
                rows++;
           }
       }

       return rows;
   }

}
