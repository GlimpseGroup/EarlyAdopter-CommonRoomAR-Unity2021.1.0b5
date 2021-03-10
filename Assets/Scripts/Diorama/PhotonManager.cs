using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.PunBehaviour
{
    public static PhotonManager instance = null;

    string currentID;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        PhotonNetwork.ConnectUsingSettings("1");
    }

    public void DeInitialize()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public IEnumerator ConnectToPresentationPhotonRoom(string ID)
    {
        currentID = ID;
        while (!PhotonNetwork.connected)
            yield return new WaitForSeconds(1.5f);
        if (PhotonNetwork.inRoom)
            PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.JoinRoom("CommonRoom-" + currentID);
        Debug.Log("Connecting to Room CommonRoom-" + currentID);
    }
    
    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom("CommonRoom-" + currentID);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        DeInitialize();
    }
}
