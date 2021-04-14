using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population {

    HexCell cell;
    public int Size { get; set; }
    public float GrowthRate { get; set; }

    public Population(HexCell cell) {
        this.cell = cell;
        Size = 0;
        GrowthRate = 0.0004f;
    }

    public Population(HexCell cell, int size) {
        this.cell = cell;
        Size = size;
        GrowthRate = 0.0004f;
    }

    public Population(HexCell cell, int size, float growthRate) {
        this.cell = cell;
        Size = size;
        GrowthRate = growthRate;
    }

    public override string ToString() {
        return Size.ToString();
    }

    public void GrowPopulation() {
        float waterSupplyRatio = cell.waterManager.supplyRatio;
        float waterSupplyFactor = 10.526f * (waterSupplyRatio * waterSupplyRatio) - 8.526f;

        float factor = waterSupplyFactor;

        Size += Mathf.RoundToInt((float)Size * GrowthRate * factor);
    }
}
