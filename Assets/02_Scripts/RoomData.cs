using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviour
{
    [SerializeField] private TMP_Text roomText;

    private RoomInfo roomInfo;
    public RoomInfo RoomInfo
    {
        get
        {
            return roomInfo;
        }
        set
        {
            roomInfo = value;
            roomText.text = $"{roomInfo.Name} {roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
        }
    }
}
