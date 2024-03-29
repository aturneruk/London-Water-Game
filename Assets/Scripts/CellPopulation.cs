﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPopulation : MonoBehaviour {

    public HexCell hexCell;
    public GridPopulation gridPopulation;

    public double Size;
    public double GrowthRate { get; set; }

    private void Awake() {
        Size = 0;
        GrowthRate = 0.00130d;
    }

    private void OnEnable() {
        GameTime.NewMonth += UpdatePopulation;
        GameTime.NewYear += PopulationIncome;
    }

    private void OnDisable() {
        GameTime.NewMonth -= UpdatePopulation;
        GameTime.NewYear -= PopulationIncome;
    }

    public override string ToString() {
        return Math.Round(Size).ToString(); ;
    }

    public void UpdatePopulation() {
        double waterSupplyRatio = hexCell.waterManager.supplyRatio;
        double waterSupplyFactor = 10.526f * (waterSupplyRatio * waterSupplyRatio) - 8.526f;

        double waterSupplyQuality = hexCell.waterManager.Supply.Quality;
        double waterQualityFactor = Mathf.InverseLerp(-1, 1, (float)waterSupplyQuality * (float)waterSupplyQuality);

        double factor;

        if (waterSupplyFactor < 0 && waterQualityFactor < 0) {
            factor = -Math.Abs(waterSupplyFactor) * Math.Abs(waterQualityFactor);
        }
        else {
            factor = waterSupplyFactor * waterQualityFactor;
        }

        Size += Size * GrowthRate * factor;
    }

    public void PopulationIncome() {
        if (Size > 0) {
            Money.AddMoney(Size);
        }
    }
}
