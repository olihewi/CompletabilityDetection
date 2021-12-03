using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    [CreateAssetMenu(fileName = "Invisible", menuName = "Voxels/Invisible")]
    public class Invisible : Voxel
    {
        public override MeshData GetVoxel(Vector3 pos, bool[] neighbours, int offset)
        {
            return new MeshData();
        }
    }
}

