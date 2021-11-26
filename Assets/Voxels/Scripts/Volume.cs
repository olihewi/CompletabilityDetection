using System;
using System.Collections;
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
        public class VoxelDict : SerializableDictionary<Vector3Int, Voxel> {}

        [HideInInspector] public Voxel[] palette;
        [HideInInspector] public VoxelDict voxels = new VoxelDict();

        [SerializeField][HideInInspector] private MeshFilter meshFilter;
        [SerializeField][HideInInspector] private MeshCollider meshCollider;
        [SerializeField] [HideInInspector] private MeshRenderer meshRenderer;
        private void Reset()
        {
            palette = Resources.LoadAll<Voxel>("Types");
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load<Material>("Materials/Atlas");
            PackTextures();
            if (voxels.Count == 0) PlaceVoxel(Vector3Int.zero, palette[0]);
        }
        
        private List<Vector3> v = new List<Vector3>();
        private List<int> t = new List<int>();
        private List<Vector2> u = new List<Vector2>();

        public void GenerateMesh()
        {
            v.Clear();
            t.Clear();
            u.Clear();
            foreach (KeyValuePair<Vector3Int, Voxel> voxel in voxels)
            {
                AddVoxel(voxel);
            }
            Mesh mesh = new Mesh();
            mesh.vertices = v.ToArray();
            mesh.triangles = t.ToArray();
            mesh.uv = u.ToArray();
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
            mesh.RecalculateNormals();
        }

        public void PackTextures()
        {
            List<Texture2D> texturesToPack = new List<Texture2D>();
            foreach (Voxel voxel in palette)
            {
                foreach (Texture2D texture in voxel.textures)
                {
                    if (texturesToPack.Contains(texture)) continue;
                    texturesToPack.Add(texture);
                }
            }
            Texture2D atlas = new Texture2D(1024,1024);
            atlas.filterMode = FilterMode.Point;
            Rect[] rects = atlas.PackTextures(texturesToPack.ToArray(), 0, 1024);
            Dictionary<Texture2D, Rect> lookup = new Dictionary<Texture2D, Rect>();
            for (int i = 0; i < rects.Length; i++)
            {
                lookup.Add(texturesToPack[i],rects[i]);
            }
            foreach (Voxel voxel in palette)
            {
                voxel.uvs = new List<Rect>();
                foreach (Texture2D texture in voxel.textures)
                {
                    voxel.uvs.Add(lookup[texture]);
                }
            }
            meshRenderer.sharedMaterial.mainTexture = atlas;
        }
        
        private void AddVoxel(KeyValuePair<Vector3Int, Voxel> voxel)
        {
            bool[] neighbours = new bool[6];
            foreach (Voxel.Face face in Voxel.Face.faces)
            {
                neighbours[face.index] = voxels.ContainsKey(voxel.Key + face.direction);
            }
            Voxel.MeshData meshData = voxel.Value.GetVoxel(voxel.Key, neighbours, v.Count);
            v.AddRange(meshData.vertices);
            t.AddRange(meshData.triangles);
            u.AddRange(meshData.uvs);
        }

        public void PlaceVoxel(Vector3Int _position, Voxel _voxel)
        {
            voxels.Add(_position, _voxel);
            GenerateMesh();
        }

        public void RemoveVoxel(Vector3Int _position)
        {
            if (voxels.Count <= 1) return;
            voxels.Remove(_position);
            GenerateMesh();
        }

        public void ReplaceVoxel(Vector3Int _position, Voxel _voxel)
        {
            voxels[_position] = _voxel;
            GenerateMesh();
        }

        [MenuItem("GameObject/Voxel Volume",false,10)]
        private static void Create(MenuCommand _menuCommand)
        {
            GameObject go = new GameObject("Voxel Volume");
            Volume volume = go.AddComponent<Volume>();
            GameObjectUtility.SetParentAndAlign(go, _menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
