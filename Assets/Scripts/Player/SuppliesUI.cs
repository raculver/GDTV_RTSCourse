using System;
using GameDevTV.RTS.Environment;
using GameDevTV.RTS.EventBus;
using GameDevTV.RTS.Events;
using GameDevTV.RTS.Units;
using NUnit.Framework.Internal.Commands;
using TMPro;
using UnityEngine;

namespace GameDevTV.RTS.Player{

public class SuppliesController:MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI suppliesTextMinerals;
    [SerializeField] private TextMeshProUGUI suppliesTextGas;
    [SerializeField] private TextMeshProUGUI suppliesTextPopulation;

    [SerializeField] private SupplySO mineralsSO;
    [SerializeField] private SupplySO gasSO;

    [SerializeField] private int initialMinerals = 500;
    [SerializeField] private int initialGas = 100;
    [SerializeField] private int initialPopulation = 40;

    public static int amountMinerals{get; private set;} = 0;
    public static int amountGas{get; private set;} = 0;
    public static int amountPopulation{get; private set;} = 0;
    public static int amountPopulationLimit{get; private set;} = 200;

    private void Start(){
        UpdateSupplyDisplay();
    }

    private void OnEnable(){
        InitialiseSupplies();
        Bus<SupplyEvent>.OnEvent += HandleSupplyEvent;
    }

    private void OnDisable(){
        Bus<SupplyEvent>.OnEvent -= HandleSupplyEvent;
    }

    private void HandleSupplyEvent(SupplyEvent args){
        SupplySO thisSupply = args.SuppliedSupplySO;
        int thisAmount = args.AmountSupplied;

        if (thisSupply.Equals(mineralsSO)){
            amountMinerals += thisAmount;
            suppliesTextMinerals.text = amountMinerals.ToString();
        }
        else if (thisSupply.Equals(gasSO)){
            amountGas += thisAmount;
            suppliesTextGas.text = amountGas.ToString();
        }
        // handle pop

    }

    private void UpdateSupplyDisplay(){
        suppliesTextMinerals.text = amountMinerals.ToString();
        suppliesTextGas.text = amountGas.ToString();
        suppliesTextPopulation.text = $"{amountPopulation/amountPopulationLimit}";
    }


    static public bool HasEnoughSupplies(SupplyCostSO cost){
         return cost.Minerals <= amountMinerals && cost.Gas <= amountGas;
    }

    private void InitialiseSupplies(){
        amountMinerals = initialMinerals;
        amountGas = initialGas;
        amountPopulation = initialPopulation;
    }
}

}