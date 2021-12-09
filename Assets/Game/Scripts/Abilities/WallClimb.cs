using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxels;

namespace Game.Abilities
{
  public class WallClimb : PlayerAbility
  {
    public InputAction climb;
    public InputAction drop;
    public List<Voxel> climbable = new List<Voxel>();
    public float speed = 3.0F;
    private bool climbing = false;
    private static readonly int ANIM_CLIMBING = Animator.StringToHash("Climbing");
    private static readonly int ANIM_CLIMBX = Animator.StringToHash("ClimbX");
    private static readonly int ANIM_CLIMBY = Animator.StringToHash("ClimbY");

    private void OnEnable()
    {
      climb.Enable();
      drop.Enable();
    }

    private void OnDisable()
    {
      climb.Disable();
      drop.Disable();
    }

    private Vector3 climbDirection;
    public override void Perform(Player _player)
    {
      climbDirection = Vector3.MoveTowards(climbDirection, climb.ReadValue<Vector2>(), 5.0F * Time.deltaTime);
      // If climbing is still valid
      Vector3 start = _player.transform.position - new Vector3(0.0F, _player.controller.height / 2.0F - 0.25F, 0.0F);
      Vector3 direction = _player.animator.transform.forward;
      float maxDistance = _player.controller.radius + 0.1F;
      Volume.VoxelCastHit hit = Volume.VoxelCastAll(start, direction, maxDistance);
      bool continueClimbing = (hit.hit) && climbable.Contains(hit.voxel);
      climbing = continueClimbing && (climbing ? !_player.controller.isGrounded : climbDirection.y > 0.0F);
      if (climbing)
      {
        if (drop.triggered)
        {
          _player.velocity = hit.normal * 2.0F + new Vector3(0.0F, Mathf.Sqrt(-2.0F * -9.81F) * 0.5F, 0.0F);
          climbing = false;
        }
        else
        {
          _player.CaptureAbility(this);
          Vector3 relativeHorizontal = Vector3.Cross(hit.normal, Vector3.up) * (climbDirection.x * speed);
          _player.velocity = new Vector3(relativeHorizontal.x, climbDirection.y * speed, relativeHorizontal.z);
          _player.animator.transform.rotation = Quaternion.LookRotation(-hit.normal);
        }
      }
      _player.animator.SetBool(ANIM_CLIMBING, climbing);
      _player.animator.SetFloat(ANIM_CLIMBX, climbDirection.x);
      _player.animator.SetFloat(ANIM_CLIMBY, climbDirection.y);
    }
    public override void Traverse(Dictionary<Vector3Int, float> _completabilityGrid, Volume _volume, Player _player)
    {
      return;
    }
  }
}

