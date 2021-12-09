using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [Serializable]
  public class AutoJump : PlayerAbility
  {
    public float height = 1.0F;
    private bool wasGrounded = false;
    public override void Perform(Player _player)
    {
      bool isGrounded = _player.controller.isGrounded;
      Vector2 horizontalVelocity = new Vector2(_player.velocity.x,_player.velocity.z);
      if (!isGrounded && wasGrounded && horizontalVelocity.magnitude > 3.0F)
      {
        _player.velocity.y = Mathf.Sqrt(-2.0F * -9.81F * height);
      }
      wasGrounded = isGrounded;
    }
    public override void Traverse(Dictionary<Vector3Int, float> _completabilityGrid, Voxels.Volume _volume, Player _player)
    {
      Dictionary<Vector3Int, float> toAdd = new Dictionary<Vector3Int, float>();
      Vector3Int[] directions =
      { new Vector3Int(1,1,0), new Vector3Int(-1,1,0), new Vector3Int(0,1,1), new Vector3Int(0,1,-1),
        //new Vector3Int(1,0,1), new Vector3Int(1,0,-1), new Vector3Int(-1,0,-1), new Vector3Int(-1,0,1)
      };
      foreach (KeyValuePair<Vector3Int, float> tile in _completabilityGrid)
      {
        foreach (Vector3Int direction in directions)
        {
          Vector3Int voxel = tile.Key + direction;
          if (_volume.voxels.ContainsKey(voxel) && !_completabilityGrid.ContainsKey(voxel) && !toAdd.ContainsKey(voxel))
          {
            bool shoudAdd = true;
            for (int i = 1; i < _player.controller.height + 1.0F; i++)
            {
              if (_volume.voxels.ContainsKey(voxel + Vector3Int.up * i)) shoudAdd = false;
            }
            if (!shoudAdd) continue;
            toAdd.Add(voxel, tile.Value + 0.5F);
          }
        }
      }
      foreach (KeyValuePair<Vector3Int, float> i in toAdd)
      {
        _completabilityGrid.Add(i.Key, i.Value);
      }
    }
  }
}
