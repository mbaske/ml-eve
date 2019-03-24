using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    private Ball ball;
    [SerializeField]
    private EvePhysics eve;

    private HexMesh mesh;
    private MeshModifier modifier;
    private MeshModPoint[] modPoints;
    private float radius;

    private void Start()
    {
        // ball.RaiseCollisionEvent += OnBallCollision;

        mesh = GetComponent<HexMesh>();
        mesh.Initialize();
        radius = mesh.Size / 2f;

        modPoints = new MeshModPoint[]
        {
            new MeshModPoint(mesh.Size),
            new MeshModPoint(mesh.Size)
        };
        modifier = new MeshModifier(mesh, modPoints);
    }

    private void Update()
    {
        float y = ball.LocalPosition.y - 8f;
        modPoints[0].Update(
            new Vector2(ball.LocalPosition.x / radius, ball.LocalPosition.z / radius),
             -Util.Sigmoid(y / 12f), -Util.Sigmoid(y / 15f));
             
        modPoints[1].Update(
            new Vector2(eve.LocalPosition.x / radius, eve.LocalPosition.z / radius), 0f, -0.5f);

        modifier.Update();
    }

    private void OnDestroy()
    {
        modifier.Destroy();
    }

    // private void OnBallCollision(object sender, CollisionArgs e)
    // {
    //     if (e.CompareTag(Tags.GROUND))
    //     {
    //         // TODO more contact points?
    //         ContactPoint cp = e.Collision.GetContact(0);
    //         RaycastHit hit;
    //         if (Physics.Raycast(ball.Position, -cp.normal, out hit, 1f, 1 << 8))
    //         {
    //             float v = mesh.GetVelocity(hit.triangleIndex, hit.barycentricCoordinate);
    //             if (v > 0)
    //             {
    //                 // Upward moving mesh pushes ball.
    //                 v *= Vector3.Dot(Vector3.up, cp.normal);
    //                 ball.ChangeVelocity(cp.normal * v * 10f);
    //             }
    //         }
    //     }
    // }
}
