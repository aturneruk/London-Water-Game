using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    HexCell hexCell;
    public double Size { get; set; }
    public double GrowthRate { get; set; }

    private void Awake() {
        hexCell = gameObject.GetComponent<HexCell>();
        Size = 0;
        GrowthRate = 0.0004f;
    }

    private void OnEnable() {
        GameTime.NewMonth += UpdatePopulation;
    }

    private void OnDisable() {
        GameTime.NewMonth -= UpdatePopulation;
    }

    public override string ToString() {
        return Mathf.Round((float)Size).ToString(); ;
    }

    public void UpdatePopulation() {
        double waterSupplyRatio = hexCell.waterManager.supplyRatio;
        double waterSupplyFactor = 10.526f * (waterSupplyRatio * waterSupplyRatio) - 8.526f;

        double waterSupplyQuality = hexCell.waterManager.Supply.Quality;
        double waterQualityFactor = Mathf.InverseLerp(-1, 1, (float)waterSupplyQuality * (float)waterSupplyQuality);

        double factor;

        if (waterSupplyFactor < 0 && waterQualityFactor < 0) {
            factor = -Mathf.Abs((float)waterSupplyFactor) * Mathf.Abs((float)waterQualityFactor);
        }
        else {
            factor = waterSupplyFactor * waterQualityFactor;
        }

        Size += Size * GrowthRate * factor;
    }
}
