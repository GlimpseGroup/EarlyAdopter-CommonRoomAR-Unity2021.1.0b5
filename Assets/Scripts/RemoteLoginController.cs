using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class RemoteLoginController : MonoBehaviour
{
    [SerializeField] string username;
    [SerializeField] string password;
    string loginUrlEndPoint = "https://avenuesar.early-adopter.com/api/earlyaxr/v1/login";
    string logoutUrlEndPoint = "https://avenuesar.early-adopter.com/api/earlyaxr/v1/logout";
    public static string TOKEN;
    public string TOKEN_READONLY;
    private static RemoteLoginController Instance;

    [Header("Callback")]
    public CanvasGroup LoginDetails;
    public List<TextMeshProUGUI> LoginText;
    [SerializeField]
    public List<Text> Username;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(AdminController.Instance.SSOAuthTOKEN))
        {
            TOKEN = AdminController.Instance.SSOAuthTOKEN;
            foreach (Text t in Username)
            {
                t.text = AdminController.Instance.Username;
            }
            foreach (TextMeshProUGUI t in LoginText)
                t.text = "Logout";
        }
    }

    public static RemoteLoginController GetInstance()
    {
        if (Instance == null)
        {
            GameObject g = new GameObject();
            g.name = "Remote Login Controller";
            Instance = g.AddComponent<RemoteLoginController>();
        }
        return Instance;
    }

    public void SetTokenAndLogoutText(string token, string text)
    {
        TOKEN = token;
        foreach (TextMeshProUGUI t in LoginText)
            t.text = text;
        LoginDetails.gameObject.SetActive(false);
        AdminController.GetInstance().isAdmin = true;
    }

    public void ChangeAdminStatus(CanvasGroup canvasGroup, TextMeshProUGUI AdminLoginText)
    {
        if (AdminLoginText.text == "Logout")
        {
            //Logout
            StartCoroutine(LogoutRequest(AdminLoginText));
            canvasGroup.gameObject.SetActive(false);

        }
        else{
            //Login
            if (canvasGroup.gameObject.activeSelf == true)
            {
                //Login form has been filled in
                canvasGroup.gameObject.SetActive(false);
                string username = canvasGroup.gameObject.transform.Find("Username").gameObject.GetComponent<TMP_InputField>().text;
                string password = canvasGroup.gameObject.transform.Find("Password").gameObject.GetComponent<TMP_InputField>().text;
                StartCoroutine(LoginRequest(username, password, AdminLoginText));
            }
            else
            {
                //Login form has yet to be filled in
                canvasGroup.gameObject.SetActive(true);
            }
        }
    }


    IEnumerator LoginRequest(string username, string password, TextMeshProUGUI AdminLoginText)
    {
        //Create Json input
        JSONObject loginJson = new JSONObject();
        loginJson.Add("username", username);
        loginJson.Add("password", password);
        loginJson.Add("issueJWT", true);
        string jsonString = loginJson.ToString();


        //Create Unity Web Request
        UnityWebRequest uwr = new UnityWebRequest(loginUrlEndPoint, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Recieved: " + uwr.downloadHandler.text);

            //Recieve token
            JSONNode jsonResponse = JSONNode.Parse(uwr.downloadHandler.text);
            TOKEN = jsonResponse["token"];

            if (TOKEN != null)
            {
                foreach(Text t in Username)
                    t.text = username;

                foreach (TextMeshProUGUI t in LoginText)
                    t.text = "Logout";

                //Pass Login details to AREA scene
                MenuManagement SurveyObjectMenuManager = GameObject.Find("SurveyObject").GetComponent<MenuManagement>();
                if (SurveyObjectMenuManager != null)
                {
                    SurveyObjectMenuManager.username = username;
                }

            }
        }
    }


    IEnumerator LogoutRequest(TextMeshProUGUI AdminLoginText)
    {
        //Create Json request
        JSONObject logoutJson = new JSONObject();
        logoutJson.Add("jwt", TOKEN);
        string jsonString = logoutJson.ToString();

        //Create Unity Web Request
        UnityWebRequest uwr = new UnityWebRequest(logoutUrlEndPoint, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Recieved: " + uwr.downloadHandler.text);

            foreach (TextMeshProUGUI t in LoginText)
                t.text = "Login";
            TOKEN = "";
            foreach (Text t in Username)
                t.text = username;

            AdminController.Instance.CHANNELID = -1;
            AdminController.Instance.CURRENTCHANNEL = null;
            AdminController.Instance.CURRENTPRIVATECHANNELS.Clear();
            ChannelManager.instance.ReEnableFirstScreen();
        }
    }

    private void Update()
    {
        TOKEN_READONLY = TOKEN;
    }

}