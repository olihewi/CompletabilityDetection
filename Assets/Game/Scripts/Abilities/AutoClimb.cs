using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "Auto-Climb", menuName = "Player Abilities/Auto-Climb")]
    public class AutoClimb : PlayerAbility
    {
        public LayerMask layerMask;
        public float height = 1.0F;
        public override void Perform(Player _player)
        {
            if (!_player.controller.isGrounded) return;
            Vector3 start = _player.transform.position - new Vector3(0.0F, _player.controller.height / 2.0F - 0.5F, 0.0F);
            Vector3 direction = new Vector3(_player.velocity.x,0.0F,_player.velocity.z);
            if (Physics.Raycast(start, direction, _player.controller.radius + 0.5F, layerMask) &&
                !Physics.Raycast(start + new Vector3(0.0F, height, 0.0F), direction, _player.controller.radius + 0.5F, layerMask))
            {
                _player.velocity = new Vector3(0.0F, Mathf.Sqrt(-2.0F * -9.81F * (height + 0.1F)), 0.0F);
            }
        }
    }
}

