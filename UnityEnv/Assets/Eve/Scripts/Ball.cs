using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Vector3 LocalPosition => transform.localPosition;
    public Vector3 Velocity => rb.velocity;
    public Vector3 AngularVelocity => rb.angularVelocity;

    [HideInInspector]
    public event EventHandler<CollisionArgs> RaiseCollisionEvent;

    [SerializeField]
    private float spawnRadius = 5f;

    private Rigidbody rb;
    private Vector3 defPos;

    public void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        defPos = transform.localPosition;
    }

    public void ReSet()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = defPos + UnityEngine.Random.insideUnitSphere * spawnRadius;
    }

    public void ChangeVelocity(Vector3 vel)
    {
        rb.AddForce(vel, ForceMode.VelocityChange);
    }

    private void OnRaiseCollisionEvent(CollisionArgs e)
    {
        EventHandler<CollisionArgs> handler = RaiseCollisionEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        OnRaiseCollisionEvent(new CollisionArgs(other, CollisionArgs.CollisionState.Enter));
    }
}

public class CollisionArgs : EventArgs
{
    public enum CollisionState
    {
        Enter,
        Stay,
        Exit
    }

    public Collision Collision { get; private set; }
    public CollisionState State { get; private set; }

    public CollisionArgs(Collision collision, CollisionState state)
    {
        Collision = collision;
        State = state;
    }

    public bool CompareTag(string tag)
    {
        return Collision.gameObject.tag == tag;
    }
}

public class Tags
{
    public const string GROUND = "Ground";
    public const string WALL = "Wall";
    public const string BALL = "Ball";
    public const string HEAD = "Head";
    public const string BODY = "Body";
    public const string ARM = "Arm";
    public const string OUT = "Out";
}