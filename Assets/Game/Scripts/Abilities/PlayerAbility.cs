using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
  public abstract class PlayerAbility : MonoBehaviour
  {
    public abstract void Perform(Player _player);
    public abstract void Traverse(Dictionary<Vector3Int, float> _grid, Voxels.Volume _volume);
  }
}