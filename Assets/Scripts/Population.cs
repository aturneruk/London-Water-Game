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
        GameTime.NewYear += UpdatePopulation;
    }

    private void OnDisable() {
        GameTime.NewYear -= UpdatePopulation;
    }

    public override string ToString() {
        return Mathf.Round(Size).ToString(); ;
    }

    public void UpdatePopulation() {
        float waterSupplyRatio = hexCell.waterManager.supplyRatio;
        float waterSupplyFactor = 10.526f * (waterSupplyRatio * waterSupplyRatio) - 8.526f;

        float waterSupplyQuality = hexCell.waterManager.Supply.Quality;
        float waterQualityFactor = Mathf.InverseLerp(-1, 1, waterSupplyQuality * waterSupplyQuality);

        float factor;

        if (waterSupplyFactor < 0 && waterQualityFactor < 0) {
            factor = -Mathf.Abs(waterSupplyFactor) * Mathf.Abs(waterQualityFactor);
        }
        else {
            factor = waterSupplyFactor * waterQualityFactor;
        }


        Size += Size * GrowthRate * factor;
    }
}
