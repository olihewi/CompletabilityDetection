using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Voxels
{
  [RequireComponent(typeof(MeshFilter))]
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshCollider))]
  public class Volume : MonoBehaviour
  {
    [Serializable]
    public class VoxelDict : SerializableDictionary<Vector3Int, Voxel>
    {
    }

    [HideInInspector] public Voxel[] palette;
    [HideInInspector] public VoxelDict voxels = new VoxelDict();

    [SerializeField] [HideInInspector] private MeshFilter meshFilter;
    [SerializeField] [HideInInspector] private MeshCollider meshCollider;
    [SerializeField] [HideInInspector] private MeshRenderer meshRenderer;

    public static List<Volume> VOLUMES = new List<Volume>();

    private void Reset()
    {
      meshCollider = GetComponent<MeshCollider>();
      meshFilter = GetComponent<MeshFilter>();
      meshRenderer = GetComponent<MeshRenderer>();
      meshRenderer.material = new Material(Shader.Find("Ciberman/Texel Space Shading"));
      //meshRenderer.material = Resources.Load<Material>("Materials/Atlas");
      PackTextures();
      if (voxels.Count == 0) PlaceVoxel(Vector3Int.zero, palette[0]);
    }

    private void OnEnable()
    {
      VOLUMES.Add(this);
    }

    private void OnDisable()
    {
      VOLUMES.Remove(this);
    }

    private List<Vector3> v = new List<Vector3>();
    private List<int> t = new List<int>();
    private List<Vector2> u = new List<Vector2>();
    private List<Vector3> v_m = new List<Vector3>();
    private List<int> t_m = new List<int>();
    private static readonly int BASE_MAP = Shader.PropertyToID("_BaseMap");
    private static readonly int BASE_COLOR = Shader.PropertyToID("_BaseColor");

    [ContextMenu("Generate Mesh")]
    public void GenerateMesh()
    {
      v.Clear();
      t.Clear();
      u.Clear();
      v_m.Clear();
      t_m.Clear();
      foreach (KeyValuePair<Vector3Int, Voxel> voxel in voxels)
      {
        AddVoxel(voxel);
      }

      Mesh mesh = new Mesh();
      mesh.vertices = v.ToArray();
      mesh.triangles = t.ToArray();
      mesh.uv = u.ToArray();
      mesh.RecalculateNormals();
      Mesh collisionMesh = new Mesh();
      collisionMesh.vertices = v_m.ToArray();
      collisionMesh.triangles = t_m.ToArray();
      collisionMesh.RecalculateNormals();
      meshFilter.mesh = mesh;
      meshCollider.sharedMesh = collisionMesh;
    }

    [ContextMenu("Pack Textures")]
    public void PackTextures()
    {
      palette = Resources.LoadAll<Voxel>("Types");
      List<Texture2D> texturesToPack = new List<Texture2D>();
      foreach (Voxel voxel in palette)
      {
        foreach (Texture2D texture in voxel.textures)
        {
          if (texturesToPack.Contains(texture)) continue;
          texturesToPack.Add(texture);
        }
      }

      Texture2D atlas = new Texture2D(1024, 1024);
      atlas.filterMode = FilterMode.Point;
      Rect[] rects = atlas.PackTextures(texturesToPack.ToArray(), 0, 1024);
      Dictionary<Texture2D, Rect> lookup = new Dictionary<Texture2D, Rect>();
      for (int i = 0; i < rects.Length; i++)
      {
        lookup.Add(texturesToPack[i], rects[i]);
      }

      foreach (Voxel voxel in palette)
      {
        voxel.uvs = new List<Rect>();
        foreach (Texture2D texture in voxel.textures)
        {
          voxel.uvs.Add(lookup[texture]);
        }
      }

      meshRenderer.sharedMaterial.SetTexture(BASE_MAP,atlas);
      meshRenderer.sharedMaterial.SetColor(BASE_COLOR,Color.white);
    }

    private void AddVoxel(KeyValuePair<Vector3Int, Voxel> voxel)
    {
      bool[] neighbours = new bool[6];
      foreach (Voxel.Face face in Voxel.Face.faces)
      {
        neighbours[face.index] = voxels.ContainsKey(voxel.Key + face.direction) && voxels[voxel.Key + face.direction].textures.Length > 0;
      }

      Voxel.MeshData meshData = voxel.Value.GetVoxel(voxel.Key, neighbours, v.Count);
      v.AddRange(meshData.vertices);
      t.AddRange(meshData.triangles);
      u.AddRange(meshData.uvs);
      Voxel.MeshData collisionMeshData = voxel.Value.GetCollisionVoxel(voxel.Key, neighbours, v_m.Count);
      v_m.AddRange(collisionMeshData.vertices);
      t_m.AddRange(collisionMeshData.triangles);
    }

    public void PlaceVoxel(Vector3Int _position, Voxel _voxel)
    {
      voxels.Add(_position, _voxel);
      if (_voxel.hasGameObject)
      {
        GameObject go = Instantiate(_voxel.gameObject, _position, Quaternion.identity, transform);
        go.name = _voxel.name + " [" + _position + "]";
      }
      GenerateMesh();
    }

    public void RemoveVoxel(Vector3Int _position)
    {
      if (voxels.Count <= 1) return;
      if (voxels[_position].hasGameObject)
      {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
          Transform child = transform.GetChild(i);
          if (Vector3Int.FloorToInt(child.position) == _position)
          {
            DestroyImmediate(child.gameObject);
          }
        }
      }
      voxels.Remove(_position);
      GenerateMesh();
    }

    public void ReplaceVoxel(Vector3Int _position, Voxel _voxel)
    {
      voxels[_position] = _voxel;
      GenerateMesh();
    }

    [MenuItem("GameObject/Voxel Volume", false, 10)]
    private static void Create(MenuCommand _menuCommand)
    {
      GameObject go = new GameObject("Voxel Volume");
      Volume volume = go.AddComponent<Volume>();
      GameObjectUtility.SetParentAndAlign(go, _menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
      Selection.activeObject = go;
    }

    public class VoxelCastHit
    {
      public bool hit = false;
      public Voxel voxel = null;
      public Vector3Int position = Vector3Int.zero;
      public Vector3 face = Vector3.zero;
      public Vector3 normal = Vector3.zero;
    }

    public VoxelCastHit VoxelCast(Vector3 start, Vector3 direction, float maxDistance)
    {
      RaycastHit rayHit;
      if (!Physics.Raycast(start, direction, out rayHit, maxDistance, 1 << gameObject.layer)) return new VoxelCastHit();
      VoxelCastHit hit = new VoxelCastHit();
      hit.hit = true;
      hit.normal = rayHit.normal;
      hit.face = transform.InverseTransformDirection(rayHit.normal);
      hit.position = Vector3Int.FloorToInt((transform.InverseTransformPoint(rayHit.point) + Vector3.one * 0.5F) - hit.face * 0.1F);
      hit.voxel = voxels[hit.position];
      return hit;
    }
    public static VoxelCastHit VoxelCastAll(Vector3 start, Vector3 direction, float maxDistance)
    {
      foreach (Volume volume in VOLUMES)
      {
        VoxelCastHit hit = volume.VoxelCast(start, direction, maxDistance);
        if (hit.hit) return hit;
      }
      return new VoxelCastHit();
    }
  }
}