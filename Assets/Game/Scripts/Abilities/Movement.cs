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
    public override void Traverse(Dictionary<Vector3Int, CompletabilityData> _completabilityGrid, Voxels.Volume _volume, Player _player)
    {
      Dictionary<Vector3Int, CompletabilityData> toAdd = new Dictionary<Vector3Int, CompletabilityData>();
      Vector3Int[] directions = 
      { Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left,
        //new Vector3Int(1,0,1), new Vector3Int(1,0,-1), new Vector3Int(-1,0,-1), new Vector3Int(-1,0,1)
      };

      float jumpHeight = 0.01F;
      foreach (Player.AbilityInstance ability in _player.abilities)
      {
        if (ability.enabled && ability.ability is Jump jump)
        {
          jumpHeight = jump.height + _player.controller.radius;
          break;
        }
      }
      float vX = speed;
      float vY = Mathf.Sqrt(-2.0F * -9.81F * jumpHeight);
      float g = -Physics.gravity.y;
      foreach(KeyValuePair<Vector3Int, CompletabilityData> tile in _completabilityGrid)
      {
        foreach (KeyValuePair<Vector3Int, Voxels.Voxel> otherTile in _volume.voxels)
        {
          if (tile.Key == otherTile.Key || _completabilityGrid.ContainsKey(otherTile.Key) || toAdd.ContainsKey(otherTile.Key)) continue;
          
          // If the player can't fit, continue.
          bool playerCantFit = false;
          for (int i = 1; i < _player.controller.height + 1.0F; i++)
          {
            if (_volume.voxels.ContainsKey(otherTile.Key + Vector3Int.up * i)) playerCantFit = true;
          }
          if (playerCantFit) continue;
          
          // Projectile motion
          float h = tile.Key.y - otherTile.Key.y;
          if (-h > jumpHeight) continue;
          bool positiveH = h >= 0.0F;
          float t = (vY + Mathf.Sqrt(vY * vY + 2.0F * g * ( positiveH ? h : 0.0F))) / g;
          float d = vX * t * (positiveH ? 1.0F : 0.5F);
          Vector2 diff = new Vector2(otherTile.Key.x, otherTile.Key.z) - new Vector2(tile.Key.x, tile.Key.z);
          float dist = diff.magnitude;
          
          // If the player can reach the tile with a jump:
          if (dist - 1.0F < d)
          {
            // Check if there are any blocks in the way
            Vector2 dir = diff.normalized;
            float interval = 0.1F;
            for (float i = 0.0F; i < 1.0F; i += interval / t)
            {
              float it = interval / i;
              Vector3 pos = new Vector3(diff.x * i + tile.Key.x, tile.Key.y + ((vY * it) - (g * it * it)), diff.y * i + tile.Key.z);
              for (float j = 1.0F; j < _player.controller.height + 1.0F; j++)
              {
                Vector3Int hitPos = Vector3Int.RoundToInt(pos + Vector3.up * j);
                if (hitPos != otherTile.Key && _volume.voxels.ContainsKey(hitPos)) playerCantFit = true;
              }
            }
            if (playerCantFit) continue;
            float time = tile.Value.time + dist / speed;
            toAdd.Add(otherTile.Key,new CompletabilityData{time = time, from = tile.Key});
          }
        }
      }
      foreach(KeyValuePair<Vector3Int,CompletabilityData> i in toAdd)
      {
        _completabilityGrid.Add(i.Key,i.Value);
      }
    }
  }
}

