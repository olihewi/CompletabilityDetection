namespace Game
{
    public class AbilityDispenser : Interactable
    {
        public override void Interact(Player _player)
        {
            //_player.abilities.AddRange(GetComponents<Abilities.PlayerAbility>());
            gameObject.SetActive(false);
        }
    }
}

