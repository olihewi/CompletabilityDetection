using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [CreateAssetMenu(fileName = "Movement", menuName = "Player Abilities/Movement")]
  public class Movement : PlayerAbility
  {
    public InputAction input;
    public float speed = 1.0F;
  
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
      if (!_player.isGrounded) return;
      Vector2 movementInput = input.ReadValue<Vector2>();
      Vector3 movement = _player.transform.forward * movementInput.y + _player.transform.right * movementInput.x;
      movement *= speed;
      _player.rb.AddForce(movement,ForceMode.Acceleration);
    }
  }
}

