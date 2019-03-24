using UnityEngine;

public class Util
{
    public static float ClampAngle(float angle)
    {
        angle = ((angle % 360) + 360) % 360;
        return angle > 180 ? angle - 360 : angle;
    }

    public static Vector3 Sigmoid(Vector3 v)
    {
        v.x = Sigmoid(v.x);
        v.y = Sigmoid(v.y);
        v.z = Sigmoid(v.z);
        return v;
    }

    public static float Sigmoid(float val)
    {
        return 2f / (1f + Mathf.Exp(-2f * val)) - 1f;
    }

    public static Vector3 ClampNorm(Vector3 v)
    {
        v.x = Mathf.Clamp(v.x, -1f, 1f);
        v.y = Mathf.Clamp(v.y, -1f, 1f);
        v.z = Mathf.Clamp(v.z, -1f, 1f);
        return v;
    }
}