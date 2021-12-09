using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    [CreateAssetMenu(fileName = "Stairs", menuName = "Voxels/Stairs")]
    public class Stairs : Voxel
    {
        public static Face[] collisionFaces =
        {
            new Face(Vector3Int.up, new[] {new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, -0.5F, -0.5F)}, -1),
            new Face(Vector3Int.down, new[] {new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.5F, -0.5F)}, 1),
            new Face(Vector3Int.forward, new[] {new Vector3(0.5F, -0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F)}, 2),
            new Face(Vector3Int.left, new[] {new Vector3(-0.5F, -0.5F, 0.5F), new Vector3(-0.5F, 0.5F, -0.5F), new Vector3(-0.5F, -0.5F, -0.5F)}, 4),
            new Face(Vector3Int.right, new[] {new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, -0.5F, 0.5F)}, 5)
        };
        public static Face[] voxelFaces =
        {
            new Face(Vector3Int.up, new[] {new Vector3(-0.5F, 0.5F, 0.25F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.25F)}, 0),
            new Face(Vector3Int.up, new[] {new Vector3(-0.5F, 0.25F, 0.0F), new Vector3(-0.5F, 0.25F, 0.25F), new Vector3(0.5F, 0.25F, 0.25F), new Vector3(0.5F, 0.25F, 0.0F)}, -1),
            new Face(Vector3Int.up, new[] {new Vector3(-0.5F, 0.0F, -0.25F), new Vector3(-0.5F, 0.0F, 0.0F), new Vector3(0.5F, 0.0F, 0.0F), new Vector3(0.5F, 0.0F, -0.25F)}, -1),
            new Face(Vector3Int.up, new[] {new Vector3(-0.5F, -0.25F, -0.5F), new Vector3(-0.5F, -0.25F, -0.25F), new Vector3(0.5F, -0.25F, -0.25F), new Vector3(0.5F, -0.25F, -0.5F)}, -1),
            new Face(Vector3Int.down, new[] {new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.5F, -0.5F)}, 1),
            new Face(Vector3Int.forward, new[] {new Vector3(0.5F, -0.5F, 0.5F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(-0.5F, -0.5F, 0.5F)}, 2),
            new Face(Vector3Int.back, new[] {new Vector3(-0.5F, -0.5F, -0.5F), new Vector3(-0.5F, -0.25F, -0.5F), new Vector3(0.5F, -0.25F, -0.5F), new Vector3(0.5F, -0.5F, -0.5F)}, 3),
            new Face(Vector3Int.back, new[] {new Vector3(-0.5F, -0.25F, -0.25F), new Vector3(-0.5F, 0.0F, -0.25F), new Vector3(0.5F, 0.0F, -0.25F), new Vector3(0.5F, -0.25F, -0.25F)}, -1),
            new Face(Vector3Int.back, new[] {new Vector3(-0.5F, 0.0F, 0.0F), new Vector3(-0.5F, 0.25F, 0.0F), new Vector3(0.5F, 0.25F, 0.0F), new Vector3(0.5F, 0.0F, 0.0F)}, -1),
            new Face(Vector3Int.back, new[] {new Vector3(-0.5F, 0.25F, 0.25F), new Vector3(-0.5F, 0.5F, 0.25F), new Vector3(0.5F, 0.5F, 0.25F), new Vector3(0.5F, 0.25F, 0.25F)}, -1),
            new Face(Vector3Int.left, new[] {new Vector3(-0.5F, -0.5F, 0.5F), new Vector3(-0.5F, -0.25F, 0.5F), new Vector3(-0.5F, -0.25F, -0.5F), new Vector3(-0.5F, -0.5F, -0.5F)}, 4),
            new Face(Vector3Int.left, new[] {new Vector3(-0.5F, -0.25F, 0.5F), new Vector3(-0.5F, 0.0F, 0.5F), new Vector3(-0.5F, 0.0F, -0.25F), new Vector3(-0.5F, -0.25F, -0.25F)}, -1),
            new Face(Vector3Int.left, new[] {new Vector3(-0.5F, 0.0F, 0.5F), new Vector3(-0.5F, 0.25F, 0.5F), new Vector3(-0.5F, 0.25F, 0.0F), new Vector3(-0.5F, 0.0F, 0.0F)}, -1),
            new Face(Vector3Int.left, new[] {new Vector3(-0.5F, 0.25F, 0.5F), new Vector3(-0.5F, 0.5F, 0.5F), new Vector3(-0.5F, 0.5F, 0.25F), new Vector3(-0.5F, 0.25F, 0.25F)}, -1),
            new Face(Vector3Int.right, new[] {new Vector3(0.5F, -0.5F, -0.5F), new Vector3(0.5F, -0.25F, -0.5F), new Vector3(0.5F, -0.25F, 0.5F), new Vector3(0.5F, -0.5F, 0.5F)}, 5),
            new Face(Vector3Int.right, new[] {new Vector3(0.5F, -0.25F, -0.25F), new Vector3(0.5F, 0.0F, -0.25F), new Vector3(0.5F, 0.0F, 0.5F), new Vector3(0.5F, -0.25F, 0.5F)}, -1),
            new Face(Vector3Int.right, new[] {new Vector3(0.5F, 0.0F, 0.0F), new Vector3(0.5F, 0.25F, 0.0F), new Vector3(0.5F, 0.25F, 0.5F), new Vector3(0.5F, 0.0F, 0.5F)}, -1),
            new Face(Vector3Int.right, new[] {new Vector3(0.5F, 0.25F, 0.25F), new Vector3(0.5F, 0.5F, 0.25F), new Vector3(0.5F, 0.5F, 0.5F), new Vector3(0.5F, 0.25F, 0.5F)}, -1),
        };
        /*public override MeshData GetCollisionVoxel(Vector3 pos, bool[] neighbours, int offset)
        {
            MeshData meshData = new MeshData();
            int faceCounter = 0;
            foreach (Face face in collisionFaces)
            {
                if (face.index < 6 && face.index >= 0 && neighbours[face.index]) continue;
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
                if (j < 3)
                {
                    meshData.triangles.Add(offset + j * 4);
                    meshData.triangles.Add(offset + j * 4 + 2);
                    meshData.triangles.Add(offset + j * 4 + 3); // Tri 2
                }
            }
            return meshData;
        }
        public override MeshData GetVoxel(Vector3 pos, bool[] neighbours, int offset)
        {
            MeshData meshData = new MeshData();
            int faceCounter = 0;
            foreach (Face face in voxelFaces)
            {
                if (face.index < 6 && face.index >= 0 && neighbours[face.index]) continue;
                foreach (Vector3 vertex in face.vertices)
                {
                    meshData.vertices.Add(pos + vertex);
                }
                Rect thisUv = uvs[0];
                meshData.uvs.AddRange(new[]
                {
                    new Vector2(thisUv.x, thisUv.y),
                    new Vector2(thisUv.x, thisUv.y + thisUv.height),
                    new Vector2(thisUv.x + thisUv.width, thisUv.y + thisUv.height),
                    new Vector2(thisUv.x + thisUv.width, thisUv.y),
                });
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
        }*/
    }
}
