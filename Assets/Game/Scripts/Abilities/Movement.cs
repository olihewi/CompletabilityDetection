using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
  [CreateAssetMenu(fileName = "Movement", menuName = "Player Abilities/Movement")]
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
      if (_player.controller.isGrounded) _player.velocity.y = -gravity * Time.deltaTime;
      
      // Animation
      Vector3 horizontalVelocity = new Vector3(_player.velocity.x,0.0F,_player.velocity.z);
      _player.animator.SetFloat(ANIM_VELOCITY, horizontalVelocity.magnitude / speed);
      _player.animator.SetBool(ANIM_GROUNDED, _player.controller.isGrounded);
      if (horizontalVelocity.magnitude > 0.1F) _player.animator.transform.rotation = Quaternion.LookRotation(horizontalVelocity);
    }
  }
}

