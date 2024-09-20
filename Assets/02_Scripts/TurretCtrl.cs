using Photon.Pun;
using UnityEngine;

public class TurretCtrl : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 20.0f;
    private PhotonView pv;
    private void Start()
    {
        pv = transform.root.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!pv.IsMine) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << 8))
        {
            //Debug.Log(hit);
            Vector3 pos = transform.InverseTransformPoint(hit.point);
            float angle = Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;
            transform.Rotate(Vector3.up * angle * Time.deltaTime * turnSpeed);
        }
    }
}
