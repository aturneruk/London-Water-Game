using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    HexCell hexCell;
    public float Size { get; set; }
    public float GrowthRate { get; set; }

    private void Awake() {
        hexCell = gameObject.GetComponent<HexCell>();
        Size = 0;
        GrowthRate = 0.0004f;
    }

    private void OnEnable() {
        GameTime.NewWeek += UpdatePopulation;
    }

    private void OnDisable() {
        GameTime.NewWeek -= UpdatePopulation;
    }

    public override string ToString() {
        return Mathf.Round(Size).ToString(); ;
    }

    public void UpdatePopulation() {
        float waterSupplyRatio = hexCell.waterManager.supplyRatio;
        float waterSupplyFactor = 10.526f * (waterSupplyRatio * waterSupplyRatio) - 8.526f;

        float factor = waterSupplyFactor;

        Size += Size * GrowthRate * factor;
    }
}
