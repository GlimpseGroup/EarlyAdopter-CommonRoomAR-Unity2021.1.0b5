using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{

    string TOKEN;

    [SerializeField] string username;
    [SerializeField] string password;

    // Start is called before the first frame update
    void Start()
    {

        JSONObject loginJSON = new JSONObject();
        loginJSON.Add("username", "testfaculty");
        loginJSON.Add("password", "##82vB3Y4W##");
        loginJSON.Add("issueJWT", true);
        string jsonString = loginJSON.ToString();

        StartCoroutine(postRequest("https://avenuesar.early-adopter.com/api/earlyaxr/v1/login", jsonString));


    }


    IEnumerator postRequest(string url, string jsonString)
    {
        var uwr = new UnityWebRequest(url, "POST");
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

            JSONNode jo = JSONNode.Parse(uwr.downloadHandler.text);
            TOKEN = jo["token"];
            Debug.Log(TOKEN);
        }
    }
        


    void Update()
    {
        if (Input.GetKey(KeyCode.Space) == true)
        {
            Debug.Log("Space pressed");

            JSONObject logoutJson = new JSONObject();
            logoutJson.Add("jwt", TOKEN);
            string jsonString = logoutJson.ToString();

            StartCoroutine(postRequest("https://avenuesar.early-adopter.com/api/earlyaxr/v1/logout", jsonString));
        }
    }




}
