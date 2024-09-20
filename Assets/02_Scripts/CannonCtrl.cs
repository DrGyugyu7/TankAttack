using Photon.Pun;
using TreeEditor;
using UnityEngine;

public class CannonCtrl : MonoBehaviour
{
    [SerializeField] private float speed = 1000.0f;
    private float r => Input.GetAxis("Mouse ScrollWheel");
    private PhotonView pv;
    private void Start()
    {
        pv = transform.root.GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (!pv.IsMine) return;
        transform.Rotate(Vector3.right * Time.deltaTime * r * speed);
    }
}
