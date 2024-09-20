using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using Unity.Cinemachine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Unity.VisualScripting;
[RequireComponent(typeof(AudioSource))]
public class TankCtrl : MonoBehaviour
{
    private Transform tr;
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 20.0f;
    [SerializeField] private float turnSpeed = 100.0f;
    private float v => Input.GetAxis("Vertical");
    private float h => Input.GetAxis("Horizontal");
    private bool isFire => Input.GetMouseButtonDown(0);
    //[SerializeField] private Button mybutton;
    private bool isFly => Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q);

    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private Transform firePos;

    [SerializeField] private AudioClip fireSfx;
    private new AudioSource audio;

    [SerializeField] private CinemachineCamera cinemachineCamera;

    private PhotonView pv;

    public TMP_Text nickName;
    //private Transform cameraTr;

    private float initHp = 100.0f;
    private float currHp = 100.0f;
    private MeshRenderer[] renderers;
    [SerializeField] private Image hpbar;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        cinemachineCamera = GameObject.Find("Cinemachine Camera").GetComponent<CinemachineCamera>();
        pv = GetComponent<PhotonView>();
        if (pv.IsMine == true)
        {
            cinemachineCamera.Follow = tr;
            cinemachineCamera.LookAt = tr;
        }
        else
        {
            rb.isKinematic = true;
        }
        // transform.Find("Canvas/Panel/Txt-Nickname").GetComponent<TMP_Text>();
        nickName.text = pv.Owner.NickName;
        //cameraTr = Camera.main.transform;
        renderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        //mybutton.onClick.AddListener(() => fly());
    }
    private void Update()
    {
        if (!pv.IsMine) return;
        Locomotion();
        if (isFire)
        {
            pv.RPC(nameof(Fire), RpcTarget.AllViaServer, pv.Owner.ActorNumber);
        }
        if (isFly)
        {
            fly();
        }
    }
    private void Locomotion()
    {
        if (!pv.IsMine) return;
        tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed);
        tr.Rotate(Vector3.up * Time.deltaTime * h * turnSpeed);
    }
    [PunRPC]
    private void Fire(int actorNumber)
    {
        audio.PlayOneShot(fireSfx, 0.8f);
        var cannon = Instantiate(cannonPrefab, firePos.position, firePos.rotation);
        cannon.GetComponent<Cannon>().shooterId = actorNumber;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("CANNON"))
        {
            int actorNumber = other.gameObject.GetComponent<Cannon>().shooterId;
            Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            Debug.Log(pv.Owner.NickName + "Hit by " + player.NickName);
            currHp -= 10f;
            if (currHp <= 0)
            {
                string msg = $"<color=#00ff00>{pv.Owner.NickName}</color>님은 사망했습니다. 막타는 <color=#ff0000>{player.NickName}</color>!";
                GameMgr.Instance.DisplayMsg(msg);
                SetVisibleTank(false);
            }
            hpbar.fillAmount = currHp / initHp;
        }
    }
    private void SetVisibleTank(bool IsVisible)
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = IsVisible;
        }
        tr.Find("Canvas").gameObject.SetActive(IsVisible);
    }
    private void RespawnTank()
    {
        currHp = initHp;
        hpbar.fillAmount = 1.0f;
        SetVisibleTank(true);
    }
    private void fly()
    {
        int flydir;
        if (Input.GetKey(KeyCode.Q)) flydir = 1;
        else if (Input.GetKey(KeyCode.E)) flydir = -1;
        else flydir = 0;
        if (!pv.IsMine)
        {
            return;
        }
        rb.AddRelativeForce(Vector3.up * 10000f);

    }
}
