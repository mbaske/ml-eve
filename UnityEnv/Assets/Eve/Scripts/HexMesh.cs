using UnityEngine;
using System.Collections.Generic;

public class HexMesh : MonoBehaviour
{
    public float Size { get; private set; }
    public Vector3[] Vertices => mesh.vertices;

    private Mesh mesh;
    private const int extent = 20;
    private const float hexRadius = 0.5f;
    private MeshCollider coll;
    // Vertex motion on y-axis.
    private float[] velocities;

    public void Initialize()
    {
        List<Hexagon> hexagons = new List<Hexagon>();
        float sqrt3 = Mathf.Sqrt(3f);
        for (int q = -extent; q <= extent; q++)
        {
            int r1 = Mathf.Max(-extent, -q - extent);
            int r2 = Mathf.Min(extent, -q + extent);
            for (int r = r1; r <= r2; r++)
            {
                hexagons.Add(new Hexagon(new Vector3(
                    hexRadius * sqrt3 * (q + r / 2f), 0, hexRadius * 3f / 2f * r), hexRadius));
            }
        }

        Vector3[] vertices = VertexDict.ToArray();
        List<int> triangles = new List<int>();
        foreach (Hexagon hexagon in hexagons)
        {
            for (int i = 0; i < 18; i++)
            {
                triangles.Add(hexagon.Triangles[i / 3].Indices[i % 3]);
            }
        }

        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();

        Size = mesh.bounds.size.x;
        Vector3 min = mesh.bounds.min;
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uv[i] = new Vector2((vertices[i].x - min.x) / Size, (vertices[i].z - min.z) / Size);
        }
        mesh.uv = uv;
        mesh.RecalculateNormals(); // TODO
        mesh.RecalculateTangents(); // TODO
        mesh.MarkDynamic();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        coll = GetComponent<MeshCollider>();
        coll.sharedMesh = mesh;
    }
    
    public void ApplyModified(Vector3[] modVertices, float[] modVelocities)
    {
        velocities = modVelocities;
        mesh.vertices = modVertices;
        coll.sharedMesh = mesh; // TODO
    }

    public float GetVelocity(int triIndex, Vector3 baryCenter)
    {
        triIndex *= 3;
        int[] tris = mesh.triangles;
        return velocities[tris[triIndex]] * baryCenter.x
             + velocities[tris[triIndex + 1]] * baryCenter.y
             + velocities[tris[triIndex + 2]] * baryCenter.z;
    }

    private void OnDestroy()
    {
        Destroy(GetComponent<MeshFilter>().sharedMesh);
        Destroy(GetComponent<MeshCollider>().sharedMesh);
    }
}

public class Hexagon
{
    public Triangle[] Triangles { get; private set; }

    public Hexagon(Vector3 pos, float radius)
    {
        Triangles = new Triangle[6];
        for (int i = 0; i < 6; i++)
        {
            Triangles[i] = new Triangle(pos, radius, i);
        }
    }
}

public class Triangle
{
    public int[] Indices { get; private set; }

    public Triangle(Vector3 pos, float radius, int index)
    {
        Indices = new int[]
        {
            VertexDict.Add(pos),
            VertexDict.Add(pos + GetVertex(radius, index)),
            VertexDict.Add(pos + GetVertex(radius, index + 1))
        };
    }

    private Vector3 GetVertex(float radius, int index)
    {
        float angle = index * 60 * Mathf.Deg2Rad;
        return new Vector3(radius * Mathf.Sin(angle), 0, radius * Mathf.Cos(angle));
    }
}

public class VertexDict
{
    // x/y/z - vertex position, w - vertex index
    private static Dictionary<int, Vector4> dict = new Dictionary<int, Vector4>();

    public static int Add(Vector4 v4)
    {
        int key = GetHash(v4);
        if (dict.ContainsKey(key))
        {
            v4 = dict[key];
        }
        else
        {
            v4.w = dict.Count;
            dict.Add(key, v4);
        }
        return (int)v4.w;
    }

    public static Vector3[] ToArray()
    {
        Vector3[] v3 = new Vector3[dict.Count];
        foreach (KeyValuePair<int, Vector4> kvp in dict)
        {
            v3[(int)kvp.Value.w] = kvp.Value;
        }
        return v3;
    }

    private static int GetHash(Vector3 v3)
    {
        return Mathf.RoundToInt(v3.x * 10) * 1000 + Mathf.RoundToInt(v3.z * 10);
    }
}