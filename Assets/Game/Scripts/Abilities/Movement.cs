using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [Serializable]
  public class Movement : PlayerAbility
  {
    public InputAction movement;
    public InputAction walk;
    public float speed = 1.0F;
    public float airMultiplier = 0.25F;
    public float acceleration = 1.0F;
    public float gravity = 9.81F;
  
    private void OnEnable()
    {
      movement.Enable();
      walk.Enable();
    }
    private void OnDisable()
    {
      movement.Disable();
      walk.Disable();
    }
  
    private static readonly int ANIM_VELOCITY = Animator.StringToHash("Velocity");
    private static readonly int ANIM_GROUNDED = Animator.StringToHash("Grounded");
    public override void Perform(Player _player)
    {
      // Horizontal Movement
      Vector2 movementInput = movement.ReadValue<Vector2>();
      Vector3 targetMovement = (_player.transform.forward * movementInput.y + _player.transform.right * movementInput.x) * speed;
      targetMovement *= 1.0F - walk.ReadValue<float>() * 0.5F;
      Vector3 targetVelocity = new Vector3(targetMovement.x,_player.velocity.y,targetMovement.z);
      float maxDistanceDelta = _player.controller.isGrounded ? acceleration * speed * Time.deltaTime : acceleration * speed * airMultiplier * Time.deltaTime;
      _player.velocity = Vector3.MoveTowards(_player.velocity, targetVelocity, maxDistanceDelta);
      
      // Gravity
      _player.velocity.y -= gravity * Time.deltaTime;
      if (_player.controller.isGrounded && _player.velocity.y < 0.0F) _player.velocity.y = -gravity * Time.deltaTime;
      
      // Animation
      Vector3 horizontalVelocity = new Vector3(_player.velocity.x,0.0F,_player.velocity.z);
      _player.animator.SetFloat(ANIM_VELOCITY, horizontalVelocity.magnitude / speed);
      _player.animator.SetBool(ANIM_GROUNDED, _player.controller.isGrounded);
      if (horizontalVelocity.magnitude > 0.1F) _player.animator.transform.rotation = Quaternion.LookRotation(horizontalVelocity);
    }
    public override void Traverse(Dictionary<Vector3Int, float> _completabilityGrid, Voxels.Volume _volume, Player _player)
    {
      Dictionary<Vector3Int, float> toAdd = new Dictionary<Vector3Int, float>();
      Vector3Int[] directions = 
      { Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left,
        //new Vector3Int(1,0,1), new Vector3Int(1,0,-1), new Vector3Int(-1,0,-1), new Vector3Int(-1,0,1)
      };
      foreach(KeyValuePair<Vector3Int, float> tile in _completabilityGrid)
      {
        // Horizontal Movement
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
            toAdd.Add(voxel, tile.Value + 1.0F/speed);
          }
        }
        // Gravity
        foreach (KeyValuePair<Vector3Int, Voxels.Voxel> otherTile in _volume.voxels)
        {
          if (otherTile.Key.y >= tile.Key.y) continue;
          if (_completabilityGrid.ContainsKey(otherTile.Key) || toAdd.ContainsKey(otherTile.Key)) continue;
          if ((new Vector3Int(otherTile.Key.x,0,otherTile.Key.z) - new Vector3Int(tile.Key.x,0,tile.Key.z)).magnitude <= tile.Key.y - otherTile.Key.y + 1.0F)
          {
            toAdd.Add(otherTile.Key, tile.Value + 0.25F);
          }
        }
      }
      foreach(KeyValuePair<Vector3Int,float> i in toAdd)
      {
        _completabilityGrid.Add(i.Key,i.Value);
      }
    }
  }
}

