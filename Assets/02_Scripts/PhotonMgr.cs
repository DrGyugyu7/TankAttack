using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Mono.Cecil;

public class PhotonMgr : MonoBehaviourPunCallbacks
{
    [Header("GameSetting")]
    [SerializeField] private const string version = "1.0";
    [SerializeField] private string nickName = "Dr.Gyugyu";

    [Header("UI")]
    [SerializeField] private TMP_InputField nickNameIF;
    [SerializeField] private TMP_InputField roomNameIF;

    [Header("Button")]
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button makeRoomBtn;

    [Header("Room List")]
    public GameObject roomPrefab;
    public Transform contentTr;
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        PhotonNetwork.GameVersion = version;
        //PhotonNetwork.NickName = nickName;

        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("NICKNAME"))
        {
            nickName = PlayerPrefs.GetString("NICKNAME");
            nickNameIF.text = nickName;
        }

        SetNickName();
        loginBtn.onClick.AddListener(() => OnLoginBtnClick());
        makeRoomBtn.onClick.AddListener(() => OnmakeRoomBtnClick());
    }

    private void OnmakeRoomBtnClick()
    {
        SetNickName();
        if (string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_{Random.Range(0, 1000):0000}";
        }
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom(roomNameIF.text, ro);
    }

    private void SetNickName()
    {
        if (string.IsNullOrEmpty(nickNameIF.text))
        {
            nickName = $"User_{Random.Range(0, 1000):0000}";
            nickNameIF.text = nickName;
        }
        nickName = nickNameIF.text;
        PhotonNetwork.NickName = nickName;
    }
    public void OnLoginBtnClick()
    {
        SetNickName();
        PlayerPrefs.SetString("NICKNAME", nickName);
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Failed : {message}");
        RoomOptions ro = new RoomOptions
        {
            MaxPlayers = 100,
            IsOpen = true,
            IsVisible = true
        };
        PhotonNetwork.CreateRoom("MyRoom" + Random.Range(0, 100), ro);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("BattleField");
        // Vector3 pos = new Vector3(Random.Range(-150, 150), 10f, Random.Range(-150, 150));
        // PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            Debug.Log($"{room.Name} {room.PlayerCount}/{room.MaxPlayers}");

            //newly created roombtn, on added/deleted

            if (room.RemovedFromList)
            {
                if (roomDict.TryGetValue(room.Name, out GameObject tempRoom))
                {
                    Destroy(tempRoom);
                    roomDict.Remove(room.Name);
                }
                continue;
            }

            if (!roomDict.ContainsKey(room.Name))
            {
                var _room = Instantiate(roomPrefab, contentTr);
                roomDict.Add(room.Name, _room);
                _room.GetComponent<RoomData>().RoomInfo = room;
            }
            else
            {
                if (roomDict.TryGetValue(room.Name, out GameObject tempRoom))
                {
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }

            }

        }
    }
}
