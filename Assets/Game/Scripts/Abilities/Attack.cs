using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Abilities
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Player Abilities/Attack")]
    public class Attack : PlayerAbility
    {
        public InputAction attack;
        private static readonly int ANIM_ATTACK = Animator.StringToHash("Attack");

        private void OnEnable()
        {
            attack.Enable();
        }

        private void OnDisable()
        {
            attack.Disable();
        }

        public override void Perform(Player _player)
        {
            if (attack.triggered)
            {
                _player.animator.SetTrigger(ANIM_ATTACK);
            }
        }
    }
}

