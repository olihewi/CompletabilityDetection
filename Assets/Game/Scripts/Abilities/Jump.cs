using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [Serializable]
  public class Jump : PlayerAbility
  {
    public InputAction jumpAction;
    public float height = 1.0F;

    private void OnEnable()
    {
      jumpAction.Enable();
    }

    private void OnDisable()
    {
      jumpAction.Disable();
    }

    public override void Perform(Player _player)
    {
      if (_player.controller.isGrounded && jumpAction.triggered)
        _player.velocity.y = Mathf.Sqrt(-2.0F * -9.81F * height);
    }
    public override void Traverse(Dictionary<Vector3Int, CompletabilityData> _completabilityGrid, Voxels.Volume _volume, Player _player)
    {
      // Handled by Movement
    }
  }
}
