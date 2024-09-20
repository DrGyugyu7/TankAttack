using Photon.Pun;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private float force = 120000.0f;
    [SerializeField] private GameObject expEffect;
    public int shooterId;

    private void Awake()
    {
        expEffect = Resources.Load<GameObject>("BigExplosion");
    }
    private void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * force);
        Destroy(this.gameObject, 5f);
    }
    private void OnCollisionEnter(Collision other)
    {
        Destroy(this.gameObject);
        var obj = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(obj, 3f);
    }
}
