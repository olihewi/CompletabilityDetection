using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voxels;

namespace Completability
{
    [CustomEditor(typeof(LevelVolume))]
    public class LevelVolumeEditor : VolumeEditor
    {
        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }
    }
}

