using UnityEngine;

public class Arm : MonoBehaviour
{
    public Vector3 Velocity => rb.velocity;
    public Vector3 AngularVelocity => rb.angularVelocity; 
    public Vector3 Anchor => transform.TransformPoint(joint.anchor);

    private Vector3 center;
    private Vector3 halfRange;
    private Vector3 targetPos;

    private ConfigurableJoint joint;
    private JointDrive drive;
    private Rigidbody rb;

    public void Initialize(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
    {
        halfRange = new Vector3((xMax - xMin) / 2f, (yMax - yMin) / 2f, (zMax - zMin) / 2f);
        center = new Vector3(xMin, yMin, zMin) + halfRange;

        joint = GetComponent<ConfigurableJoint>();
        drive = joint.slerpDrive;
        rb = GetComponent<Rigidbody>();
    }

    public void Rotate(Vector3 rot, float force)
    {
        // rot, force -> -1/+1
        // TODO high joint rotation delta can break physics.
        // Workaround: interpolation.
        Quaternion r = Quaternion.Euler(center + Vector3.Scale(halfRange, rot));
        joint.targetRotation = Quaternion.Slerp(joint.targetRotation, r, 0.25f);
        drive.maximumForce = Mathf.Pow(10f, 3f + force); // 100 - 10000
        joint.slerpDrive = drive;
    }

    public float[] GetNormalizedRotation()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        return new float[]
        {
            Util.ClampAngle(rot.x) / 180f,
            Util.ClampAngle(rot.y) / 180f,
            Util.ClampAngle(rot.z) / 180f
        };
    }

    // Only move on y-axis from 0 to 0.5
    public void Move(float y)
    {
        // y -> -1/+1
        targetPos.y = y / 4f + 0.25f;
        joint.targetPosition = targetPos;
    }

    public float GetNormalizedDriveForce()
    {
        return Mathf.Log10(drive.maximumForce) - 3f;
    }
}
