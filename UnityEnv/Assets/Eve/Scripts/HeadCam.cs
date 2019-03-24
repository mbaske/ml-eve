using UnityEngine;

public class HeadCam : MonoBehaviour
{
    [SerializeField]
    private Transform camPos;

    [SerializeField]
    private float interpolate = 3f;

    private void LateUpdate()
    {
        float t = Time.deltaTime * interpolate;
        transform.position = Vector3.Lerp(transform.position, camPos.position, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, camPos.rotation, t);
    }
}
