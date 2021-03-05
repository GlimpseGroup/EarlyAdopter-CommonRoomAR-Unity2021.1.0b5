using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UIController : MonoBehaviour
{
	public static UIController instance;

	public delegate void SceneTransition(int sceneNumber);

	public event SceneTransition LoadScene;

    public CanvasGroup startScreen;
    private float startDelay = 3f;
	public GameObject infoPanel;
    [HideInInspector]
    public SwipeInstructionPanel swipePanel;
	[SerializeField]
	private List<GameObject> tutorialsScreen;

	private int screenCounter;
	
	private void Awake()
	{
        Debug.Log("0:Awake inside the UIController");
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);
		
		//DontDestroyOnLoad(this.gameObject);
		Debug.Log("tutorial ScreenCount " + tutorialsScreen.Count);
	}

	private void Start()
	{
        Debug.Log("1:Start Inside the UIController");
		ShowInstruction();
        //HidelInstruction();

        swipePanel = GetComponentInChildren<SwipeInstructionPanel>();
	}

	public void HidelInstruction()
	{
		Debug.Log("hide panel ");
		infoPanel.SetActive(false);
		//Camera.main.enabled = true;
		Screen.orientation = ScreenOrientation.AutoRotation;
	}

	public void ShowInstruction()
	{
		infoPanel.SetActive(true);
	}

	
	public void GoToNextSceen()
	{
		//screenCounter++;
		if (screenCounter < tutorialsScreen.Count - 1)
		{
			screenCounter++;
			tutorialsScreen[screenCounter].gameObject.SetActive(true);
			tutorialsScreen[screenCounter - 1].gameObject.SetActive(false);
		}
		else
		{
			HidelInstruction();
					
		}
	}

	public void GoToPreviousScreen()
	{
		if (screenCounter > 0)
		{
			screenCounter--;
			tutorialsScreen[screenCounter].gameObject.SetActive(true);
			tutorialsScreen[screenCounter + 1].gameObject.SetActive(false);
		}
		
	}

    private void Update()
    {
        if (!startScreen)
            return;
        startDelay -= Time.deltaTime;
        startScreen.alpha = startDelay;
        if (startScreen.alpha <= 0)
            Destroy(startScreen.gameObject);
    }

}
