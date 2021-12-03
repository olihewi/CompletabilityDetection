using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [Serializable]
  public class AutoJump : PlayerAbility
  {
    public float height = 1.0F;
    private bool wasGrounded = false;
    public override void Perform(Player _player)
    {
      bool isGrounded = _player.controller.isGrounded;
      Vector2 horizontalVelocity = new Vector2(_player.velocity.x,_player.velocity.z);
      if (!isGrounded && wasGrounded && horizontalVelocity.magnitude > 3.0F)
      {
        _player.velocity.y = Mathf.Sqrt(-2.0F * -9.81F * height);
      }
      wasGrounded = isGrounded;
    }
  }
}
