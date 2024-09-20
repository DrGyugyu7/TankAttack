using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameMgr : MonoBehaviourPunCallbacks
{
    public static GameMgr Instance = null;
    [SerializeField] private Button exitButton;
    [SerializeField] TMP_Text connectInfoTxt;
    [SerializeField] private TMP_Text msgText;
    [SerializeField] private TMP_InputField chatMsgIf;
    [SerializeField] private Button senMgsBtn;
    [SerializeField] private TMP_Text playerListTxt;
    private PhotonView pv;
    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Start()
    {
        exitButton.onClick.AddListener(() => OnExitBtnClick());
        yield return new WaitForSeconds(0.5f);
        CreateTank();
        DisplayConnectInfo();
        pv = GetComponent<PhotonView>();
        senMgsBtn.onClick.AddListener(() => SendChatMsg());
        DisplayPlayerListInfo();
    }
    private void CreateTank()
    {
        Vector3 pos = new Vector3(Random.Range(-150, 150), 10f, Random.Range(-150, 150));
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }
    public void SendChatMsg()
    {
        string msg = $"<color=#00ff00>{PhotonNetwork.NickName}</color> : {chatMsgIf.text}";
        DisplayMsg(msg);
        pv.RPC(nameof(DisplayMsg), RpcTarget.OthersBuffered, msg);
    }
    [PunRPC]
    public void DisplayMsg(string msg)
    {
        msgText.text += msg + "\n";
    }
    private void OnExitBtnClick()
    {
        PhotonNetwork.LeaveRoom();
    }
    #region  photon callback func    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayConnectInfo();
        DisplayPlayerListInfo();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DisplayPlayerListInfo();
        DisplayConnectInfo();
    }

    private void DisplayConnectInfo()
    {
        int currPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        string roomName = PhotonNetwork.CurrentRoom.Name;
        string msg = $"[{roomName}]({currPlayer}/{maxPlayers})";
        connectInfoTxt.text = msg;
    }
    #endregion

    private void DisplayPlayerListInfo()
    {
        string playerList = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            string _color = player.IsMasterClient ? "#ff0000" : "#00ff00";
            playerList += $"<color={_color}>{player.NickName}</color>\n";
        }
        playerListTxt.text = playerList;
    }
}