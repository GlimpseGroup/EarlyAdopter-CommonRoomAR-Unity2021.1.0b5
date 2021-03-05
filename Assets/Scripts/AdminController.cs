using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdminController : MonoBehaviour
{
    public static AdminController Instance = null;
    public string SSOAuthTOKEN = "";
    public bool isAdmin;
    public static bool read_isAdmin = false;
    public string Username = "";

    [Header("Diorama Info")]
    public bool HasSeenTutorial = false;
    public bool DioramaWebinar = false;
    // DIORAMA TOKEN, NOT SSO
    public string DioramaTOKEN = "";
    public int CHANNELID = -1;
    public DioramaChannel CURRENTCHANNEL;
    public List<DioramaChannel> CURRENTPRIVATECHANNELS = new List<DioramaChannel>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            isAdmin = false;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null)
        {
            Destroy(this);
        }
    }

    public static AdminController GetInstance()
    {
        if (Instance == null)
        {
            GameObject g = new GameObject();
            Instance = g.AddComponent<AdminController>();
            Instance.isAdmin = false;
            DontDestroyOnLoad(Instance.gameObject);
        }
        return Instance;
    }

    // Update is called once per frame
    void Update()
    {
        isAdmin = !string.IsNullOrEmpty(RemoteLoginController.TOKEN);

        read_isAdmin = isAdmin;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetDioramaWebinar(bool value)
    {
        DioramaWebinar = value;
    }
}
