using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    [CreateAssetMenu(fileName = "New Voxel Type", menuName = "Voxels/Type")]
    public class Voxel : ScriptableObject
    {
        public bool isAir = false;
    }
}
