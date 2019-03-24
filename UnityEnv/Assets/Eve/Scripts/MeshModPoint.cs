using UnityEngine;

public class MeshModPoint
{
    public Vector4 Values { get; private set; }

    private Vector4 values;
    private float radius;
    private float sqrSize;

    private const float multiplier = 3f; // TBD

    public MeshModPoint(float sheetSize)
    {
        radius = sheetSize / 2f;
        sqrSize = sheetSize * sheetSize;
        ReSet();
    }

    public void ReSet()
    {
        Values = new Vector4();
        values = new Vector4();
    }

    public void Update(Vector2 posNorm, float slopeNorm, float strengthNorm)
    {
        values.x = posNorm.x * radius;
        values.y = posNorm.y * radius;
        values.z = sqrSize * (slopeNorm + 1f) / 8f;
        values.w = strengthNorm * multiplier;

        float t = Time.deltaTime * 2f;
        Values = Vector4.Lerp(Values, values, t);
    }
}