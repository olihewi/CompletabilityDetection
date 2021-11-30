using System;
using UnityEngine;

namespace Game.Abilities
{
  [Serializable]
  public abstract class PlayerAbility : ScriptableObject
  {
    public abstract void Perform(Player _player);
    //public abstract void Traverse(CompletabilityGrid _grid, VoxelLevel _level, Player _player);
  }
}