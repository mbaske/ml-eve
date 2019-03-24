using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class MeshModifier
{
    struct MeshModJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        public NativeArray<float> velocities;
        public Vector4 mpv0;
        public Vector4 mpv1;

        public void Execute(int i)
        {
            Vector3 vertex = vertices[i];
            Vector2 v2D = new Vector2(vertex.x, vertex.z);
            Vector2 c2D;
            float y = 0;
            float d;

            c2D = mpv0;
            d = 1f - Mathf.Min((v2D - c2D).sqrMagnitude / mpv0.z, 1f);
            y += d * d * mpv0.w;

            c2D = mpv1;
            d = 1f - Mathf.Min((v2D - c2D).sqrMagnitude / mpv1.z, 1f);
            y += d * d * mpv1.w;

            velocities[i] = y - vertex.y;
            vertex.y = y;
            vertices[i] = vertex;
        }
    }

    private MeshModPoint[] modPoints;
    private NativeArray<Vector3> vertices;
    private NativeArray<float> velocities;
    private Vector3[] modVertices;
    private float[] modVelocities;
    private MeshModJob meshModJob;
    private JobHandle jobHandle;
    private HexMesh mesh;

    public MeshModifier(HexMesh mesh, MeshModPoint[] modPoints)
    {
        this.mesh = mesh;
        this.modPoints = modPoints;

        vertices = new NativeArray<Vector3>(mesh.Vertices, Allocator.Persistent);
        velocities = new NativeArray<float>(vertices.Length, Allocator.Persistent);
        modVertices = new Vector3[vertices.Length];
        modVelocities = new float[vertices.Length];
    }

    public void Update()
    {
        meshModJob = new MeshModJob()
        {
            vertices = vertices,
            velocities = velocities,
            mpv0 = modPoints[0].Values,
            mpv1 = modPoints[1].Values
        };

        jobHandle = meshModJob.Schedule(vertices.Length, 64);
        jobHandle.Complete();
        meshModJob.vertices.CopyTo(modVertices);
        meshModJob.velocities.CopyTo(modVelocities);
        mesh.ApplyModified(modVertices, modVelocities);
    }

    public void Destroy()
    {
        vertices.Dispose();
        velocities.Dispose();
    }
}