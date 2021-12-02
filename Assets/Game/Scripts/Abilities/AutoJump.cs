using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [CreateAssetMenu(fileName = "Auto-Jump", menuName = "Player Abilities/Jumping")]
  public class AutoJump : PlayerAbility
  {
    public InputAction input;
    public float height = 1.0F;
  
    private void OnEnable()
    {
      input.Enable();
    }
    private void OnDisable()
    {
      input.Disable();
    }

    private bool wasGrounded = false;
    public override void Perform(Player _player)
    {
      bool isGrounded = _player.controller.isGrounded;
      if (!isGrounded && wasGrounded)
      {
        _player.velocity.y = Mathf.Sqrt(-2.0F * -9.81F * height);
      }
      wasGrounded = isGrounded;
    }
  }
}
