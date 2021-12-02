using System.Collections.Generic;
using Game.Abilities;
using UnityEngine;

namespace Game
{
  [RequireComponent(typeof(CharacterController))]
  public class Player : MonoBehaviour
  {
    public List<PlayerAbility> abilities;
    public Transform cameraPivot;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;

    private void Awake()
    {
      controller = GetComponent<CharacterController>();
      animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
      foreach (PlayerAbility ability in abilities)
      {
        ability.Perform(this);
      }
      Move();
    }

    public Vector3 velocity;

    private void Move()
    {
      controller.Move(velocity * Time.deltaTime);
    }
  }
}