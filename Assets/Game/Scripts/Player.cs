using System.Collections.Generic;
using Game.Abilities;
using UnityEngine;

namespace Game
{
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(CapsuleCollider))]
  public class Player : MonoBehaviour
  {
    public List<PlayerAbility> abilities;
    public Transform cameraPivot;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider cc;

    private void Awake()
    {
      rb = GetComponent<Rigidbody>();
      cc = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
      foreach (PlayerAbility ability in abilities)
      {
        ability.Perform(this);
      }

      grounded = Physics.CheckCapsule(cc.bounds.center, new Vector3(cc.bounds.center.x, cc.bounds.min.y - 0.1F, cc.bounds.center.z), cc.radius, ~(1 << 6));
    }

    private bool grounded;

    public bool isGrounded
    {
      get { return grounded; }
    }

    public bool wasGrounded
    {
      get { return isGrounded && !Physics.CheckCapsule(cc.bounds.center, new Vector3(cc.bounds.center.x, cc.bounds.min.y - 0.1F, cc.bounds.center.z), cc.radius, ~(1 << 6)); }
    }
  }
}