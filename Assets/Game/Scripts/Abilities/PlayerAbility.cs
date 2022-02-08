using System;
using System.Collections.Generic;
using UnityEngine;

public class CompletabilityData
{
  public float time;
  public Vector3Int from;
}

namespace Game.Abilities
{
  public abstract class PlayerAbility : MonoBehaviour
  {
    public abstract void Perform(Player _player);
    public abstract void Traverse(Dictionary<Vector3Int, CompletabilityData> _completabilityGrid, Voxels.Volume _volume, Player _player);
  }
}