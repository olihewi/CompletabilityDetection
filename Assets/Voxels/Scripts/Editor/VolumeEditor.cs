using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
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
      Building,
      Completability
    }

    private ToolMode toolMode = ToolMode.Building;
    private ToolMode lastToolMode = ToolMode.Building;

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

    private Vector3 V3IntToV3(Vector3Int _v)
    {
      return new Vector3(_v.x,_v.y,_v.z);
    }

    private Rect paletteWindowRect;
    private Rect completabilityWindowRect;

    protected virtual void OnSceneGUI()
    {
      Event e = Event.current;
      Handles.BeginGUI();
      lastToolMode = toolMode;
      toolMode = (ToolMode) GUI.Toolbar(new Rect(10, 10, 300, 30), (int) toolMode, new[] {"Transform", "Build", "Completability"});
      if (lastToolMode != toolMode && toolMode == ToolMode.Completability)
        GenerateCompletabilityGrid();
      Rect windowSize = SceneView.lastActiveSceneView.position;
      if (toolMode == ToolMode.Building)
      {
        buildMode = (BuildMode) GUI.Toolbar(new Rect(10, 50, 200, 30), (int) buildMode, new[] {"Place", "Replace", "Remove"});
        paletteWindowRect = new Rect(10, 120, 200, SceneView.lastActiveSceneView.position.height - 140);
        GUI.Window(0, paletteWindowRect, PaletteWindow, "Palette");
      }
      else if (toolMode == ToolMode.Completability)
      {
        completabilityWindowRect = new Rect(10, 75, 200, 35.0F + activeAbilities.Count * 15.0F);
        GUI.Window(1, completabilityWindowRect, CompletabilityWindow, "Level Completability");
      }
      

      Handles.EndGUI();

      if (toolMode == ToolMode.Transform)
      {
        if (Tools.current == Tool.None) Tools.current = Tool.Move;
        return;
      }

      Tools.current = Tool.None;

      if (toolMode == ToolMode.Completability)
      {
        DrawCompletabilityGrid(e);
      }
      else if (toolMode == ToolMode.Building)
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

    private void DrawPath(Vector3Int start)
    {
      Vector3Int pos = start;
      Handles.color = Color.magenta;
      while (completabilityGrid.ContainsKey(pos) && pos != completabilityStart && completabilityGrid[pos].from != pos)
      {
        Handles.DrawLine(pos + Vector3.up * 0.6F, completabilityGrid[pos].from + Vector3.up * 0.6F, 3.0F);
        pos = completabilityGrid[pos].from;
      }GUIStyle style = new GUIStyle();
      style.normal.textColor = Color.black;
      Handles.Label(start + Vector3.up * 2.0F,completabilityGrid.ContainsKey(start) ? completabilityGrid[start].time.ToString("F1") + "s" : "Unreachable", style);
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
      if (GUI.Button(new Rect(10, paletteWindowRect.height - 40, 180, 30), "Reload")) volume.PackTextures();
    }

    private void CompletabilityWindow(int id)
    {
      int i = 0;
      foreach (Game.Player.AbilityInstance ability in activeAbilities)
      {
        bool newEnabled = GUI.Toggle(new Rect(20, 20+ i * 15, completabilityWindowRect.width - 30, 15), ability.enabled, ability.ability.GetType().Name);
        if (newEnabled != ability.enabled) GenerateCompletabilityGrid();
        ability.enabled = newEnabled;
        i++;
      }
    }
    
    // LEVEL COMPLETABILITY

    private Dictionary<Vector3Int, CompletabilityData> completabilityGrid = new Dictionary<Vector3Int, CompletabilityData>();
    private Vector3Int completabilityStart;
    private void GenerateCompletabilityGrid()
    {
      EditorCoroutineUtility.StartCoroutine(CompletabilityGeneration(), this);
    }
    private Game.Player player;
    private List<Game.Player.AbilityInstance> activeAbilities = new List<Game.Player.AbilityInstance>();
    private IEnumerator CompletabilityGeneration()
    {
      completabilityGrid.Clear();
      completabilityGrid.Add(completabilityStart, new CompletabilityData{time = 0.0F, from = completabilityStart});
      int lastCount = 0;
      player = GameObject.FindObjectOfType<Game.Player>();
      activeAbilities = player.abilities;
      double lastWaitTime = EditorApplication.timeSinceStartup;
      double oneFrame = 1.0D / 60.0D;
      while (completabilityGrid.Count > lastCount)
      {
        lastCount = completabilityGrid.Count;
        foreach (Game.Player.AbilityInstance ability in activeAbilities)
        {
          if (!ability.enabled) continue;
          ability.ability.Traverse(completabilityGrid, volume, player);
          if (EditorApplication.timeSinceStartup - lastWaitTime > oneFrame)
          {
            lastWaitTime = EditorApplication.timeSinceStartup;
            yield return null;
          }
        }
      }
    }
    private void DrawCompletabilityGrid(Event e)
    {
      Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
      foreach (KeyValuePair<Vector3Int,CompletabilityData> tile in completabilityGrid)
      {
        if (!volume.voxels.ContainsKey(tile.Key)) continue;
        Vector3[] verts =
        {
          volume.transform.TransformPoint(new Vector3(-0.5F,0.5F,-0.5F) + tile.Key),
          volume.transform.TransformPoint(new Vector3(-0.5F,0.5F,+0.5F) + tile.Key),
          volume.transform.TransformPoint(new Vector3(+0.5F,0.5F,+0.5F) + tile.Key),
          volume.transform.TransformPoint(new Vector3(+0.5F,0.5F,-0.5F) + tile.Key),
        };
        Handles.DrawSolidRectangleWithOutline(verts, new Color(0.0F, 1.0F, 0.0F, 0.5F/*0.75F - tile.Value / 7.5F*/), Color.clear);
      }
      DrawFace(new Selection{tile = completabilityStart, face = Vector3.up}, Color.magenta);
      Selection selection = GetSelectionAt(e.mousePosition);
      if (selection == null) return;
      bool isReachable = completabilityGrid.ContainsKey(selection.tile);
      DrawFace(selection,isReachable ? Color.magenta : Color.red);
      if (isReachable)
      {
        DrawPath(selection.tile);
      }

      if (e.type == EventType.MouseDown && e.button == 0)
      {
        completabilityStart = selection.tile;
        GenerateCompletabilityGrid();
      }
      EditorGUI.EndChangeCheck();
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