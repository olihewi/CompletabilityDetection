using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    public class Volume : MonoBehaviour
    {
        private Dictionary<Vector3Int,Chunk> chunks = new Dictionary<Vector3Int,Chunk>();
        public Voxel airVoxel;
        public Voxel blockVoxel;

        void Start()
        {
            AddChunkAtPos(new Vector3Int(0, 0, 0));
            AddChunkAtPos(new Vector3Int(1, 0, 0));
        }

        private void AddChunkAtPos(Vector3Int pos)
        {
            GameObject go = new GameObject(pos.ToString());
            go.transform.position = pos * Chunk.chunkSize;
            go.transform.parent = transform;
            Chunk c = go.AddComponent(typeof(Chunk)) as Chunk;
            c.airVoxel = airVoxel;
            c.blockVoxel = blockVoxel;
            chunks.Add(pos, c);
        }
    }
}
