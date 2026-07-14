using GameDevTV.RTS.UI;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
/*
flowchart TD
    subgraph ActionsUI [ActionsUI]
        z[ActionsUI]
        A[ActionsUI.currentlySelected]
        G[ActionsUI.UIActionButtons]
        H[ActionsUI.RefreshButtons]
        K[ActionsUI.HandleUnitSelected]
    end
    
    subgraph UIActionButton ["UIActionButton"]
        J0[UIActionButton]-->J[UIActionButton.SetIcon]
    end

    subgraph CommandSystem
        B[AbstractCommandable]
        C[AbstractCommandable.AvailableCommands]
    end
    subgraph WorkerSO
        D[ActionBase]
        E[MoveCommand]
        F[WorkerSO]
    end

    subgraph EventBus
        M[Bus UnitSelectedEvent]
    end

    %% Connections
    z --> A
    A -->|contains| B
    B --> C
    C -->|contains|D
    D -->E
    E -->|instance of|F

    z --> G
    G -->|contains| J0
    H --> |calls|J
    K-->|calls|H

    M -->|Subscribes| K
    F -->|gets Slot, Icon Sprite| H

    %% Styling
    linkStyle 9,11,12,10 stroke:#22ff88,stroke-width:2.5px
*/
    public abstract class ActionBase : ScriptableObject, ICommand{
        [field:SerializeField] public Sprite Icon {get; private set;}
        [field:Range(0,8)][field:SerializeField] public int Slot {get; private set;}
        [field:SerializeField] public bool RequiresClickToActivate {get; private set;}

        public abstract bool CanHandle(CommandContext cxt);
        public abstract void Handle(CommandContext cxt);
    }
}