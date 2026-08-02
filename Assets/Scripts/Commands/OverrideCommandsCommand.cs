using UnityEngine;

namespace GameDevTV.RTS.Commands
{
[CreateAssetMenu(fileName = "Override Commands", menuName = "Units/Commands/ActionsOverride", order = 110)]
public class OverrideCommandsCommand : ActionBase
{
    [field: SerializeField] public ActionBase[] newCommands {get; private set;}

    public override bool CanHandle(CommandContext cxt)
    {
        return cxt.Commandable != null;
    }

    public override void Handle(CommandContext cxt){
        cxt.Commandable.SetCommandsOverride(newCommands);
    }
}
}