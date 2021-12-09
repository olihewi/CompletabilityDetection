using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [Serializable]
  public class Looking : PlayerAbility
  {
    public InputAction look;
    public float distance = 3.0F;
    public float lookSpeed = 2.0F;

    private void OnEnable()
    {
      look.Enable();
    }

    private void OnDisable()
    {
      look.Disable();
    }

    public override void Perform(Player _player)
    {
      Vector2 lookInput = look.ReadValue<Vector2>() * distance;
      _player.cameraPivot.localPosition = Vector3.Lerp(_player.cameraPivot.localPosition, new Vector3(lookInput.x, _player.cameraPivot.localPosition.y, lookInput.y), lookSpeed * distance * Time.deltaTime);
    }
    public override void Traverse(Dictionary<Vector3Int, float> _completabilityGrid, Voxels.Volume _volume, Player _player)
    {
      return;
    }
  }
}