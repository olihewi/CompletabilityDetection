using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  public class Interact : PlayerAbility
  {
    public InputAction interact;

    private void OnEnable()
    {
      interact.Enable();
    }

    private void OnDisable()
    {
      interact.Disable();
    }

    private Player player;
    public override void Perform(Player _player)
    {
      player = _player;
    }

    private void OnTriggerStay(Collider other)
    {
      if (!other.CompareTag("Interactible")) return;
      Debug.Log("Hello");
      if (interact.triggered)
      {
        foreach (Interactable interactable in other.GetComponents<Interactable>())
        {
          interactable.Interact(player);
        }
      }
    }
    public override void Traverse(Dictionary<Vector3Int, float> _completabilityGrid, Voxels.Volume _volume, Player _player)
    {
      return;
    }
  }
}

