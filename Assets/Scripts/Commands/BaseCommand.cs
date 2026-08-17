using System.Linq;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
    public abstract class BaseCommand : ScriptableObject, ICommand{
        [field:SerializeField] public Sprite Icon {get; private set;}
        [field:Range(0,8)][field:SerializeField] public int Slot {get; private set;}
        [field:SerializeField] public bool RequiresClickToActivate {get; private set;}
        [field:SerializeField] public GameObject GhostPrefab {get; private set;}
        [field: SerializeField]  public BuildingRestrictionSO[] Restrictions {get; private set;}

        // CanHandle asks "can we handle this, in this specific context?"
        public abstract bool CanHandle(CommandContext cxt);

        // Is locked is a way of telling the user "look, mate, don't even try..."
        // (building with not enough supplies)
        // (building units before the base building is complete)
        public abstract bool IsLocked(CommandContext cxt);

        // Handle actually executes the command
        public abstract void Handle(CommandContext cxt);

        // Check restrictions list. This is a dogshit place to put this? Only relates to base building.
        public bool AllRestrictionsPass(Vector3 position){
            return Restrictions.Length == 0 || Restrictions.All(restriction => restriction.CanPlace(position));
        }

    }
}