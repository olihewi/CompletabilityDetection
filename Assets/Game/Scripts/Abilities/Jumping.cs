using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [CreateAssetMenu(fileName = "Jumping", menuName = "Player Abilities/Jumping")]
  public class Jumping : PlayerAbility
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
  
    public override void Perform(Player _player)
    {
      if (_player.wasGrounded)
      {
        _player.rb.AddForce(Vector3.up * height, ForceMode.VelocityChange);
      }
    }
  }
}
