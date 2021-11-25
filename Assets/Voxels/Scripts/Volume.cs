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

        public Voxel block;
        [HideInInspector] public VoxelDict voxels = new VoxelDict();

        [SerializeField][HideInInspector] private MeshFilter meshFilter;
        [SerializeField][HideInInspector] private MeshCollider meshCollider;
        private void Reset()
        {
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
            if (voxels.Count == 0) PlaceVoxel(Vector3Int.zero);
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

        public struct Face
        {
            public Face(Vector3Int _direction, Vector3[] _vertices)
            {
                direction = _direction;
                vertices = _vertices;
            }
            public Vector3Int direction;
            public Vector3[] vertices;
            public static Face[] faces = 
            {
                new Face(Vector3Int.up, new [] { new Vector3(-0.5F, 0.5F, -0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.5F, -0.5F) }),
                new Face(Vector3Int.down, new [] { new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F) }),
                new Face(Vector3Int.forward, new [] { new Vector3(0.5F, -0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F) }),
                new Face(Vector3Int.back, new [] { new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(-0.5F, 0.5F, -0.5F), new Vector3(0.5F, 0.5F, -0.5F), new Vector3(0.5F, -0.5F, -0.5F) }),
                new Face(Vector3Int.left, new [] { new Vector3(-0.5F, -0.5F, 0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(-0.5F, 0.5F, -0.5F), new Vector3(-0.5F, -0.5F, -0.5F) }),
                new Face(Vector3Int.right, new [] { new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, -0.5F, 0.5F) })
            };
        }
        private void AddVoxel(KeyValuePair<Vector3Int, Voxel> voxel)
        {
            Vector3 thisPos = voxel.Key;
            int faceCounter = 0;
            // Vertices
            foreach (Face face in Face.faces)
            {
                if (voxels.ContainsKey(voxel.Key + face.direction)) continue;
                foreach (Vector3 vertex in face.vertices)
                {
                    v.Add(thisPos + vertex);
                }
                faceCounter++;
            }
            // Triangles
            int vertCountOffset = v.Count - 4 * faceCounter; // Gets this block's vertices' offset from the start of the list
            for (int i = 0; i < faceCounter; i++)
            {
                t.Add(vertCountOffset + i * 4);
                t.Add(vertCountOffset + i * 4 + 1);
                t.Add(vertCountOffset + i * 4 + 2); // Tri 1
                t.Add(vertCountOffset + i * 4);
                t.Add(vertCountOffset + i * 4 + 2);
                t.Add(vertCountOffset + i * 4 + 3); // Tri 2
            }
        }

        public void PlaceVoxel(Vector3Int _position)
        {
            voxels.Add(_position,block);
            GenerateMesh();
        }

        public void RemoveVoxel(Vector3Int _position)
        {
            if (voxels.Count <= 1) return;
            voxels.Remove(_position);
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
