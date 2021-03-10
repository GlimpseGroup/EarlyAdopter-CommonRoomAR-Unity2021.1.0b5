using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
//using Observer;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class AgoraInterface : MonoBehaviour
{
    public static AgoraInterface instance = null;

    public LayoutManager layoutManager;

    //Defining Agora Engine Instance
    private IRtcEngine mRtcEngine = null;

    //Enter unique app ID here
    [SerializeField] string appId = "YOUR APP ID";

    //UiD gets stored for volume manipulatio here
    string m_UiD_String = "0";

    public string AgoraPassword;
    public string AgoraPasswordInputted;

    public bool r_Muted = false;

    private void Awake()
    {
        instance = this;
        layoutManager.m_agoraInterface = this;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (mRtcEngine == null)
            return;
        //mRtcEngine.SetEnableSpeakerphone(true);
        //mRtcEngine.SetDefaultAudioRouteToSpeakerphone(true);
    }

    // Start is called before the first frame update
    void Start()
    {

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif

        //Initialising agora engine
        mRtcEngine = IRtcEngine.GetEngine(appId);
        mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR |
                                LOG_FILTER.CRITICAL);
        mRtcEngine.SetEnableSpeakerphone(true);
        mRtcEngine.SetDefaultAudioRouteToSpeakerphone(true);

        //When user joins channel
        mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) =>
        {
            Debug.Log("AGORA Joined Channel " + channelName);
            UserInfo info = mRtcEngine.GetUserInfoByUid(uid);
            layoutManager.AddNewUserElement(uid.ToString(), info.userAccount, true);
            m_UiD_String = uid.ToString();
            layoutManager.m_StartCallImg.SetActive(false);
            layoutManager.m_EndCallImg.SetActive(true);

            mRtcEngine.SetEnableSpeakerphone(true);
            mRtcEngine.SetDefaultAudioRouteToSpeakerphone(true);

            //layoutManager.SetMute(r_Muted);
            StartCoroutine(cr_JoinRoomMuteOption());
        };

        //Getting local stats
        mRtcEngine.OnUserJoined += (uint uid, int elapsed) =>
        {
            UserInfo info = mRtcEngine.GetUserInfoByUid(uid);
            string user_name = info.userAccount;
            string uid_string = uid.ToString();
            layoutManager.AddNewUserElement(uid_string, user_name);
        };

        mRtcEngine.OnUserMutedAudio += (uint uid, bool muted) =>
        {

        };

        //Listening to Agora Events
        mRtcEngine.OnUserOffline += (uint uid, USER_OFFLINE_REASON reason) =>
        {
            UserInfo info = mRtcEngine.GetUserInfoByUid(uid);
            layoutManager.RemoveUserElement(uid.ToString());
        };


        //When user leaves channel
        mRtcEngine.OnLeaveChannel += (RtcStats stats) =>
        {
            layoutManager.removeAllUsers();
            layoutManager.m_StartCallImg.SetActive(true);
            layoutManager.m_EndCallImg.SetActive(false);
            layoutManager.m_MuteImg.SetActive(false);
            layoutManager.m_NotMuteImg.SetActive(true);

        };

        mRtcEngine.OnUserInfoUpdated += (uint uid, UserInfo userInfo) =>
        {
            layoutManager.UpdateUsername(uid.ToString(), userInfo.userAccount);
        };

        //On Volume Indication
        mRtcEngine.EnableAudioVolumeIndication(200, 3, true);
        mRtcEngine.OnVolumeIndication += (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume) =>
        {
            for (int i = 0; i < speakerNumber; i++)
            {
                string uid_string = speakers[i].uid.ToString();
                uint vol = speakers[i].volume;
                if (uid_string == "0")
                {
                    layoutManager.modifySoundIcon(m_UiD_String, vol);
                }
                else
                {
                    layoutManager.modifySoundIcon(uid_string, vol);
                }
            }
        };

    }
    IEnumerator cr_JoinRoomMuteOption()
    {
        yield return new WaitForSeconds(1.5f);

        GameObject count = GameObject.Find("AgoraClientRoles");
        int broadcasters = 0;
        foreach (Transform t in count.transform)
            if (t.tag == "crBroadcaster")
                broadcasters++;
        if (broadcasters >= 7)
            r_Muted = true;

        layoutManager.SetMute(r_Muted);
    }

    public static void JoinRoomID(string ID, string username, bool CheckPreviousCallState = true)
    {
        if (instance == null)
            return;
        if (CheckPreviousCallState)
            if (instance.layoutManager.m_StartCallImg.activeSelf == true)
                return;
        if (instance.layoutManager.m_EndCallImg.activeSelf == true)
            instance.LeaveAndJoinDelay(ID, username);
        else
            instance.JoinChannel(ID, username);

    }
    public void LeaveAndJoinDelay(string ID, string username)
    {
        StartCoroutine(cr_LeaveAndJoinDelay(ID, username));
    }
    IEnumerator cr_LeaveAndJoinDelay(string ID, string username)
    {
        instance.LeaveChannel();
        yield return new WaitForSeconds(1.5f);
        instance.JoinChannel(ID, username);
    }
    public static void LeaveRoom()
    {
        if (instance == null)
            return;
        instance.LeaveChannel();
        //Observer_Events.CustomEvent("TOUCH")
        //    .AddParameter("assetName", "Agora-LeaveChannel")
        //    .AddParameter("startTime", Observer_Functions.GetCurrentTimeString())
        //    .EndParameters();
    }

    public void LeaveToHome()
    {
        mRtcEngine.LeaveChannel();
        PhotonManager.instance.DeInitialize();
        IRtcEngine.Destroy();
    }

    //Register new user
    public void RegisterNewUser(string username)
    {
        mRtcEngine.RegisterLocalUserAccount(appId, username);
    }


    //Joining Channel
    public void JoinChannel(string channelName, string user_name)
    {

        //Checking if text field is empty
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        mRtcEngine.JoinChannelWithUserAccount(null, channelName, user_name);
    }

    public void SwitchChannel(string channelName)
    {
        mRtcEngine.SwitchChannel(null, channelName);
    }

    //Leaving Channel
    public void LeaveChannel()
    {
        mRtcEngine.LeaveChannel();

    }

    public void ControlMute(bool mute)
    {
        mRtcEngine.MuteLocalAudioStream(mute);
        if (mute)
            mRtcEngine.SetClientRole(CLIENT_ROLE.AUDIENCE);
        else
            mRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);

    }
    public void SetClientRole(CLIENT_ROLE role)
    {
        mRtcEngine.SetClientRole(role);
    }

    public void OnChangeAgoraPasswordInputted(string password)
    {
        AgoraPasswordInputted = password;
    }

    private void OnApplicationQuit()
    {
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
        }
    }
}
