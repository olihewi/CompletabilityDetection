using System;
using UnityEditor;
using UnityEngine;

namespace Voxels
{
  [CustomEditor(typeof(Volume))]
  public class VolumeEditor : Editor
  {
    public Volume volume
    {
      get { return (Volume) target; }
    }

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
    private int selectedVoxel = 0;

    private class Selection
    {
      public Vector3Int tile;
      public Vector3 face;
    }

    private Rect windowRect;

    protected virtual void OnSceneGUI()
    {
      Event e = Event.current;
      Handles.BeginGUI();
      toolMode = (ToolMode) GUI.Toolbar(new Rect(10, 10, 200, 30), (int) toolMode, new[] {"Transform", "Build"});
      if (toolMode == ToolMode.Building)
      {
        buildMode = (BuildMode) GUI.Toolbar(new Rect(10, 50, 200, 30), (int) buildMode, new[] {"Place", "Replace", "Remove"});
        windowRect = new Rect(10, 120, 200, SceneView.lastActiveSceneView.position.height - 140);
        GUI.Window(0, windowRect, PaletteWindow, "Palette");
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
        Building(e);
      }

      UnityEditor.Selection.activeGameObject = volume.gameObject;
    }

    private void Building(Event e)
    {
      buildMode = e.type switch
      {
        EventType.KeyDown when e.keyCode == KeyCode.LeftShift => BuildMode.Remove,
        EventType.KeyUp when e.keyCode == KeyCode.LeftShift => BuildMode.Place,
        EventType.KeyDown when e.keyCode == KeyCode.LeftControl => BuildMode.Replace,
        EventType.KeyUp when e.keyCode == KeyCode.LeftControl => BuildMode.Place,
        _ => buildMode
      };

      Selection selection = GetSelectionAt(e.mousePosition);
      if (selection != null)
      {
        DrawFace(selection, buildMode == BuildMode.Place ? Color.green : buildMode == BuildMode.Replace ? Color.yellow : Color.red);
        EditorGUI.EndChangeCheck();
        if (e.type == EventType.MouseDown && e.button == 0)
        {
          Undo.RecordObject(target, "EditMesh");
          switch (buildMode)
          {
            case BuildMode.Place:
              volume.PlaceVoxel(selection.tile + Vector3Int.FloorToInt(selection.face), volume.palette[selectedVoxel]);
              break;
            case BuildMode.Replace:
              volume.ReplaceVoxel(selection.tile, volume.palette[selectedVoxel]);
              break;
            case BuildMode.Remove:
              volume.RemoveVoxel(selection.tile);
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
      else if (e.type == EventType.MouseDown && e.button == 0) toolMode = ToolMode.Transform;
    }

    private Selection GetSelectionAt(Vector2 _position)
    {
      Ray ray = HandleUtility.GUIPointToWorldRay(_position);
      RaycastHit hit;
      if (!Physics.Raycast(ray, out hit)) return null;
      Volume other = hit.collider.gameObject.GetComponent<Volume>();
      if (other == volume)
      {
        return new Selection
        {
          tile = Vector3Int.FloorToInt(other.transform.InverseTransformPoint(hit.point - hit.normal * 0.5F) + Vector3.one * 0.5F),
          face = other.transform.InverseTransformDirection(hit.normal)
        };
      }

      return null;
    }

    private void DrawFace(Selection face, Color _color, float thickness = 3.0F)
    {
      Vector3 normal = volume.transform.TransformDirection(face.face);
      Vector3 front = volume.transform.TransformPoint(face.tile) + normal * 0.51F;
      Vector3 upDown = Vector3.Cross(normal, face.face.y == 0 ? volume.transform.up : volume.transform.right);
      Vector3 leftRight = Vector3.Cross(upDown, normal);
      Vector3 a = front + (-upDown + leftRight) * 0.5F;
      Vector3 b = front + (upDown + leftRight) * 0.5F;
      Vector3 c = front + (upDown - leftRight) * 0.5F;
      Vector3 d = front + (-upDown - leftRight) * 0.5F;
      Handles.color = _color;
      Handles.DrawLine(a, b, thickness);
      Handles.DrawLine(b, c, thickness);
      Handles.DrawLine(c, d, thickness);
      Handles.DrawLine(d, a, thickness);
    }

    private void PaletteWindow(int id)
    {
      GUIContent[] contents = new GUIContent[volume.palette.Length];
      for (int i = 0; i < contents.Length; i++)
      {
        contents[i] = new GUIContent {tooltip = volume.palette[i].name};
        if (volume.palette[i].textures.Length == 0)
        {
          contents[i].text = volume.palette[i].name;
          continue;
        }
        contents[i].image = volume.palette[i].textures[0];
      }
      GUIStyle style = GUI.skin.button;
      style.fontSize = 10;
      style.wordWrap = true;
      selectedVoxel = GUI.SelectionGrid(new Rect(10, 25, 180, 50 * contents.Length / 4), selectedVoxel, contents, 4, style);
      if (GUI.Button(new Rect(10, windowRect.height - 40, 180, 30), "Reload")) volume.PackTextures();
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
      volume.GenerateMesh();
    }
  }
}