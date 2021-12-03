using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
  [CreateAssetMenu(fileName = "New Voxel Type", menuName = "Voxels/Basic")]
  public class Voxel : ScriptableObject
  {
    [HideInInspector] public List<Rect> uvs;
    public Texture2D[] textures = new Texture2D[1];
    public bool hasGameObject = false;
    public GameObject gameObject;


    public class MeshData
    {
      public MeshData()
      {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
      }
      public List<Vector3> vertices;
      public List<int> triangles;
      public List<Vector2> uvs;
    }

    public struct Face
    {
      public Face(Vector3Int _direction, Vector3[] _vertices, int _index)
      {
        direction = _direction;
        vertices = _vertices;
        index = _index;
      }

      public Vector3Int direction;
      public Vector3[] vertices;
      public int index;

      public static Face[] faces =
      {
        new Face(Vector3Int.up, new[] {new Vector3(-0.5F, 0.5F, -0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.5F, -0.5F)}, 0),
        new Face(Vector3Int.down, new[] {new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.5F, -0.5F)}, 1),
        new Face(Vector3Int.forward, new[] {new Vector3(0.5F, -0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F)}, 2),
        new Face(Vector3Int.back, new[] {new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(-0.5F, 0.5F, -0.5F), new Vector3(0.5F, 0.5F, -0.5F), new Vector3(0.5F, -0.5F, -0.5F)}, 3),
        new Face(Vector3Int.left, new[] {new Vector3(-0.5F, -0.5F, 0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(-0.5F, 0.5F, -0.5F), new Vector3(-0.5F, -0.5F, -0.5F)}, 4),
        new Face(Vector3Int.right, new[] {new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, -0.5F, 0.5F)}, 5)
      };
    }

    public virtual MeshData GetVoxel(Vector3 pos, bool[] neighbours, int offset)
    {
      MeshData meshData = GetCollisionVoxel(pos, neighbours, offset);
      int i = 0;
      foreach (Face face in Face.faces)
      {
        if (neighbours[i++]) continue;
        Rect thisUv = uvs[0];
        meshData.uvs.AddRange(new[]
        {
          new Vector2(thisUv.x, thisUv.y),
          new Vector2(thisUv.x, thisUv.y + thisUv.height),
          new Vector2(thisUv.x + thisUv.width, thisUv.y + thisUv.height),
          new Vector2(thisUv.x + thisUv.width, thisUv.y),
        });
      }
      return meshData;
    }

    public virtual MeshData GetCollisionVoxel(Vector3 pos, bool[] neighbours, int offset)
    {
      MeshData meshData = new MeshData();
      int i = 0;
      int faceCounter = 0;
      foreach (Face face in Face.faces)
      {
        if (neighbours[i++]) continue;
        foreach (Vector3 vertex in face.vertices)
        {
          meshData.vertices.Add(pos + vertex);
        }
        
        faceCounter++;
      }

      for (int j = 0; j < faceCounter; j++)
      {
        meshData.triangles.Add(offset + j * 4);
        meshData.triangles.Add(offset + j * 4 + 1);
        meshData.triangles.Add(offset + j * 4 + 2); // Tri 1
        meshData.triangles.Add(offset + j * 4);
        meshData.triangles.Add(offset + j * 4 + 2);
        meshData.triangles.Add(offset + j * 4 + 3); // Tri 2
      }

      return meshData;
    }

    public Mesh MakeMesh(MeshData _meshData)
    {
      Mesh mesh = new Mesh();
      mesh.vertices = _meshData.vertices.ToArray();
      mesh.triangles = _meshData.triangles.ToArray();
      mesh.uv = _meshData.uvs.ToArray();
      return mesh;
    }
  }
}