using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTr;
    private void Start()
    {
        cameraTr = Camera.main.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(cameraTr);
    }
}