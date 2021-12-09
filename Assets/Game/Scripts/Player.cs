using System;
using System.Collections.Generic;
using Game.Abilities;
using UnityEngine;

namespace Game
{
  [RequireComponent(typeof(CharacterController))]
  public class Player : MonoBehaviour
  {
    public List<PlayerAbility> abilities = new List<PlayerAbility>();
    public Transform cameraPivot;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;
    [HideInInspector] public GameObject abilityHolder;

    private PlayerAbility capturedAbility;

    [ContextMenu("Register Abilities")]
    public void LoadAbilities()
    {
      abilities.Clear();
      abilities.AddRange(GetComponentsInChildren<PlayerAbility>());
    }

    private void OnEnable()
    {
      controller = GetComponent<CharacterController>();
      animator = GetComponentInChildren<Animator>();
      abilityHolder = GetComponentInChildren<PlayerAbility>().gameObject;
      LoadAbilities();
    }

    private void Update()
    {
      if (capturedAbility != null)
      {
        PlayerAbility ability = capturedAbility;
        capturedAbility = null;
        ability.Perform(this);
      }
      else
      {
        foreach (PlayerAbility ability in abilities)
        {
          ability.Perform(this);
        }
      }
      Move();
    }

    public void CaptureAbility(PlayerAbility _ability)
    {
      capturedAbility = _ability;
    }

    public Vector3 velocity;

    private void Move()
    {
      controller.Move(velocity * Time.deltaTime);
    }
  }
}