using UnityEngine;

public class OrbitCam : MonoBehaviour
{
    [SerializeField]
    private float speed = 1000f;
    [SerializeField]
    private float yOffset = -3f;

    private void LateUpdate()
    {
        float t = Time.deltaTime * speed;
        transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * t);
        transform.LookAt(Vector3.up * yOffset);
    }
}
