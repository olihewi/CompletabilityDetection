using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Voxels;

namespace Completability
{
    public class LevelVolume : Volume
    {
        [MenuItem("GameObject/Level Volume", false, 10)]
        private static void Create(MenuCommand _menuCommand)
        {
            GameObject go = new GameObject("Level Volume");
            Volume volume = go.AddComponent<Volume>();
            GameObjectUtility.SetParentAndAlign(go, _menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }

}
