using System.Collections.Generic;
using System.Linq;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.UI.Containers;
using GameDevTV.RTS.Units;
using UnityEngine;

/* Mermaid attempt
flowchart LR
    RUI[RuntimeUI]-->|activates|UI["UI.Container.ActionUI"]-->|activates|UIC["UI.Component.UIActionButton"]
    UIC -->|inherits from|IUIE2["IUIElement(ActionBase,UnityAction)"]
    UI -->|inherits from|IUIE1["IUIElement(T)"]
    RUI -->|owns|L_AC["List<_AbstractCommandable_>"]-->AC[AbstractCommandable]
    AB[ActionBase]--> IUIE2
    q[?]-->AB
*/

/* Mermaid example of selecting a worker
flowchart TD
    subgraph ActionsUI [ActionsUI]
        z[ActionsUI]
        G[ActionsUI.UIActionButtons]
        H[ActionsUI.EnableFor]
    end
    
    subgraph RuntimeUI [RuntimeUI]
        A[RuntimeUI.currentlySelected]
        RUI[RuntimeUI]
        K[RuntimeUI.HandleUnitSelected]
    end

    subgraph UIActionButton ["UIActionButton"]
        J0[UIActionButton]-->J[UIActionButton.EnableFor]
        J-->|calls|J1[UIActionButton.SetIcon]
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
    
    A -->|contains| B
    B --> C
    C -->|contains|D
    D -->E
    E -->|instance of|F

    z --> G
    G -->|contains| J0
    H --> |calls for each buytton in ui|J
    K-->|"HashSet<_AbstractCommandables_>"|H



    M -->|Subscribes| K

    F -->|gets Slot, Icon Sprite| H
    K1 -->|raises| M1
    M1 -->|subscribes|N1

    H-->|creates|K1
    %% Styling
    linkStyle 10,9,11,12,11,1,15 stroke:#22ff88,stroke-width:2.5px
    linkStyle 13,14 stroke:#e942f5,stroke-width:2.5px
*/
namespace GameDevTV.RTS.UI
{
public class RuntimeUI:MonoBehaviour{
    [SerializeField] private ActionsUI actionsUI;
    [SerializeField] private BuildingBuildingUI buildingBuildingUI;

    private HashSet<AbstractCommandable> currentlySelected = new(12);

    void Start(){
        DisableAll();
    }

    void OnEnable(){
        Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelected;
        Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselected;
    }

    void OnDisable(){
        Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelected;
        Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselected;
    }

    private void HandleUnitSelected(UnitSelectedEvent evt)
    {
        if (evt.Unit is AbstractCommandable commandable){
            currentlySelected.Add(commandable);
            actionsUI.EnableFor(currentlySelected);
            DebugLogging.Instance.Message($"Running Enablefor commandable {commandable}.", DebugLogging.Instance.REPORT_SELECTION);
        }

        if (currentlySelected.Count == 1 && evt.Unit is BaseBuilding building){
            buildingBuildingUI.EnableFor(building);
            DebugLogging.Instance.Message($"Running Enablefor building {building}.", DebugLogging.Instance.REPORT_SELECTION);
        }
    }

    private void HandleUnitDeselected(UnitDeselectedEvent evt)
    {
        if (evt.Unit is AbstractCommandable commandable)
        {
            currentlySelected.Remove(commandable);

            if (currentlySelected.Count > 0)
            {
                actionsUI.EnableFor(currentlySelected);

                if (currentlySelected.Count == 1 && currentlySelected.First() is BaseBuilding building)
                {
                    buildingBuildingUI.EnableFor(building);
                }
                else
                {
                    buildingBuildingUI.Disable();
                }
            }
            else
            {
                DisableAll();
            }
        }
    }


    private void DisableAll()
    {
        actionsUI.Disable();
        buildingBuildingUI.Disable();
    }
}
}


