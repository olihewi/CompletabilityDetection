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
  
    private static readonly int VELOCITY = Animator.StringToHash("Velocity");
    public override void Perform(Player _player)
    {
      //if (!_player.controller.isGrounded) return;
      Vector2 movementInput = input.ReadValue<Vector2>();
      Vector3 movement = _player.transform.forward * movementInput.y + _player.transform.right * movementInput.x;
      movement *= speed;
      //_player.velocity += movement * Time.deltaTime;
      _player.velocity = Vector3.Lerp(_player.velocity,movement, Time.deltaTime * 4.0F);
      _player.animator.SetFloat(VELOCITY, new Vector2(_player.velocity.x,_player.velocity.z).magnitude / speed);
      if (_player.velocity.magnitude > 0.1F) _player.animator.transform.rotation = Quaternion.LookRotation(_player.velocity);
    }
  }
}

