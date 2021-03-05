using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using RenderHeads.Media.AVProVideo;
//using Observer;

public class MenuManagement : MonoBehaviour {

    //public Canvas SurveyCanvas;
    //public Canvas VideoCanvas;

    [Header("Admin Info")]
    public TextMeshProUGUI AdminLoginText;
    public GameObject AdminSkipSurveyButton;
    public string username;

    [HideInInspector]
    public Canvas MainMenu;

    [Header("ARea Tutorial")]
    public Transform AReaTutorialPrefab;

    // Use this for initialization
    void Start() {

        if (AdminController.GetInstance().isAdmin)
        {
            if (AdminLoginText != null)
            {
                AdminLoginText.text = "Logout";
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OpenDioramaTutorial()
    {
        if (AdminController.Instance.HasSeenTutorial)
        {
            LoadDioramaScene();
            return;
        }
        AdminController.Instance.HasSeenTutorial = true;

        Transform t = Instantiate(AReaTutorialPrefab);
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        t.Find("Exit").GetComponent<Button>().onClick.AddListener(delegate { Destroy(t.gameObject); });
        t.Find("Enter").GetComponent<Button>().onClick.AddListener(delegate { LoadDioramaScene(); });
        t.gameObject.SetActive(true);
        StartCoroutine(cr_OpenDioramaTutorial(t));
    }
    IEnumerator cr_OpenDioramaTutorial(Transform t)
    {
        t.GetComponentInChildren<ApplyToMaterial>().GetComponent<Image>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        t.GetComponentInChildren<ApplyToMaterial>().GetComponent<Image>().enabled = true;
    }
    public void SetAdminControllerDioramaWebinar(bool value)
    {
        if (AdminController.Instance)
        {
            AdminController.Instance.SetDioramaWebinar(value);
        }
    }
    public void LoadDioramaScene()
    {
        //Observer_Events.CustomEvent("LoadingDioramaScene").NoParameters();
        //SurveyCanvas.gameObject.SetActive(false);
        //VideoCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene("Diorama");
    }
    public void LoadMenuScene()
    {
        if (AgoraInterface.instance != null)
        {
            AgoraInterface.instance.LeaveToHome();
        }
        //Observer_Events.CustomEvent("LoadingMenuBasedScene").NoParameters();
        SceneManager.LoadScene("ChoiceScene");
    }

    public void HandleLoginDetails(CanvasGroup details)
    {
        //DownloadController.GetInstance().changeAdminStatus(details, AdminLoginText);
        //Observer_Events.CustomEvent("CheckLoginCredentials").NoParameters();

        RemoteLoginController.GetInstance().ChangeAdminStatus(details, AdminLoginText);
    }

    //public void ConfirmUserName(TMP_InputField text)
    //{
    //    DownloadController.GetInstance().CheckUserName(string.Compare(text.text.Trim(), DownloadController.USERNAME) == 0);
    //}

    //public void ConfirmPassword(TMP_InputField text)
    //{
    //   DownloadController.GetInstance().CheckPassword(string.Compare(text.text.Trim(), DownloadController.PASSWORD) == 0);
        
    //}

}
