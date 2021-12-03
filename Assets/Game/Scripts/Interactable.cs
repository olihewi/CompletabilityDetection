using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Interactable : MonoBehaviour
    {
        public abstract void Interact(Player _player);
    }
}

