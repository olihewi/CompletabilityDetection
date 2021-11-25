using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Voxels
{
    [CustomEditor(typeof(Volume))]
    public class VolumeEditor : Editor
    {
        public enum ToolMode
        {
            Transform,
            Building
        }
        private ToolMode toolMode = ToolMode.Building;

        public enum BuildMode
        {
            Place,
            Replace,
            Remove
        }
        private BuildMode buildMode = BuildMode.Place;

        private class Selection
        {
            public Vector3Int tile;
            public Vector3 face;
        }

        private void OnSceneGUI()
        {
            Event e = Event.current;
            Handles.BeginGUI();
            toolMode = (ToolMode) GUI.Toolbar(new Rect(10, 10, 200, 30), (int) toolMode, new[] {"Transform", "Build"});
            if (toolMode == ToolMode.Building)
            {
                buildMode = (BuildMode) GUI.Toolbar(new Rect(10, 50, 200, 30), (int) buildMode, new[] {"Place", "Replace", "Remove"});
                GUI.Window(0, new Rect(10, 120, 200, 400), PaletteWindow, "Pallete");
            }
            Handles.EndGUI();

            if (toolMode == ToolMode.Transform)
            {
                if (Tools.current == Tool.None) Tools.current = Tool.Move;
                return;
            }

            Tools.current = Tool.None;

            if (toolMode == ToolMode.Building)
            {
                Selection selection = GetSelectionAt(e.mousePosition);
                if (selection != null)
                {
                    DrawFace(selection, buildMode == BuildMode.Place ? Color.green : buildMode == BuildMode.Replace ? Color.yellow : Color.red);
                    EditorGUI.EndChangeCheck();
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        Undo.RecordObject(target,"EditMesh");
                        switch (buildMode)
                        {
                            case BuildMode.Place:
                                ((Volume) target).PlaceVoxel(selection.tile + selection.face.Floor());
                                SceneView.lastActiveSceneView.camera.transform.position += selection.face;
                                break;
                            case BuildMode.Replace:
                                break;
                            case BuildMode.Remove:
                                ((Volume) target).RemoveVoxel(selection.tile);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        EditorUtility.SetDirty(target);
                    }
                }
            }
            UnityEditor.Selection.activeGameObject = ((Volume) target).gameObject;
        }

        private Selection GetSelectionAt(Vector2 _position)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(_position);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit)) return null;
            Volume other = hit.collider.gameObject.GetComponent<Volume>();
            if (other == (Volume) target)
            {
                return new Selection()
                {
                    tile = (other.transform.InverseTransformPoint(hit.point - hit.normal * 0.5F) + Vector3.one * 0.5F).Floor(),
                    face = other.transform.InverseTransformDirection(hit.normal)
                };
            }
            return null;
        }

        private void DrawFace(Selection face, Color _color, float thickness = 3.0F)
        {
            Volume volume = (Volume) target;
            Vector3 normal = volume.transform.TransformDirection(face.face);
            Vector3 front = volume.transform.TransformPoint(face.tile.Float()) + normal * 0.51F;
            Vector3 upDown = Vector3.Cross(normal, face.face.y == 0 ? volume.transform.up : volume.transform.right);
            Vector3 leftRight = Vector3.Cross(upDown, normal);
            Vector3 a = front + (-upDown + leftRight) * 0.5F;
            Vector3 b = front + (upDown + leftRight) * 0.5F;
            Vector3 c = front + (upDown - leftRight) * 0.5F;
            Vector3 d = front + (-upDown - leftRight) * 0.5F;
            Handles.color = _color;
            Handles.DrawLine(a,b,thickness);
            Handles.DrawLine(b,c,thickness);
            Handles.DrawLine(c,d,thickness);
            Handles.DrawLine(d,a,thickness);
        }

        private void PaletteWindow(int id)
        {
            
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }
        private void OnUndoRedo()
        {
            ((Volume) target).GenerateMesh();
        }
    }
}
