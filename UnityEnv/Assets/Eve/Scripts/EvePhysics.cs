using UnityEngine;

public class EvePhysics : MonoBehaviour
{
    public Arm LeftArm { get; private set; }
    public Arm RightArm { get; private set; }

    public Vector3 Position => head.position;
    public Vector3 LocalPosition => head.localPosition;

    public Vector3 UpAxis => head.up;
    public Vector3 RightAxis => head.right;
    public Vector3 ForwardAxis => head.forward;
    public Vector3 Velocity => rbHead.velocity;
    public Vector3 AngularVelocity => rbHead.angularVelocity; 

    private Transform head;
    private Transform body;
    private Rigidbody rbHead;
    private Rigidbody rbBody;
    private Transform armL;
    private Transform armR;
    private Vector3 defAnchorL;
    private Vector3 defAnchorR;
    private Vector3 anchorL => LocalizePos(LeftArm.Anchor);
    private Vector3 anchorR => LocalizePos(RightArm.Anchor);
    
    private const int groundLayer = 1 << 13;
    private const float heightAboveGround = 4.5f;

    public void Initialize()
    {
        head = transform.Find("Head");
        rbHead = head.GetComponent<Rigidbody>();
        body = transform.Find("Body");
        rbBody = body.GetComponent<Rigidbody>();

        armL = transform.Find("LeftArm");
        LeftArm = armL.GetComponent<Arm>();
        LeftArm.Initialize(-15, 105, -90, 90, 0, 135);
        defAnchorL = anchorL;

        armR = transform.Find("RightArm");
        RightArm = armR.GetComponent<Arm>();
        RightArm.Initialize(-15, 105, -90, 90, -135, 0);
        defAnchorR = anchorR;
    }

    public void Move(float x, float z)
    {
        rbHead.AddRelativeForce(Vector3.right * x + Vector3.forward * z, ForceMode.VelocityChange);
    }

    public void Turn(float force)
    {
        rbHead.AddRelativeTorque(Vector3.up * force, ForceMode.VelocityChange);
    }

    public void Hover()
    {
        float force = Mathf.Max(0f, -LocalPosition.y);
        RaycastHit hit;
        if (Physics.Raycast(Position, Vector3.down, out hit, 10f, groundLayer))
        {
            force = Mathf.Max(0f, heightAboveGround - hit.distance);
        }
        rbHead.AddForce(Vector3.up * force, ForceMode.VelocityChange);

        Vector3 stabilize = body.up * 10f;
        stabilize.y = 0;
        rbBody.AddForce(stabilize, ForceMode.VelocityChange);
    }

    public float[] GetNormalizedArmOffsets()
    {
        // -0.5 / +0.5
        return new float[]
        {
            (defAnchorL - anchorL).sqrMagnitude * 4f - 0.5f,
            (defAnchorR - anchorR).sqrMagnitude * 4f - 0.5f
        };
    }

    public Vector3 LocalizePos(Vector3 p)
    {
        return head.InverseTransformPoint(p);
    }
}
