using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class ChannelManager : MonoBehaviour
{
    public static ChannelManager instance = null;

    public DioramaChannels ChannelCollection;

    [Header("Channels (UI)")]

    public GameObject ChannelSelectionUI;
    public GameObject ChannelSelectionContent;
    public Transform ChannelSelectionButton;

    public Transform LoadingBlockUI;
    public Transform ChangeChannelBlockUI;
    public Transform PrivateChannelLoadingUI;

    public GameObject FirstChannelScreen;

    public Button RoomPlacementModeButton;
    public Button WebinarModeButton;

    public void ReEnableFirstScreen()
    {
        FirstChannelScreen.SetActive(true);
    }

    public void LoadChannelSelection()
    {
        StartCoroutine(cr_LoadChannelSelection());
    }
    IEnumerator cr_LoadChannelSelection()
    {
        if (TOKEN == "")
            RequestToken();

        foreach (Transform t in ChannelSelectionContent.transform)
            if (t.name != "Channel-Template")
                Destroy(t.gameObject);

        //yield return StartCoroutine(co_RequestDioramaByChannel());
        yield return new WaitForSeconds(0.1f);
        for (int a = 0; a < ChannelCollection.channels.Length + AdminController.Instance.CURRENTPRIVATECHANNELS.Count; a++)
        {
            DioramaChannel c;
            if (a < ChannelCollection.channels.Length)
                c = ChannelCollection.channels[a];
            else
                c = AdminController.Instance.CURRENTPRIVATECHANNELS[a - ChannelCollection.channels.Length];

            Transform button = Instantiate(ChannelSelectionButton);
            SceneChannelColorManager.instance.ToChangeColor.Add(button.GetComponentInChildren<Button>().gameObject);
            button.SetParent(ChannelSelectionContent.transform);
            button.localRotation = Quaternion.identity;
            button.localScale = Vector3.one;
            button.GetComponentInChildren<Text>().text = c.name;
            button.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(c.buttonGraphic))
            {
                string url = c.buttonGraphic;
                string fileName = c.buttonGraphic.Substring(c.buttonGraphic.LastIndexOf('/') + 1);
                fileName = "Channels/" + c.name + "/" + fileName;
                Debug.Log(fileName + " ::: " + url);
                string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);

                if (!c.buttonGraphic.Substring(c.buttonGraphic.Length - 5).Contains("."))
                {
                    fileName += ".png";
                    savePath += ".png";
                }
                Texture2D image = new Texture2D(1, 1);
                byte[] bytes = File.ReadAllBytes(savePath);
                image.LoadImage(bytes);
                button.GetComponentInChildren<Button>().transform.GetChild(1).GetComponent<Image>().material.mainTexture = image;
                button.GetComponentInChildren<Button>().transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(true);
            }
            button.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                AdminController.Instance.CHANNELID = c.id;
                AdminController.Instance.CURRENTCHANNEL = c;
                Debug.Log("CHANNEL SELECTED : " + c.id + " : COLOR : " + c.color);
                SceneChannelColorManager.instance.ChangeColors();
                //ChannelSelectionUI.SetActive(false);
                StartCoroutine(co_RequestDioramaByCategory());
            });
        }
        ChannelSelectionUI.SetActive(true);

    }

    public void CreateChannelButton(DioramaChannel c)
    {
        Transform button = Instantiate(ChannelSelectionButton);
        SceneChannelColorManager.instance.ToChangeColor.Add(button.GetComponentInChildren<Button>().gameObject);
        button.SetParent(ChannelSelectionContent.transform);
        button.localRotation = Quaternion.identity;
        button.localScale = Vector3.one;
        button.GetComponentInChildren<Text>().text = c.name;
        button.gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(c.buttonGraphic))
        {
            string url = c.buttonGraphic;
            string fileName = c.buttonGraphic.Substring(c.buttonGraphic.LastIndexOf('/') + 1);
            fileName = "Channels/" + c.name + "/" + fileName;
            Debug.Log(fileName + " ::: " + url);
            string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);

            if (!c.buttonGraphic.Substring(c.buttonGraphic.Length - 5).Contains("."))
            {
                fileName += ".png";
                savePath += ".png";
            }
            Texture2D image = new Texture2D(1, 1);
            byte[] bytes = File.ReadAllBytes(savePath);
            image.LoadImage(bytes);
            button.GetComponentInChildren<Button>().transform.GetChild(1).GetComponent<Image>().material.mainTexture = image;
            button.GetComponentInChildren<Button>().transform.GetChild(1).GetComponent<Image>().gameObject.SetActive(true);
        }
        button.GetComponentInChildren<Button>().onClick.AddListener(delegate {
            AdminController.Instance.CHANNELID = c.id;
            AdminController.Instance.CURRENTCHANNEL = c;
            Debug.Log("CHANNEL SELECTED : " + c.id + " : COLOR : " + c.color);
            SceneChannelColorManager.instance.ChangeColors();
            //ChannelSelectionUI.SetActive(false);
            StartCoroutine(co_RequestDioramaByCategory());
        });
    }

    public void LoadPrivateChannel(Text text)
    {
        StartCoroutine(co_LoadPrivateChannel(text));
    }
    IEnumerator co_LoadPrivateChannel(Text text)
    {
        yield return StartCoroutine(co_RequestChannelByCode(text.text));

    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    [Header("BACKEND")]

    public string TOKEN = "";

    [Serializable]
    class DioramaAuthentication
    {
        [JsonProperty]
        public string username;
        [JsonProperty]
        public string password;
        [JsonProperty]
        public bool issueJWT;
    }

    [Serializable]
    public class DioramaAuthenticationTokenResponse
    {
        [JsonProperty]
        public string token;
        [JsonProperty]
        public string user_email;
        [JsonProperty]
        public string user_nicename;
        [JsonProperty]
        public string user_display_name;
    }

    public void RequestToken()
    {
        StartCoroutine(co_RequestToken());
    }
    IEnumerator co_RequestToken()
    {
        int TOKEN_attempts = 0;
        string uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/login";
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        DioramaAuthentication auth = new DioramaAuthentication();
        //auth.username = "arapp";
        auth.username = "testfaculty";
        //auth.password = "sXAuW$zWAc4W9euu1n";
        auth.password = "##82vB3Y4W##";
        auth.issueJWT = true;

        string jsonInput = JsonConvert.SerializeObject(auth, settings);

        byte[] body = Encoding.UTF8.GetBytes(jsonInput);

        for (int i = 0; i < 3; i++)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");

            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.chunkedTransfer = false;
            request.SendWebRequest();

            float totalTime = 0;
            while ((!request.isDone || !request.downloadHandler.isDone) && totalTime < 8)
            {
                yield return new WaitForSeconds(1f);
                totalTime++;
            }

            if (!string.IsNullOrEmpty(request.error))
            {
                TOKEN_attempts++;
                Debug.Log("REQUESTTOKEN NETWORK ERROR : " + request.error);
                yield break;
            }
            else if (totalTime < 8)
            {
                string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("REQUESTTOKEN : " + response);
                if (string.IsNullOrEmpty(response))
                {
                    TOKEN_attempts++;
                    Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM AUTHENTICATION TOKEN attempt#" + TOKEN_attempts + " : " + response);
                    Debug.Log("EMPTY RESPONSE: RESTARTING SCENE");
                    //SceneManager.LoadScene("ChoiceScene");
                    yield break;
                }

                DioramaAuthenticationTokenResponse tokenResponse = JsonConvert.DeserializeObject<DioramaAuthenticationTokenResponse>(response);
                TOKEN = tokenResponse.token;
                Debug.Log("Request Token is: " + TOKEN);

                AdminController.Instance.DioramaTOKEN = TOKEN;
                RequestChannel();
                break;
            }

            TOKEN_attempts++;
            if (i == 2)
            {
                Debug.Log("CONNECTION FAILED WHILE OBTAINING TOKEN? RESTARTING SCENE attempt#" + TOKEN_attempts);
                //SceneManager.LoadScene("ChoiceScene");
            }
            else
            {
                //string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM AUTHENTICATION TOKEN attempt#" + TOKEN_attempts);
            }
        }
    }

    // -1 = Not Running
    // 0 = Evaluating
    // 1 = Error
    // 100 = Requested Successfully
    private int RequestChannelByCodeReturnCode = -1;
    public void RequestChannelByCode(string code)
    {
        RequestChannelByCodeReturnCode = -1;
        if (TOKEN == "")
            RequestToken();
        StartCoroutine(co_RequestChannelByCode(code));
    }
    IEnumerator co_RequestChannelByCode(string code)
    {
        int CHANNEL_attempts = 0;
        RequestChannelByCodeReturnCode = 0;
        PrivateChannelLoadingUI.gameObject.SetActive(true);

        string uri = "";
        
        uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/channel/code/" + code;

        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = true,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        for (int i = 0; i < 1; i++)
        {
            UnityWebRequest request = new UnityWebRequest(uri);

            request.redirectLimit = 10;
            request.SetRequestHeader("Authentication", "Bearer " + TOKEN);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.chunkedTransfer = false;
            request.SendWebRequest();

            float totalTime = 0;
            while ((!request.isDone || !request.downloadHandler.isDone) && totalTime < 5)
            {
                yield return new WaitForSeconds(1f);
                totalTime++;
            }

            if (!string.IsNullOrEmpty(request.error))
            {
                CHANNEL_attempts++;
                Debug.Log("REQUESTESSION NETWORK ERROR : " + request.error);
                RequestChannelByCodeReturnCode = 1;
                yield break;
            }
            else if (totalTime < 5)
            {
                string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("REQUESTESSION : " + response);
                if (string.IsNullOrEmpty(response))
                {
                    CHANNEL_attempts++;
                    Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM PRIVATE CHANNEL attempt#" + CHANNEL_attempts + " : " + response);
                    PrivateChannelLoadingUI.gameObject.SetActive(false);
                    yield break;
                }

                RequestChannelByCodeReturnCode = 100;

                DioramaDocument LoadedDocument = JsonConvert.DeserializeObject<DioramaDocument>(response, settings);
                
                PrivateChannelLoadingUI.gameObject.SetActive(false);

                CreateChannelButton(LoadedDocument.channel);
                AdminController.Instance.CURRENTPRIVATECHANNELS.Add(LoadedDocument.channel);

                //AdminController.Instance.CHANNELID = LoadedDocument.channel.id;
                //AdminController.Instance.CURRENTCHANNEL = LoadedDocument.channel;
                //Debug.Log("CHANNEL SELECTED : " + LoadedDocument.channel.id + " : COLOR : " + LoadedDocument.channel.color);
                //SceneChannelColorManager.instance.ChangeColors();
                //ChannelSelectionUI.SetActive(false);
                break;
            }
            CHANNEL_attempts++;
            //string response = Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM PRIVATE CHANNEL attempt#" + CHANNEL_attempts);
            PrivateChannelLoadingUI.gameObject.SetActive(false);
        }
    }

    // -1 = Not Running
    // 0 = Evaluating
    // 1 = Error
    // 100 = Requested Successfully
    private int RequestChannelReturnCode = -1;
    public void RequestChannel()
    {
        RequestChannelReturnCode = -1;
        if (TOKEN == "")
            RequestToken();
        StartCoroutine(co_RequestChannel());
    }
    IEnumerator co_RequestChannel()
    {
        int CHANNEL_attempts = 0;
        RequestChannelReturnCode = 0;

        string uri = "";

        if (!string.IsNullOrEmpty(AdminController.Instance.SSOAuthTOKEN))
            uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/channels/by-user/" + TOKEN;
        else
            uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/channels";

        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = true,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        for (int i = 0; i < 3; i++)
        {
            UnityWebRequest request = new UnityWebRequest(uri);

            request.redirectLimit = 10;
            request.SetRequestHeader("Authentication", "Bearer " + TOKEN);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.chunkedTransfer = false;
            request.SendWebRequest();
            
            float totalTime = 0;
            while ((!request.isDone || !request.downloadHandler.isDone) && totalTime < 8)
            {
                yield return new WaitForSeconds(1f);
                totalTime++;
            }

            if (!string.IsNullOrEmpty(request.error))
            {
                CHANNEL_attempts++;
                Debug.Log("REQUESTESSION NETWORK ERROR : " + request.error);
                RequestChannelReturnCode = 1;
                if (uri != "https://cradmin.early-adopter.com/api/earlyaxr/v1/channels")
                    uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/channels";
                else
                    yield break;

            }
            else if (totalTime < 8)
            {
                string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("REQUESTESSION : " + response);
                if (string.IsNullOrEmpty(response))
                {
                    CHANNEL_attempts++;
                    Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM CHANNELS attempt#" + CHANNEL_attempts + " : " + response);
                    Debug.Log("EMPTY RESPONSE: RESTARTING SCENE");
                    //SceneManager.LoadScene("ChoiceScene");
                    yield break;
                }

                ChannelCollection = JsonConvert.DeserializeObject<DioramaChannels>(response, settings);

                foreach (DioramaChannel c in ChannelCollection.channels)
                {
                    if (!string.IsNullOrEmpty(c.buttonGraphic))
                    {
                        string url = c.buttonGraphic;
                        string fileName = c.buttonGraphic.Substring(c.buttonGraphic.LastIndexOf('/') + 1);
                        fileName = "Channels/" + c.name + "/" + fileName;
                        Debug.Log(fileName + " ::: " + url);
                        string savePath = string.Format("{0}/{1}", Application.persistentDataPath, fileName);

                        if (!c.buttonGraphic.Substring(c.buttonGraphic.Length - 5).Contains("."))
                        {
                            fileName += ".png";
                            savePath += ".png";
                        }

                        Texture2D image = new Texture2D(1, 1);
                        float aspectRatio = 1.0f;
                        if (!File.Exists(savePath))
                        {
                            using (UnityWebRequest www = UnityWebRequest.Get(url))
                            {
                                www.chunkedTransfer = false;
                                www.SendWebRequest();
                                float downloadtime = 0;
                                while ((!www.isDone || !www.downloadHandler.isDone) && downloadtime < 12)
                                {
                                    yield return new WaitForSeconds(1f);
                                    downloadtime++;
                                }
                                //yield return www.SendWebRequest();
                                if (!string.IsNullOrEmpty(www.error))
                                {
                                    Debug.Log(www.error);
                                }
                                else if (downloadtime < 12)
                                {
                                    Directory.CreateDirectory(savePath.Substring(0, savePath.LastIndexOf('/')));
                                    File.WriteAllBytes(savePath, www.downloadHandler.data);
                                    byte[] bytes = File.ReadAllBytes(savePath);
                                    image.LoadImage(bytes);
                                }
                                if (www != null && !www.isDone)
                                {
                                    www.Abort();
                                }
                            }
                        }
                    }
                }

                RequestChannelReturnCode = 100;

                //DrawlightSession save = JsonConvert.DeserializeObject<DrawlightSession>(response, settings);
                //Debug.Log(save.drawlight_session_id + ":" + Encoding.UTF8.GetString(save.drawlight_session));

                //DrawMenuManager.DrawSavedata(DrawlightSession.SessionToSavedata(save));
                LoadingBlockUI.gameObject.SetActive(false);
                ChangeChannelBlockUI.gameObject.SetActive(false);
                break;
            }
            CHANNEL_attempts++;
            if (i == 2)
            {
                Debug.Log("CONNECTION FAILED WHILE OBTAINING CHANNELS? RESTARTING SCENE attempt#" + CHANNEL_attempts);
                //SceneManager.LoadScene("ChoiceScene");
            }
            else
            {
                //string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM CHANNELS attempt#" + CHANNEL_attempts);
            }
        }
    }

    // -1 = Not Running
    // 0 = Evaluating
    // 1 = Error
    // 100 = Requested Successfully
    private int RequestDioramaByChannelReturnCode = -1;
    public void RequestDioramaByChannel()
    {
        RequestDioramaByChannelReturnCode = -1;
        if (TOKEN == "")
            RequestToken();
        StartCoroutine(co_RequestDioramaByCategory());
    }
    IEnumerator co_RequestDioramaByCategory()
    {
        int CATEGORY_attempts = 0;
        RequestDioramaByChannelReturnCode = 0;

        string uri = "";

        if (AdminController.Instance.CHANNELID < 0)
            uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/presentations-by-category";
        else
            uri = "https://cradmin.early-adopter.com/api/earlyaxr/v1/channel/id/" + AdminController.Instance.CHANNELID;

        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = true,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        for (int i = 0; i < 3; i++)
        {
            UnityWebRequest request = new UnityWebRequest(uri);

            request.redirectLimit = 10;
            request.SetRequestHeader("Authentication", "Bearer " + TOKEN);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.chunkedTransfer = false;
            request.SendWebRequest();

            float totalTime = 0;
            while ((!request.isDone || !request.downloadHandler.isDone) && totalTime < 8)
            {
                yield return new WaitForSeconds(1f);
                totalTime++;
            }

            if (!string.IsNullOrEmpty(request.error))
            {
                CATEGORY_attempts++;
                Debug.Log("REQUESTESSION NETWORK ERROR : " + request.error);
                RequestDioramaByChannelReturnCode = 1;
                ChannelSelectionUI.SetActive(false);
                yield break;
            }
            else if (totalTime < 8)
            {
                string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("REQUESTESSION : " + response);
                if (string.IsNullOrEmpty(response))
                {
                    CATEGORY_attempts++;
                    Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM CATEGORIES attempt#" + CATEGORY_attempts + " : " + response);
                    Debug.Log("EMPTY RESPONSE: RESTARTING SCENE");
                    //SceneManager.LoadScene("ChoiceScene");
                    yield break;
                }
                RequestDioramaByChannelReturnCode = 100;

                DioramaDocument LoadedDocument = JsonConvert.DeserializeObject<DioramaDocument>(response, settings);

                bool Webinar = false;
                foreach (DioramaCategory cat in LoadedDocument.categories) {
                    if (Webinar)
                        break;
                    if (cat.name == "Tutorial")
                        continue;
                    foreach (DioramaProject proj in cat.presentations) {
                        if (Webinar)
                            break;
                        if (!string.IsNullOrEmpty(proj.trackedImage)) {
                            Debug.Log("WebinarImageFound-NoSkippingWebinar : " + proj.trackedImage);
                            Webinar = true;
                        }
                    }
                }
                if (!Webinar) {
                    RoomPlacementModeButton.onClick.Invoke();
                }
                foreach (DioramaCategory cat in LoadedDocument.categories) {
                    if (!Webinar)
                        break;
                    if (cat.name == "Tutorial")
                        continue;
                    foreach (DioramaProject proj in cat.presentations) {
                        if (!Webinar)
                            break;
                        if (string.IsNullOrEmpty(proj.trackedImage)) {
                            Debug.Log("WebinarImageNotFound-NoSkippingRoomPlacement : " + proj.trackedImage);
                            Webinar = false;
                        }
                    }
                }
                if (Webinar) {
                    WebinarModeButton.onClick.Invoke();
                }

                ChannelSelectionUI.SetActive(false);
                break;
            }
            if (i == 2)
            {
                CATEGORY_attempts++;
                Debug.Log("CONNECTION FAILED WHILE OBTAINING CATEGORIES? RESTARTING SCENE attempt#" + CATEGORY_attempts);
                //SceneManager.LoadScene("ChoiceScene");
            }
            else
            {
                CATEGORY_attempts++;
                //string response = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log("ERROR : COULD NOT OBTAIN COMMONROOM CATEGORIES attempt#" + CATEGORY_attempts);
            }
            ChannelSelectionUI.SetActive(false);
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        instance = this;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        FirstChannelScreen.transform.localScale = Vector3.one;
        FirstChannelScreen.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        FirstChannelScreen.GetComponent<RectTransform>().sizeDelta = Vector3.zero;

        if (string.IsNullOrEmpty(AdminController.Instance.DioramaTOKEN))
            RequestToken();
        else
        {
            TOKEN = AdminController.Instance.DioramaTOKEN;
            RequestChannel();
        }

        //if (AdminController.Instance.CHANNELID > 0)
        //    FirstChannelScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (RequestChannelReturnCode == 100 && FirstChannelScreen.gameObject.activeSelf)
        {
            AttemptLoadChannels();
        }
    }

    void AttemptLoadChannels()
    {
        if (AttemptLoadChannelsReturnCode != -1)
            return;
        StartCoroutine(cr_AttemptLoadChannels());
    }
    int AttemptLoadChannelsReturnCode = -1;
    IEnumerator cr_AttemptLoadChannels()
    {
        AttemptLoadChannelsReturnCode = 0;
        yield return new WaitForSeconds(0.5f);
        if (RequestChannelReturnCode == 100)
        {
            FirstChannelScreen.gameObject.SetActive(false);
            LoadChannelSelection();
        }
        AttemptLoadChannelsReturnCode = -1;
    }

    public IEnumerator ReloadChannelScreen()
    {
        PrivateChannelLoadingUI.gameObject.SetActive(true);
        RequestChannelReturnCode = -1;
        yield return StartCoroutine(co_RequestChannel());
        StartCoroutine(cr_LoadChannelSelection());
        PrivateChannelLoadingUI.gameObject.SetActive(false);
    }
}
