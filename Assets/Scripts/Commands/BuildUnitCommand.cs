using GameDevTV.RTS.Player;
using GameDevTV.RTS.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace GameDevTV.RTS.Commands
{
[CreateAssetMenu(fileName = "Build Unit", menuName = "Buildings/Commands/Build Unit", order = 120)]
public class BuildUnitCommand : BaseCommand
{
    // ====== Weird Shared Reference Across ALL BuildUnitCommand ====== 
    // Because BaseCommand inherits from ScriptableObject, it is an asset (a file on disk). 
    // When you put a BuildUnitCommand into the AvailableCommands array on the prefab, every instantiated unit 
    // gets a reference to that same asset. They do not get their own separate copy.
    //
    // There is only ever one instance of BuildUnitCommand.
/*
no idea how the f this works...
flowchart TD
    subgraph ActionsUI [ActionsUI]
        z[ActionsUI]
        A[ActionsUI.currentlySelected]
        G[ActionsUI.UIActionButtons]
        H[ActionsUI.RefreshButtons]
        K[ActionsUI.HandleUnitSelected]
    end
    
    subgraph UIActionButton ["UIActionButton"]
        J0[UIActionButton]-->J[UIActionButton.EnableFor]
        J-->|calls|J1[UIActionButton.SetIcon]
    end

    subgraph CommandSystem
        B[AbstractCommandable]
        C[AbstractCommandable.AvailableCommands]
    end

    subgraph BaseBuilding
        BB0[BaseBuilding]
        BB1[BaseBuilding.BuildUnit]
    end

    subgraph BuildUnitCommand
        BUC[BuildUnitCommand]
        BUC0[BuildUnitCommand.Handle]
        BUC1[BuildUnitCommand.CanHandle]
    end

    subgraph UnitSO
        USO[UnitSO]
        D[Health]
        E[Prefab]
        F[BuildTime]
    end

    subgraph BaseCommandSO
        ABSO[BaseCommandSO]
        ABI[BaseCommandSO.Icon]
        ABS[BaseCommandSO.Slot]
        ABRC[BaseCommandSO.RequiresClickToActivate]
    end

    subgraph UnitSelectedEvent Bus
        M[Bus UnitSelectedEvent]
    end

    K1[ActionsUI.HandleClick]

    subgraph ActionSelectedEvent Bus
        M1[Bus ActionSelectedEvent]
    end

    subgraph PlayerInput
        N1[PlayerInput.HandleActionSelected]
    end

    %% Connections
    z --> A
    A -->|contains| BB0
    BB0 -->|inherits from| B
    B --> C

    USO-->|owns|D
    USO-->|owns|E
    USO-->|owns|F

    ABSO-->|owns|ABI
    ABSO-->|owns|ABS
    ABSO-->|owns|ABRC

    BUC0-->|calls|BB1
    BUC--> BUC0
    BUC--> BUC1

    BUC -->|inherits from|ABSO

    z --> G
    G -->|contains| J0
    H --> |calls|J
    K-->|calls|H

    M -->|Subscribes| K
%%    F -->|gets Slot, Icon Sprite| H
    K1 -->|raises| M1
    M1 -->|subscribes|N1

    H-->|creates|K1
    %% Styling
    linkStyle 10,12,13,11,15,1,16 stroke:#22ff88,stroke-width:2.5px
    linkStyle 14,15 stroke:#e942f5,stroke-width:2.5px
*/

    [field:SerializeField] public AbstractUnitSO UnitToBuild{get; private set;}
    
    public override bool CanHandle(CommandContext cxt){
        bool CanYouHandleIt = cxt.Commandable is BaseBuilding && HasEnoughSupplies();
        
        return CanYouHandleIt;
    }

    public override void Handle(CommandContext cxt){
        if (!HasEnoughSupplies()) return;

        BaseBuilding building = (BaseBuilding)cxt.Commandable;
        building.BuildUnit(UnitToBuild);
    }

    public override bool IsLocked(CommandContext cxt) => !HasEnoughSupplies();

    private bool HasEnoughSupplies(){
    // return UnitToBuild.Cost.Minerals <= SuppliesController.amountMinerals
    //     && UnitToBuild.Cost.Gas <= SuppliesController.amountGas;
    return SuppliesController.HasEnoughSupplies(UnitToBuild.Cost);
    }
}
}