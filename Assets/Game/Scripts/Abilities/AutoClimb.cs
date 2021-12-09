using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
  [Serializable]
  public class AutoClimb : PlayerAbility
  {
    public LayerMask layerMask;
    public float height = 1.0F;
    public override void Perform(Player _player)
    {
      if (!_player.controller.isGrounded) return;
      Vector3 start = _player.transform.position - new Vector3(0.0F, _player.controller.height / 2.0F - 0.5F, 0.0F);
      Vector3 direction = new Vector3(_player.velocity.x, 0.0F, _player.velocity.z);
      float maxDistance = _player.controller.radius - 0.45F + direction.magnitude / 8.0F;
      if (Physics.Raycast(start, direction, maxDistance, layerMask) &&
          !Physics.Raycast(start + new Vector3(0.0F, height, 0.0F), direction, maxDistance, layerMask))
      {
        _player.velocity.Normalize();
        _player.velocity = new Vector3(_player.velocity.x, Mathf.Sqrt(-2.0F * -9.81F * (height + 0.1F)), _player.velocity.z);
      }
    }
    public override void Traverse(Dictionary<Vector3Int, float> _completabilityGrid, Voxels.Volume _volume, Player _player)
    {
      return;
    }
  }
}

