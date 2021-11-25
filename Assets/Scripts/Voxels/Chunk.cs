using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        public static Vector3Int chunkSize = new Vector3Int(16, 16, 16);

        public Voxel airVoxel;
        public Voxel blockVoxel;

        private Voxel[,,] voxels = new Voxel[chunkSize.x, chunkSize.y, chunkSize.z];

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;
        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
            FillWithAir();
            for (int x = 0; x < chunkSize.x; x++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    for (int y = 0; y < chunkSize.y; y++)
                    {
                        if (x <= y && z <= y)
                        {
                            voxels[x, y, z] = blockVoxel;
                        }
                    }
                }
            }
            GenerateMesh();
        }

        public void GenerateMesh()
        {
            List<Vector3> v = new List<Vector3>();
            List<int> t = new List<int>();
            List<Vector2> u = new List<Vector2>();
            for (int x = 0; x < chunkSize.x; x++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    for (int y = 0; y < chunkSize.y; y++)
                    {
                        Voxel voxel = voxels[x, y, z];
                        if (voxel.isAir) continue;
                        Vector3 thisPos = new Vector3(x, y, z);
                        int faceCounter = 0;
                        // Top
                        if (y == chunkSize.y - 1 || voxels[x, y + 1, z].isAir)
                        {
                            v.Add(thisPos + new Vector3(0, 1, 0));
                            v.Add(thisPos + new Vector3(0, 1, 1));
                            v.Add(thisPos + new Vector3(1, 1, 1));
                            v.Add(thisPos + new Vector3(1, 1, 0));
                            faceCounter++;
                        }

                        // Bottom
                        if (y == 0 || voxels[x, y - 1, z].isAir)
                        {
                            v.Add(thisPos + new Vector3(0, 0, 0));
                            v.Add(thisPos + new Vector3(1, 0, 0));
                            v.Add(thisPos + new Vector3(1, 0, 1));
                            v.Add(thisPos + new Vector3(0, 0, 1));
                            faceCounter++;
                        }

                        // Front
                        if (z == 0 || voxels[x, y, z - 1].isAir)
                        {
                            v.Add(thisPos + new Vector3(0, 0, 0));
                            v.Add(thisPos + new Vector3(0, 1, 0));
                            v.Add(thisPos + new Vector3(1, 1, 0));
                            v.Add(thisPos + new Vector3(1, 0, 0));
                            faceCounter++;
                        }

                        // Back
                        if (z == chunkSize.z - 1 || voxels[x, y, z + 1].isAir)
                        {
                            v.Add(thisPos + new Vector3(1, 0, 1));
                            v.Add(thisPos + new Vector3(1, 1, 1));
                            v.Add(thisPos + new Vector3(0, 1, 1));
                            v.Add(thisPos + new Vector3(0, 0, 1));
                            faceCounter++;
                        }

                        // Left
                        if (x == 0 || voxels[x - 1, y, z].isAir)
                        {
                            v.Add(thisPos + new Vector3(0, 0, 1));
                            v.Add(thisPos + new Vector3(0, 1, 1));
                            v.Add(thisPos + new Vector3(0, 1, 0));
                            v.Add(thisPos + new Vector3(0, 0, 0));
                            faceCounter++;
                        }

                        // Right
                        if (x == chunkSize.x - 1 || voxels[x + 1, y, z].isAir)
                        {
                            v.Add(thisPos + new Vector3(1, 0, 0));
                            v.Add(thisPos + new Vector3(1, 1, 0));
                            v.Add(thisPos + new Vector3(1, 1, 1));
                            v.Add(thisPos + new Vector3(1, 0, 1));
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
                }
            }
            mesh.vertices = v.ToArray();
            mesh.triangles = t.ToArray();
            mesh.uv = u.ToArray();
        }

        public void FillWithAir()
        {
            for (int x = 0; x < chunkSize.x; x++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    for (int y = 0; y < chunkSize.y; y++)
                    {
                        voxels[x, y, z] = airVoxel;
                    }
                }
            }
        }
    }
}

