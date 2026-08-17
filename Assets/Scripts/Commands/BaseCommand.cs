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

        public abstract bool CanHandle(CommandContext cxt);
        public abstract void Handle(CommandContext cxt);

        public bool AllRestrictionsPass(Vector3 position){
            return Restrictions.Length == 0 || Restrictions.All(restriction => restriction.CanPlace(position));
        }

    }
}